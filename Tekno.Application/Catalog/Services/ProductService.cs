using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;
using Tekno.Application.Catalog.DTOs;
using Tekno.Application.Catalog.DTOs.Admin;
using Tekno.Application.Catalog.DTOs.Products;
using Tekno.Application.Catalog.Interface;
using Tekno.Application.Common;
using Tekno.Application.Common.Media.Services;
using Tekno.Application.Common.Paging;
using Tekno.Domain.Catalog;

namespace Tekno.Application.Catalog.Services
{
    public class ProductService
    {
        private readonly ILogger<ProductService> _logger;
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly IElasticProductService _elasticService;
        private readonly MediaService _mediaService;

        public ProductService(IProductRepository productRepository, IMapper mapper, IElasticProductService elasticService, ILogger<ProductService> logger, MediaService mediaService)
        {
            _productRepository = productRepository;
            _mapper = mapper;
            _elasticService = elasticService;
            _logger = logger;
            _mediaService = mediaService;
        }

        public async Task<PagedResult<ProductSummaryDto>> GetPagedProductAsync(ProductSearchRequestDto request)
        {
            var paging = new PagingParams(request.Page, request.PageSize);

            // Use ES when keyword or spec filters present (keeps previous behavior)
            if (!string.IsNullOrEmpty(request.Keyword) || (request.Filters != null && request.Filters.Any()))
            {
                var elasticResult = await _elasticService.SearchProductsAsync(
                    request.Keyword,
                    request.Category,
                    request.Brand,
                    request.Filters,
                    request.MinPrice,
                    request.MaxPrice,
                    request.Sort,
                    paging.Page,
                    paging.PageSize);

                return elasticResult;
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
            var product = await _productRepository.GetProductBySlugAsync(slug);
            if (product == null) return null;
            return _mapper.Map<ProductDetailDto>(product);
        }

        public async Task<CreateProductDto> CreateProductAsync(CreateProductDto dto)
        {
            var existingProduct = await _elasticService.IsProductExistBySlug(dto.Slug);
            if (existingProduct)
            {
                throw new Exception("Product with the same slug already exists.");
            }

            var uploadedImages = new List<string>();
            try
            {
                // Map DTO -> domain (Images handled separately)
                var newProduct = _mapper.Map<Product>(dto);

                // Upload images and add to entity
                int sort = 0;
                foreach (var file in dto.Images)
                {
                    var imageUrl = await _mediaService.UploadImageAsync(file, $"tekno/product/{dto.Slug}");
                    uploadedImages.Add(imageUrl);
                    newProduct.AddImage(imageUrl, isPrimary: sort == 0, sortOrder: sort++);
                }

                // Save to db
                newProduct = await _productRepository.AddProductAsync(newProduct);
                _logger.LogInformation("Created product {ProductName} with {ImageCount} images", dto.Name, uploadedImages.Count);

                // Index to Elastic (map domain -> summary DTO)
                var summary = _mapper.Map<ProductSummaryDto>(newProduct);
                _logger.LogInformation("Indexing product {ProductName} to ElasticSearch, with primary img {img}", summary.Name, summary.PrimaryImagePath);
                await _elasticService.IndexProductAsync(summary);

                return _mapper.Map<CreateProductDto>(newProduct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create product {Name}", dto.Name);

                // Delete uploaded images on failure
                foreach (var img in uploadedImages)
                    await _mediaService.DeleteImageAsync(img);
                throw;
            }
        }

        public async Task<ProductDetailDto?> UpdateProductAsync(int id, CreateProductDto dto)
        {
            var existing = await _productRepository.GetProductByIdAsync(id);
            if (existing == null) return null;

            // Update scalar properties
            existing.Name = dto.Name;
            existing.Slug = dto.Slug;
            existing.CategoryId = dto.CategoryId;
            existing.BrandId = dto.BrandId;
            existing.BasePrice = dto.BasePrice;
            existing.Description = dto.Description;
            existing.Overview = dto.Overview;
            existing.Status = dto.Status ?? existing.Status;
            existing.UpdatedAt = DateTime.UtcNow;

            // Upload any new images and add
            int sort = existing.Images?.Count ?? 0;
            foreach (var file in dto.Images)
            {
                var imageUrl = await _mediaService.UploadImageAsync(file, $"tekno/product/{dto.Slug}");
                existing.AddImage(imageUrl, isPrimary: sort == 0, sortOrder: sort++);
            }

            // Persist
            var updated = await _productRepository.UpdateProductAsync(existing);

            // Reindex updated document
            var summary = _mapper.Map<ProductSummaryDto>(updated);
            await _elasticService.IndexProductAsync(summary);

            return _mapper.Map<ProductDetailDto>(updated);
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var product = await _productRepository.GetProductByIdAsync(id);
            if (product == null) return false;

            // Delete images from media store
            foreach (var img in product.Images.Select(i => i.ImageUrl).ToList())
            {
                try { await _mediaService.DeleteImageAsync(img); } catch { /* loge */ }
            }

            await _productRepository.DeleteProductAsync(product);

            // Delete from elastic
            await _elasticService.DeleteProductAsync(id);

            return true;
        }
    }
}
