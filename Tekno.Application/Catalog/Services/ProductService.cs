using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
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
        private readonly bool _useDbTriggerForSpecs;

        public ProductService(
            IProductRepository productRepository,
            IElasticProductService elasticService,
            IMapper mapper,
            MediaService mediaService,
            IAppLogger<ProductService> logger,
            ICacheService cacheService,
            IConfiguration configuration)
        {
            _productRepository = productRepository;
            _elasticService = elasticService;
            _mapper = mapper;
            _mediaService = mediaService;
            _logger = logger;
            _cacheService = cacheService;

            // Default false: app will manage product specs rebuild. Set Database:UseTriggerForProductSpecs = true to let DB trigger handle specs.
            _useDbTriggerForSpecs = false;
            try
            {
                var cfg = configuration["Database:UseTriggerForProductSpecs"];
                if (!string.IsNullOrEmpty(cfg) && bool.TryParse(cfg, out var parsed))
                {
                    _useDbTriggerForSpecs = parsed;
                }
            }
            catch
            {
                // ignore and use default
            }
        }

        // Backwards-compatible constructor (no IConfiguration) - assumes DB trigger is used
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

            // Default false: app will manage product specs rebuild. Set Database:UseTriggerForProductSpecs = true to let DB trigger handle specs.
            _useDbTriggerForSpecs = false;
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

            // Fallback to database - also cache these results
            var dbCacheKey = CachePolicies.SearchProductsKey(
                null, // no keyword
                request.Category,
                request.Brand,
                null, // no filters
                request.MinPrice,
                request.MaxPrice,
                request.Sort,
                paging.Page,
                paging.PageSize);

            // Try cache first
            var cachedDbResults = await _cacheService.GetAsync<PagedResult<ProductSummaryDto>>(dbCacheKey);
            if (cachedDbResults != null)
            {
                _logger.LogInformation("Retrieved database results from cache for category: {Category}, brand: {Brand}", 
                    request.Category, request.Brand);
                return cachedDbResults;
            }

            // Cache miss - query database
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

            var result = new PagedResult<ProductSummaryDto>(mapped, pagedResult.TotalRecords, paging.Page, paging.PageSize);

            // Cache the database results
            await _cacheService.SetAsync(dbCacheKey, result, CachePolicies.ProductListTtl);
            
            _logger.LogInformation("Database results cached for category: {Category}, brand: {Brand}, page: {Page}", 
                request.Category, request.Brand, paging.Page);

            return result;
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

            var dto = _mapper.Map<ProductDetailDto>(product);

            // Enrich with rating data for this product
            var stats = await _productRepository.GetProductsRatingStatsAsync(new List<int> { product.Id });
            if (stats != null && stats.TryGetValue(product.Id, out var s))
            {
                dto.Rating = (decimal?)Math.Round(s.AverageRating, 1);
            }

            return dto;
        }

        public async Task<ProductDetailDto?> GetProductDetailByIdAsync(int id)
        {
            var product = await _productRepository.GetProductByIdAsync(id);
            if (product == null)
            {
                _logger.LogInformation("Product not found with ID: {Id}", id);
                return null;
            }

            var dto = _mapper.Map<ProductDetailDto>(product);

            // Enrich with rating data for this product
            var stats = await _productRepository.GetProductsRatingStatsAsync(new List<int> { product.Id });
            if (stats != null && stats.TryGetValue(product.Id, out var s))
            {
                dto.Rating = (decimal?)Math.Round(s.AverageRating, 1);
            }

            return dto;
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

            // Validate attribute inputs
            foreach (var attr in dto.Attributes)
            {
                if (!attr.IsValid(out var errorMessage))
                {
                    throw new InvalidOperationException($"Invalid attribute input: {errorMessage}");
                }
            }

            // Load existing attributes for the category once to avoid repeated DB calls
            var existingAttributes = await _productRepository.GetAttributesByCategoryIdAsync(product.CategoryId);

            // Process attributes and get/create attribute values
            var attributeValueIds = new Dictionary<int, int>();
            
            foreach (var attrInput in dto.Attributes)
            {
                int attributeId;
                
                // If Name is provided, try to find existing attribute with the same name (case-insensitive)
                if (!string.IsNullOrWhiteSpace(attrInput.Name))
                {
                    var attrName = attrInput.Name.Trim();
                    var found = existingAttributes.FirstOrDefault(a => string.Equals(a.Name, attrName, StringComparison.OrdinalIgnoreCase));
                    if (found != null)
                    {
                        attributeId = found.Id;
                        _logger.LogInformation("Found existing attribute '{AttributeName}' (ID: {AttributeId}) for category {CategoryId}", attrName, attributeId, product.CategoryId);
                    }
                    else
                    {
                        _logger.LogInformation("Creating new attribute '{AttributeName}' for category {CategoryId}", 
                            attrName, product.CategoryId);

                        var newAttribute = new ProductAttribute(
                            name: attrName,
                            inputType: "select", // Default to select for variant attributes
                            isGlobal: false,
                            categoryId: product.CategoryId);

                        var createdAttribute = await _productRepository.CreateAttributeAsync(newAttribute);
                        attributeId = createdAttribute.Id;
                        existingAttributes.Add(createdAttribute); // cache for subsequent iterations
                        
                        _logger.LogInformation("Created new attribute '{AttributeName}' with ID {AttributeId}", 
                            attrName, attributeId);
                    }
                }
                // If Id is provided, use existing attribute
                else if (attrInput.Id.HasValue)
                {
                    attributeId = attrInput.Id.Value;
                    
                    // Validate attribute exists and belongs to category
                    var existingAttribute = await _productRepository.GetAttributeByIdAsync(attributeId);
                    if (existingAttribute == null)
                    {
                        throw new NotFoundException("Attribute", attributeId);
                    }

                    if (!existingAttribute.IsGlobal && existingAttribute.CategoryId != product.CategoryId)
                    {
                        throw new InvalidOperationException(
                            $"Attribute '{existingAttribute.Name}' (ID: {attributeId}) does not belong to product's category");
                    }
                }
                else
                {
                    throw new InvalidOperationException("Either attribute Id or Name must be provided");
                }

                // Get or create the attribute value
                var attributeValue = await _productRepository.GetOrCreateAttributeValueAsync(
                    attributeId, 
                    attrInput.Value, 
                    product.CategoryId);

                if (attributeValue == null)
                {
                    throw new InvalidOperationException(
                        $"Failed to get or create attribute value: AttributeId={attributeId}, Value={attrInput.Value}");
                }

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

                if (_useDbTriggerForSpecs)
                {
                    // Commit so DB trigger runs and writes specs; then re-query and index
                    await transaction.CommitAsync();

                    var updatedProduct = await _productRepository.GetProductByIdAsync(dto.ProductId);
                    var summary = _mapper.Map<ProductSummaryDto>(updatedProduct);
                    await _elasticService.IndexProductAsync(summary);

                    _logger.LogInformation("Added variant {Sku} to product {ProductId} with {AttributeCount} attributes (DB trigger used)", 
                        dto.Sku, dto.ProductId, attributeValueIds.Count);

                    var variantWithDetails = await _productRepository.GetProductVariantByIdAsync(created.Id);
                    return _mapper.Map<ProductVariantDetailDto>(variantWithDetails);
                }

                // App-side rebuild
                await UpdateProductSpecsFromVariantsAsync(dto.ProductId);

                var updatedProductApp = await _productRepository.GetProductByIdAsync(dto.ProductId);
                var summaryApp = _mapper.Map<ProductSummaryDto>(updatedProductApp);
                await _elasticService.IndexProductAsync(summaryApp);

                await transaction.CommitAsync();

                _logger.LogInformation("Added variant {Sku} to product {ProductId} with {AttributeCount} attributes", 
                    dto.Sku, dto.ProductId, attributeValueIds.Count);
                
                var variantWithDetailsApp = await _productRepository.GetProductVariantByIdAsync(created.Id);
                return _mapper.Map<ProductVariantDetailDto>(variantWithDetailsApp);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add variant to product {ProductId}", dto.ProductId);
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<ProductVariantDetailDto?> UpdateProductVariantAsync(int variantId, UpdateProductVariantDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            var variant = await _productRepository.GetProductVariantByIdAsync(variantId);
            if (variant == null)
            {
                _logger.LogWarning("Update failed: Variant with ID {Id} not found", variantId);
                return null;
            }

            var product = await _productRepository.GetProductByIdAsync(variant.ProductId);
            if (product == null) throw new NotFoundException("Product", variant.ProductId);

            // Validate attribute inputs
            foreach (var attr in dto.Attributes)
            {
                if (!attr.IsValid(out var errorMessage))
                {
                    throw new InvalidOperationException($"Invalid attribute input: {errorMessage}");
                }
            }

            // Load existing attributes for the category once
            var existingAttributes = await _productRepository.GetAttributesByCategoryIdAsync(product.CategoryId);

            await using var transaction = await _productRepository.BeginTransactionAsync();

            try
            {
                // SKU uniqueness check if changed
                if (!string.Equals(variant.Sku, dto.Sku, StringComparison.OrdinalIgnoreCase))
                {
                    if (await _productRepository.IsSkuExistsAsync(dto.Sku))
                        throw new ConflictException($"SKU '{dto.Sku}' already exists", "SKU_EXISTS");

                    variant.UpdateSku(dto.Sku);
                }

                // Update price/stock/status
                variant.UpdatePrice(dto.Price);
                variant.UpdateStock(dto.Stock);
                variant.UpdateStatus(dto.Status);

                // Clear existing variant attributes
                variant.VariantAttributes.Clear();

                var attributeValueIds = new Dictionary<int, int>();
                foreach (var attrInput in dto.Attributes)
                {
                    int attributeId;

                    if (!string.IsNullOrWhiteSpace(attrInput.Name))
                    {
                        var attrName = attrInput.Name.Trim();
                        var found = existingAttributes.FirstOrDefault(a => string.Equals(a.Name, attrName, StringComparison.OrdinalIgnoreCase));
                        if (found != null)
                        {
                            attributeId = found.Id;
                            _logger.LogInformation("Found existing attribute '{AttributeName}' (ID: {AttributeId}) for category {CategoryId}", attrName, attributeId, product.CategoryId);
                        }
                        else
                        {
                            var newAttribute = new ProductAttribute(
                                name: attrName,
                                inputType: "select",
                                isGlobal: false,
                                categoryId: product.CategoryId);

                            var createdAttribute = await _productRepository.CreateAttributeAsync(newAttribute);
                            attributeId = createdAttribute.Id;
                            existingAttributes.Add(createdAttribute);
                        }
                    }
                    else if (attrInput.Id.HasValue)
                    {
                        attributeId = attrInput.Id.Value;
                        var existingAttribute = await _productRepository.GetAttributeByIdAsync(attributeId);
                        if (existingAttribute == null)
                            throw new NotFoundException("Attribute", attributeId);

                        if (!existingAttribute.IsGlobal && existingAttribute.CategoryId != product.CategoryId)
                            throw new InvalidOperationException($"Attribute '{existingAttribute.Name}' (ID: {attributeId}) does not belong to product's category");
                    }
                    else
                    {
                        throw new InvalidOperationException("Either attribute Id or Name must be provided");
                    }

                    var attributeValue = await _productRepository.GetOrCreateAttributeValueAsync(
                        attributeId,
                        attrInput.Value,
                        product.CategoryId);

                    if (attributeValue == null)
                        throw new InvalidOperationException($"Failed to get or create attribute value: AttributeId={attributeId}, Value={attrInput.Value}");

                    attributeValueIds.Add(attributeId, attributeValue.Id);
                }

                // Recreate variant attribute links
                foreach (var kvp in attributeValueIds)
                {
                    variant.AddAttribute(kvp.Key, kvp.Value);
                }

                // Persist variant update (repository should handle update)
                var updatedVariant = await _productRepository.UpdateProductVariantAsync(variant);

                if (_useDbTriggerForSpecs)
                {
                    await transaction.CommitAsync();

                    var updatedProduct = await _productRepository.GetProductByIdAsync(variant.ProductId);
                    var summary = _mapper.Map<ProductSummaryDto>(updatedProduct);
                    await _elasticService.IndexProductAsync(summary);

                    _logger.LogInformation("Updated variant {VariantId} for product {ProductId} (DB trigger used)", variantId, variant.ProductId);

                    var variantWithDetails = await _productRepository.GetProductVariantByIdAsync(updatedVariant.Id);
                    return _mapper.Map<ProductVariantDetailDto>(variantWithDetails);
                }

                // App-side rebuild and index
                await UpdateProductSpecsFromVariantsAsync(variant.ProductId);
                var updatedProductApp = await _productRepository.GetProductByIdAsync(variant.ProductId);
                var summaryApp = _mapper.Map<ProductSummaryDto>(updatedProductApp);
                await _elasticService.IndexProductAsync(summaryApp);

                await transaction.CommitAsync();

                _logger.LogInformation("Updated variant {VariantId} for product {ProductId}", variantId, variant.ProductId);

                var variantWithDetailsApp = await _productRepository.GetProductVariantByIdAsync(updatedVariant.Id);
                return _mapper.Map<ProductVariantDetailDto>(variantWithDetailsApp);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update variant {VariantId}", variantId);
                await transaction.RollbackAsync();
                throw;
            }
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

                if (_useDbTriggerForSpecs)
                {
                    await transaction.CommitAsync();

                    var product = await _productRepository.GetProductByIdAsync(productId);
                    var summary = _mapper.Map<ProductSummaryDto>(product);
                    await _elasticService.IndexProductAsync(summary);

                    _logger.LogInformation("Deleted variant {VariantId} (DB trigger used)", variantId);
                    return true;
                }

                await UpdateProductSpecsFromVariantsAsync(productId);
                var productApp = await _productRepository.GetProductByIdAsync(productId);
                var summaryApp = _mapper.Map<ProductSummaryDto>(productApp);
                await _elasticService.IndexProductAsync(summaryApp);

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
            // For now, we let cache expire naturally to avoid complex invalidation logic
            // In a production system, you could use Redis SCAN to find and delete matching keys
            // or use cache tags/groups for bulk invalidation
            
            // Example pattern: cache:search:products:*
            // This would require ICacheService to support pattern-based deletion
            
            _logger.LogInformation("Product changed - search cache will expire naturally in {Minutes} minutes", 
                CachePolicies.SearchTtl.TotalMinutes);
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

        /// <summary>
        /// Get products on sale (with active discounts)
        /// </summary>
        /// <param name="categorySlug">Optional category filter</param>
        /// <param name="count">Number of products to return</param>
        /// <returns>List of products on sale</returns>
        public async Task<List<ProductSummaryDto>> GetProductsOnSaleAsync(string? categorySlug = null, int count = 20)
        {
            var cacheKey = $"products:on-sale:{categorySlug ?? "all"}:{count}";
            
            return await _cacheService.CacheOrGetAsync(cacheKey, async () =>
            {
                _logger.LogInformation("Fetching products on sale from database");
                
                var products = await _productRepository.GetProductsWithDiscountAsync(categorySlug, count);
                var productDtos = _mapper.Map<List<ProductSummaryDto>>(products);
                
                // Enrich with rating data
                await EnrichWithRatingDataAsync(productDtos);
                
                return productDtos;
            }, TimeSpan.FromMinutes(15));
        }

        private async Task UpdateProductSpecsFromVariantsAsync(int productId)
        {
            if (_useDbTriggerForSpecs)
            {
                _logger.LogInformation("Skipping UpdateProductSpecsFromVariantsAsync because DB trigger handles specs");
                return;
            }

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
            // reuse the already fetched product instance
            product.UpdateSpecs(specsJson);

            await _productRepository.UpdateProductAsync(product);
        }

        public async Task<ProductSummaryDto?> GetProductSummaryByIdAsync(int id)
        {
            if (id <= 0) return null;

            var product = await _productRepository.GetProductByIdAsync(id);
            if (product == null) return null;

            var dto = _mapper.Map<ProductSummaryDto>(product);

            // Enrich rating info
            await EnrichWithRatingDataAsync(new List<ProductSummaryDto> { dto });

            return dto;
        }

        public async Task<List<ProductSummaryDto>> GetProductSummariesByIdsAsync(List<int> ids)
        {
            if (ids == null || !ids.Any()) return new List<ProductSummaryDto>();

            // Load products in parallel
            var tasks = ids.Select(i => _productRepository.GetProductByIdAsync(i)).ToList();
            await Task.WhenAll(tasks);

            var products = tasks.Select(t => t.Result).Where(p => p != null).ToList()!;

            var dtos = _mapper.Map<List<ProductSummaryDto>>(products);

            await EnrichWithRatingDataAsync(dtos);

            return dtos;
        }
    }
}