using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tekno.Application.Catalog.DTOs.Admin;
using Tekno.Application.Catalog.DTOs.Products;
using Tekno.Application.Catalog.Interface;
using Tekno.Application.Common;
using Tekno.Application.Common.Cache;
using Tekno.Application.Common.Exceptions;
using Tekno.Application.Common.Interfaces;
using Tekno.Application.Common.Media.Services;
using Tekno.Application.Common.Paging;
using Tekno.Domain.Catalog;

namespace Tekno.Application.Catalog.Services
{
    public class ProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IElasticProductService _elasticService;
        private readonly IMapper _mapper;
        private readonly MediaService _mediaService;
        private readonly IAppLogger<ProductService> _logger;
        private readonly ICacheService _cacheService;

        public ProductService(
            IProductRepository productRepository,
            IElasticProductService elasticService,
            IMapper mapper,
            MediaService mediaService,
            IAppLogger<ProductService> logger,
            ICacheService cacheService)
        {
            _productRepository = productRepository;
            _elasticService = elasticService;
            _mapper = mapper;
            _mediaService = mediaService;
            _logger = logger;
            _cacheService = cacheService;
        }

        public async Task<PagedResult<ProductSummaryDto>> GetPagedProductAsync(ProductSearchRequestDto request)
        {
            var paging = new PagingParams(request.Page, request.PageSize);

            // Use ES when keyword or spec filters present
            if (!string.IsNullOrEmpty(request.Keyword) || (request.Filters != null && request.Filters.Any()))
            {
                // Generate cache key for this search
                var cacheKey = CachePolicies.SearchProductsKey(
                    request.Keyword,
                    request.Category,
                    request.Brand,
                    request.Filters,
                    request.MinPrice,
                    request.MaxPrice,
                    request.Sort,
                    paging.Page,
                    paging.PageSize);

                // Try to get from cache
                var cachedResults = await _cacheService.GetAsync<PagedResult<ProductSummaryDto>>(cacheKey);
                if (cachedResults != null)
                {
                    _logger.LogInformation("Retrieved search results from cache for keyword: {Keyword}", request.Keyword);
                    return cachedResults;
                }

                // Cache miss - search Elasticsearch
                var searchResults = await _elasticService.SearchProductsAsync(
                    request.Keyword,
                    request.Category,
                    request.Brand,
                    request.Filters,
                    request.MinPrice,
                    request.MaxPrice,
                    request.Sort,
                    paging.Page,
                    paging.PageSize);

                // Enrich with rating data
                await EnrichWithRatingDataAsync(searchResults.Data.ToList());

                // Cache the search results
                await _cacheService.SetAsync(cacheKey, searchResults, CachePolicies.SearchTtl);
                
                _logger.LogInformation("Search results cached for keyword: {Keyword}, category: {Category}, brand: {Brand}", 
                    request.Keyword, request.Category, request.Brand);

                return searchResults;
            }

            // Fallback to database
            var pagedResult = await _productRepository.GetPagedProductAsync(
                request.Category,
                request.Brand,
                null,
                request.Sort,
                request.MinPrice?.ToString(),
                request.MaxPrice?.ToString(),
                paging);

            var mapped = _mapper.Map<List<ProductSummaryDto>>(pagedResult.Data);
            
            // Enrich with rating data
            await EnrichWithRatingDataAsync(mapped);

            return new PagedResult<ProductSummaryDto>(mapped, pagedResult.TotalRecords, paging.Page, paging.PageSize);
        }

        public async Task<ProductDetailDto?> GetProductDetailAsync(string slug)
        {
            if (string.IsNullOrWhiteSpace(slug))
            {
                _logger.LogWarning("GetProductDetailAsync called with empty slug");
                return null;
            }

            var product = await _productRepository.GetProductBySlugAsync(slug);
            if (product == null)
            {
                _logger.LogInformation("Product not found with slug: {Slug}", slug);
                return null;
            }

            return _mapper.Map<ProductDetailDto>(product);
        }

        public async Task<ProductDetailDto?> GetProductDetailByIdAsync(int id)
        {
            var product = await _productRepository.GetProductByIdAsync(id);
            if (product == null)
            {
                _logger.LogInformation("Product not found with ID: {Id}", id);
                return null;
            }

            return _mapper.Map<ProductDetailDto>(product);
        }

        public async Task<CreateProductDto> CreateProductAsync(CreateProductDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            // Check slug existence in both DB and ES in parallel
            var existsInDbTask = _productRepository.IsProductExistBySlugAsync(dto.Slug);
            var existsInEsTask = _elasticService.IsProductExistBySlug(dto.Slug);
            await Task.WhenAll(existsInDbTask, existsInEsTask);

            if (existsInDbTask.Result || existsInEsTask.Result)
            {
                throw new InvalidOperationException($"Product with slug '{dto.Slug}' already exists");
            }

            var uploadedImages = new List<string>();
            await using var transaction = await _productRepository.BeginTransactionAsync();

            try
            {
                // Map and create product
                var newProduct = _mapper.Map<Product>(dto);

                // Upload images
                int sort = 0;
                foreach (var file in dto.Images ?? Enumerable.Empty<Microsoft.AspNetCore.Http.IFormFile>())
                {
                    var imageUrl = await _mediaService.UploadImageAsync(file, $"tekno/product/{dto.Slug}");
                    uploadedImages.Add(imageUrl);
                    newProduct.AddImage(imageUrl, isPrimary: sort == 0, sortOrder: sort++);
                }

                // Save to DB
                newProduct = await _productRepository.AddProductAsync(newProduct);
                _logger.LogInformation("Created product {ProductName} (ID: {ProductId}) with {ImageCount} images",
                    dto.Name, newProduct.Id, uploadedImages.Count);

                // Index to Elasticsearch
                var summary = _mapper.Map<ProductSummaryDto>(newProduct);
                await _elasticService.IndexProductAsync(summary);

                await transaction.CommitAsync();

                // Invalidate new products cache for this category
                await InvalidateNewProductsCacheAsync(newProduct.CategoryId);
                
                // Invalidate search cache
                InvalidateSearchCache();

                return _mapper.Map<CreateProductDto>(newProduct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create product {Name}", dto.Name);
                await transaction.RollbackAsync();

                // Clean up uploaded images
                foreach (var img in uploadedImages)
                {
                    try { await _mediaService.DeleteImageAsync(img); }
                    catch (Exception cleanupEx)
                    {
                        _logger.LogWarning(cleanupEx, "Failed to delete image {ImageUrl} during rollback", img);
                    }
                }

                throw;
            }
        }

        public async Task<ProductDetailDto?> UpdateProductAsync(int id, CreateProductDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var existing = await _productRepository.GetProductByIdAsync(id);
            if (existing == null)
            {
                _logger.LogWarning("Update failed: Product with ID {Id} not found", id);
                return null;
            }

            var uploadedImages = new List<string>();
            await using var transaction = await _productRepository.BeginTransactionAsync();

            try
            {
                // Check if slug changed and new slug already exists
                if (existing.Slug != dto.Slug)
                {
                    var slugExistsDbTask = _productRepository.IsProductExistBySlugAsync(dto.Slug);
                    var slugExistsEsTask = _elasticService.IsProductExistBySlug(dto.Slug);
                    await Task.WhenAll(slugExistsDbTask, slugExistsEsTask);

                    if (slugExistsDbTask.Result || slugExistsEsTask.Result)
                    {
                        throw new InvalidOperationException($"Product with slug '{dto.Slug}' already exists");
                    }
                }

                // Update scalar properties (null checks)
                if (!string.IsNullOrWhiteSpace(dto.Name)) existing.Name = dto.Name;
                if (!string.IsNullOrWhiteSpace(dto.Slug)) existing.Slug = dto.Slug;
                if (dto.CategoryId > 0) existing.CategoryId = dto.CategoryId;
                if (dto.BrandId > 0) existing.BrandId = dto.BrandId;
                if (dto.BasePrice > 0) existing.BasePrice = dto.BasePrice;
                
                existing.Description = dto.Description;
                existing.Overview = dto.Overview;
                existing.DiscountPercent = dto.DiscountPercent;
                existing.Status = dto.Status ?? existing.Status;
                existing.UpdatedAt = DateTime.UtcNow;

                // Upload new images if provided
                if (dto.Images != null && dto.Images.Any())
                {
                    int sort = existing.Images?.Count ?? 0;
                    foreach (var file in dto.Images)
                    {
                        var imageUrl = await _mediaService.UploadImageAsync(file, $"tekno/product/{dto.Slug}");
                        uploadedImages.Add(imageUrl);
                        existing.AddImage(imageUrl, isPrimary: sort == 0, sortOrder: sort++);
                    }
                }

                // Persist changes
                var updated = await _productRepository.UpdateProductAsync(existing);

                // Reindex in Elasticsearch
                var summary = _mapper.Map<ProductSummaryDto>(updated);
                await _elasticService.IndexProductAsync(summary);

                await transaction.CommitAsync();

                _logger.LogInformation("Updated product ID {ProductId} ({ProductName})", id, dto.Name);

                // Invalidate new products cache for this category
                await InvalidateNewProductsCacheAsync(updated.CategoryId);
                
                // Invalidate search cache
                InvalidateSearchCache();

                return _mapper.Map<ProductDetailDto>(updated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update product ID {Id}", id);
                await transaction.RollbackAsync();

                // Clean up uploaded images
                foreach (var img in uploadedImages)
                {
                    try { await _mediaService.DeleteImageAsync(img); }
                    catch (Exception cleanupEx)
                    {
                        _logger.LogWarning(cleanupEx, "Failed to delete image {ImageUrl} during rollback", img);
                    }
                }

                throw;
            }
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var product = await _productRepository.GetProductByIdAsync(id);
            if (product == null)
            {
                _logger.LogWarning("Delete failed: Product with ID {Id} not found", id);
                return false;
            }

            var categoryId = product.CategoryId; // Store before deletion

            await using var transaction = await _productRepository.BeginTransactionAsync();

            try
            {
                // Delete from database
                await _productRepository.DeleteProductAsync(product);

                // Delete from Elasticsearch
                await _elasticService.DeleteProductAsync(id);

                await transaction.CommitAsync();

                // Invalidate new products cache for this category
                await InvalidateNewProductsCacheAsync(categoryId);
                
                // Invalidate search cache
                InvalidateSearchCache();

                // Clean up images from media store (after commit - best effort)
                foreach (var img in product.Images.Select(i => i.ImageUrl).ToList())
                {
                    try { await _mediaService.DeleteImageAsync(img); }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to delete image {ImageUrl} for product ID {ProductId}", img, id);
                    }
                }

                _logger.LogInformation("Deleted product ID {ProductId} ({ProductName})", id, product.Name);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete product ID {Id}", id);
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<ProductVariantDetailDto?> GetProductVariantByIdAsync(int variantId)
        {
            if (variantId <= 0)
            {
                _logger.LogWarning("GetProductVariantByIdAsync called with invalid ID: {VariantId}", variantId);
                return null;
            }

            var variant = await _productRepository.GetProductVariantByIdAsync(variantId);
            if (variant == null)
            {
                _logger.LogInformation("Product variant not found with ID: {VariantId}", variantId);
                return null;
            }

             return _mapper.Map<ProductVariantDetailDto>(variant);
        }

        public async Task<PagedResult<AdminProductListDto>> GetAdminProductsPagedAsync(AdminProductSearchDto request)
        {
            var paging = new PagingParams(request.Page, request.PageSize);

            var result = await _productRepository.GetAdminProductsPagedAsync(
                request.Search,
                request.Category,
                request.Brand,
                request.Status,
                paging);

            var dtos = _mapper.Map<List<AdminProductListDto>>(result.Data);

             return new PagedResult<AdminProductListDto>(dtos, result.TotalRecords, paging.Page, paging.PageSize);
        }

        public async Task<ProductImageDto> AddProductImageAsync(AddProductImageDto dto)
        {
            var product = await _productRepository.GetProductByIdAsync(dto.ProductId);
            if (product == null)
                throw new NotFoundException("Product", dto.ProductId);

            string? uploadedImageUrl = null;
            await using var transaction = await _productRepository.BeginTransactionAsync();

            try
            {
                uploadedImageUrl = await _mediaService.UploadImageAsync(dto.ImageFile, $"tekno/product/{product.Slug}");

                var images = await _productRepository.GetProductImagesAsync(dto.ProductId);
                var nextSortOrder = images.Any() ? images.Max(i => i.SortOrder) + 1 : 0;

                if (dto.IsPrimary && images.Any(i => i.IsPrimary))
                {
                    var currentPrimary = images.First(i => i.IsPrimary);
                    currentPrimary.SetPrimary(false);
                    await _productRepository.UpdateProductImageAsync(currentPrimary);
                }

                var productImage = new ProductImage(dto.ProductId, uploadedImageUrl, dto.IsPrimary, nextSortOrder);

                var created = await _productRepository.AddProductImageAsync(productImage);

                var updatedProduct = await _productRepository.GetProductByIdAsync(dto.ProductId);
                var summary = _mapper.Map<ProductSummaryDto>(updatedProduct);
                await _elasticService.IndexProductAsync(summary);

                await transaction.CommitAsync();

                _logger.LogInformation("Added image to product {ProductId}: {ImageUrl}", dto.ProductId, uploadedImageUrl);

                return new ProductImageDto
                {
                    Id = created.Id,
                    ProductId = created.ProductId,
                    ImageUrl = created.ImageUrl,
                    IsPrimary = created.IsPrimary,
                    SortOrder = created.SortOrder
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add image to product {ProductId}", dto.ProductId);
                await transaction.RollbackAsync();

                if (uploadedImageUrl != null)
                {
                    try { await _mediaService.DeleteImageAsync(uploadedImageUrl); }
                    catch (Exception cleanupEx) { _logger.LogWarning(cleanupEx, "Failed to delete image during rollback"); }
                }

                throw;
            }
        }
        public async Task<bool> DeleteProductImageAsync(int imageId)
        {
            var image = await _productRepository.GetProductImageByIdAsync(imageId);
            if (image == null)
            {
                _logger.LogWarning("Delete failed: Image {ImageId} not found", imageId);
                return false;
            }

            var productId = image.ProductId;
            var imageUrl = image.ImageUrl;

            await using var transaction = await _productRepository.BeginTransactionAsync();

            try
            {
                var deleted = await _productRepository.DeleteProductImageAsync(imageId);
                if (!deleted)
                {
                    await transaction.RollbackAsync();
                    return false;
                }

                var product = await _productRepository.GetProductByIdAsync(productId);
                var summary = _mapper.Map<ProductSummaryDto>(product);
                await _elasticService.IndexProductAsync(summary);

                await transaction.CommitAsync();

                try { await _mediaService.DeleteImageAsync(imageUrl); }
                catch (Exception ex) { _logger.LogWarning(ex, "Failed to delete image from cloud"); }

                _logger.LogInformation("Deleted image {ImageId} from product {ProductId}", imageId, productId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete image {ImageId}", imageId);
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> UpdateProductImageAsync(UpdateProductImageDto dto)
        {
            var image = await _productRepository.GetProductImageByIdAsync(dto.ImageId);
            if (image == null)
            {
                _logger.LogWarning("Update failed: Image {ImageId} not found", dto.ImageId);
                return false;
            }

            await using var transaction = await _productRepository.BeginTransactionAsync();

            try
            {
                if (dto.IsPrimary.HasValue && dto.IsPrimary.Value)
                {
                    var images = await _productRepository.GetProductImagesAsync(image.ProductId);
                    var currentPrimary = images.FirstOrDefault(i => i.IsPrimary && i.Id != dto.ImageId);
                    if (currentPrimary != null)
                    {
                        currentPrimary.SetPrimary(false);
                        await _productRepository.UpdateProductImageAsync(currentPrimary);
                    }
                    image.SetPrimary(true);
                }

                if (dto.SortOrder.HasValue)
                {
                    image.SetSortOrder(dto.SortOrder.Value);
                }

                await _productRepository.UpdateProductImageAsync(image);

                var product = await _productRepository.GetProductByIdAsync(image.ProductId);
                var summary = _mapper.Map<ProductSummaryDto>(product);
                await _elasticService.IndexProductAsync(summary);

                await transaction.CommitAsync();

                _logger.LogInformation("Updated image {ImageId}", dto.ImageId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update image {ImageId}", dto.ImageId);
                await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task<List<ProductImageDto>> ReorderProductImagesAsync(ReorderImagesDto dto)
        {
            var images = await _productRepository.GetProductImagesAsync(dto.ProductId);

            if (images.Count != dto.ImageIds.Count || !images.All(i => dto.ImageIds.Contains(i.Id)))
            {
                throw new InvalidOperationException("Invalid image IDs provided");
            }

            await using var transaction = await _productRepository.BeginTransactionAsync();

            try
            {
                for (int i = 0; i < dto.ImageIds.Count; i++)
                {
                    var image = images.First(img => img.Id == dto.ImageIds[i]);
                    image.SetSortOrder(i);
                    await _productRepository.UpdateProductImageAsync(image);
                }

                var product = await _productRepository.GetProductByIdAsync(dto.ProductId);
                var summary = _mapper.Map<ProductSummaryDto>(product);
                await _elasticService.IndexProductAsync(summary);

                await transaction.CommitAsync();

                _logger.LogInformation("Reordered images for product {ProductId}", dto.ProductId);

                var updatedImages = await _productRepository.GetProductImagesAsync(dto.ProductId);
                return updatedImages.Select(i => new ProductImageDto
                {
                    Id = i.Id,
                    ProductId = i.ProductId,
                    ImageUrl = i.ImageUrl,
                    IsPrimary = i.IsPrimary,
                    SortOrder = i.SortOrder
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to reorder images for product {ProductId}", dto.ProductId);
                await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task<ProductVariantDetailDto> AddProductVariantAsync(AddProductVariantDto dto)
        {
            var product = await _productRepository.GetProductByIdAsync(dto.ProductId);
            if (product == null)
                throw new NotFoundException("Product", dto.ProductId);

            if (await _productRepository.IsSkuExistsAsync(dto.Sku))
                throw new ConflictException($"SKU '{dto.Sku}' already exists", "SKU_EXISTS");

            // Validate and get/create attribute values
            var attributeValueIds = new Dictionary<int, int>();
            
            foreach (var kvp in dto.AttributeValues)
            {
                var attributeId = kvp.Key;
                var valueString = kvp.Value;

                if (string.IsNullOrWhiteSpace(valueString))
                    throw new InvalidOperationException($"Attribute value cannot be empty for AttributeId={attributeId}");

                // Get or create the attribute value
                var attributeValue = await _productRepository.GetOrCreateAttributeValueAsync(
                    attributeId, 
                    valueString, 
                    product.CategoryId);

                if (attributeValue == null)
                    throw new InvalidOperationException($"Failed to get or create attribute value: AttributeId={attributeId}, Value={valueString}");

                attributeValueIds.Add(attributeId, attributeValue.Id);
            }

            await using var transaction = await _productRepository.BeginTransactionAsync();

            try
            {
                var variant = new ProductVariant(dto.ProductId, dto.Sku, dto.Price, dto.Stock, dto.Status);

                // Add attributes using the resolved value IDs
                foreach (var kvp in attributeValueIds)
                {
                    variant.AddAttribute(kvp.Key, kvp.Value);
                }

                var created = await _productRepository.AddProductVariantAsync(variant);

                await UpdateProductSpecsFromVariantsAsync(dto.ProductId);

                var updatedProduct = await _productRepository.GetProductByIdAsync(dto.ProductId);
                var summary = _mapper.Map<ProductSummaryDto>(updatedProduct);
                await _elasticService.IndexProductAsync(summary);

                await transaction.CommitAsync();

                _logger.LogInformation("Added variant {Sku} to product {ProductId}", dto.Sku, dto.ProductId);
                var variantWithDetails = await _productRepository.GetProductVariantByIdAsync(created.Id);
                return _mapper.Map<ProductVariantDetailDto>(variantWithDetails);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add variant to product {ProductId}", dto.ProductId);
                await transaction.RollbackAsync();
                throw;
            }
        }
        private async Task UpdateProductSpecsFromVariantsAsync(int productId)
        {
            var product = await _productRepository.GetProductByIdAsync(productId);
            if (product == null) return;

            var specs = new Dictionary<string, HashSet<string>>();

            foreach (var variant in product.Variants)
            {
                foreach (var variantAttr in variant.VariantAttributes)
                {
                    var attrName = variantAttr.Attribute?.Name;
                    var attrValue = variantAttr.Value?.Value;

                    if (string.IsNullOrEmpty(attrName) || string.IsNullOrEmpty(attrValue)) continue;

                    if (!specs.ContainsKey(attrName))
                        specs[attrName] = new HashSet<string>();

                    specs[attrName].Add(attrValue);
                }
            }

            var specsList = specs.Select(kvp => new ProductAttributeDto
            {
                Name = kvp.Key,
                Value = kvp.Value.OrderBy(v => v).ToList()
            }).ToList();

            var specsJson = System.Text.Json.JsonSerializer.Serialize(specsList);
            product.UpdateSpecs(specsJson);

            await _productRepository.UpdateProductAsync(product);
        }

        public async Task<bool> DeleteProductVariantAsync(int variantId)
        {
            var variant = await _productRepository.GetProductVariantByIdAsync(variantId);
            if (variant == null)
            {
                _logger.LogWarning("Delete failed: Variant {VariantId} not found", variantId);
                return false;
            }

            var productId = variant.ProductId;

            await using var transaction = await _productRepository.BeginTransactionAsync();

            try
            {
                var deleted = await _productRepository.DeleteProductVariantAsync(variantId);
                if (!deleted)
                {
                    await transaction.RollbackAsync();
                    return false;
                }
                await UpdateProductSpecsFromVariantsAsync(productId);
                var product = await _productRepository.GetProductByIdAsync(productId);
                var summary = _mapper.Map<ProductSummaryDto>(product);
                await _elasticService.IndexProductAsync(summary);

                await transaction.CommitAsync();

                _logger.LogInformation("Deleted variant {VariantId}", variantId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete variant {VariantId}", variantId);
                await transaction.RollbackAsync();
                throw;
            }
        }
        private async Task EnrichWithRatingDataAsync(List<ProductSummaryDto> products)
        {
            if (products == null || !products.Any())
                return;

            var productIds = products.Select(p => p.Id).ToList();
            var ratingStats = await _productRepository.GetProductsRatingStatsAsync(productIds);

            foreach (var product in products)
            {
                if (ratingStats.TryGetValue(product.Id, out var stats))
                {
                    product.AverageRating = stats.AverageRating;
                    product.TotalReviews = stats.TotalReviews;
                }
                else
                {
                    product.AverageRating = 0;
                    product.TotalReviews = 0;
                }
            }
        }
        private async Task InvalidateNewProductsCacheAsync(int categoryId)
        {
            // Get product to find its category slug
            var product = await _productRepository.GetProductByIdAsync(categoryId);
            if (product?.Category != null)
            {
                var categorySlug = product.Category.Slug;

                // Invalidate cache for common count values (5, 10, 20, 50, 100)
                var commonCounts = new[] { 5, 10, 20, 50, 100 };
                foreach (var count in commonCounts)
                {
                    var cacheKey = CachePolicies.NewProductsKey(categorySlug, count);
                    await _cacheService.RemoveAsync(cacheKey);
                }

                _logger.LogInformation("Invalidated new products cache for category {CategorySlug}", categorySlug);
            }
        }
        private void InvalidateSearchCache()
        {
            _logger.LogInformation("Product changed - search cache will expire naturally in {Minutes} minutes", CachePolicies.SearchTtl.TotalMinutes);
        }
        public async Task<List<ProductSummaryDto>> GetTopNewProductsByCategoryAsync(string categorySlug, int count = 10)
        {
            if (count <= 0 || count > 100)
            {
                count = 10;
            }

            var cacheKey = CachePolicies.NewProductsKey(categorySlug, count);
            var cached = await _cacheService.GetAsync<List<ProductSummaryDto>>(cacheKey);
            if (cached != null)
            {
                _logger.LogInformation("Retrieved {Count} newest products for category {CategorySlug} from cache", cached.Count, categorySlug);
                return cached;
            }

            var products = await _productRepository.GetTopNewProductsByCategoryAsync(categorySlug, count);
            var dtos = _mapper.Map<List<ProductSummaryDto>>(products);

            await EnrichWithRatingDataAsync(dtos);

            await _cacheService.SetAsync(cacheKey, dtos, CachePolicies.NewProductsTtl);

            _logger.LogInformation("Retrieved {Count} newest products for category {CategorySlug} from database and cached", dtos.Count, categorySlug);

            return dtos;
        }

    }
}





