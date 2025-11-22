using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tekno.Application.Catalog.DTOs;
using Tekno.Application.Catalog.DTOs.Admin;
using Tekno.Application.Catalog.DTOs.Products;
using Tekno.Application.Catalog.Interface;
using Tekno.Application.Common;
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

        public ProductService(
            IProductRepository productRepository,
            IElasticProductService elasticService,
            IMapper mapper,
            MediaService mediaService,
            IAppLogger<ProductService> logger)
        {
            _productRepository = productRepository;
            _elasticService = elasticService;
            _mapper = mapper;
            _mediaService = mediaService;
            _logger = logger;
        }

        public async Task<PagedResult<ProductSummaryDto>> GetPagedProductAsync(ProductSearchRequestDto request)
        {
            var paging = new PagingParams(request.Page, request.PageSize);

            // Use ES when keyword or spec filters present
            if (!string.IsNullOrEmpty(request.Keyword) || (request.Filters != null && request.Filters.Any()))
            {
                return await _elasticService.SearchProductsAsync(
                    request.Keyword,
                    request.Category,
                    request.Brand,
                    request.Filters,
                    request.MinPrice,
                    request.MaxPrice,
                    request.Sort,
                    paging.Page,
                    paging.PageSize);
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

            await using var transaction = await _productRepository.BeginTransactionAsync();

            try
            {
                // Delete from database
                await _productRepository.DeleteProductAsync(product);

                // Delete from Elasticsearch
                await _elasticService.DeleteProductAsync(id);

                await transaction.CommitAsync();

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
    }
}
