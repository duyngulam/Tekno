using AutoMapper;
using Microsoft.Extensions.Logging;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Tekno.Application.Catalog.DTOs.Products;
using Tekno.Application.Catalog.Interface;

namespace Tekno.Infrastructure.Search
{
    public class ElasticBulkIndexer
    {
        private readonly IElasticClient _client;
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ElasticBulkIndexer> _logger;
        private const string IndexName = "products";

        public ElasticBulkIndexer(
            IElasticClient client,
            IProductRepository productRepository,
            ICategoryRepository categoryRepository,
            IMapper mapper,
            ILogger<ElasticBulkIndexer> logger)
        {
            _client = client;
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task RunAsync()
        {
            _logger.LogInformation("🚀 [Elastic] Bulk indexing started...");

            var exists = await _client.Indices.ExistsAsync(IndexName);
            if (!exists.Exists)
            {
                _logger.LogWarning("⚠️ Index '{IndexName}' not found. Creating...", IndexName);
                ElasticMappings.CreateProductIndex(_client);
                ElasticMappings.CreateProductDetailIndex(_client);
            }

            var allProducts = await _productRepository.GetAllProductsWithDetailAsync();
            if (allProducts == null || !allProducts.Any())
            {
                _logger.LogWarning("⚠️ No products found in database. Skipping bulk indexing.");
                return;
            }

            var docs = new List<ProductSearchDocument>();
            foreach (var product in allProducts)
            {
                var specsList = new List<ProductAttributeDto>();

                if (!string.IsNullOrEmpty(product?.Specs))
                {
                    try
                    {
                        specsList = JsonSerializer.Deserialize<List<ProductAttributeDto>>(product.Specs)
                                     ?? new List<ProductAttributeDto>();

                        // normalize name/value
                        foreach (var spec in specsList)
                        {
                            spec.Name = spec.Name.Trim().ToLowerInvariant();
                            spec.Value = spec.Value.Select(v => v.Trim().ToLowerInvariant()).ToList();
                        }
                    }
                    catch (JsonException)
                    {
                        // fallback nếu JSON lỗi
                        specsList = new List<ProductAttributeDto>();
                    }
                }
                // Lấy imageUrl từ product.Images nếu có
                var imageUrl = product.Images?.FirstOrDefault()?.ImageUrl ?? string.Empty;

                // Build categories list (category + ancestors)
                var categories = new List<string>();
                try
                {
                    if (product.Category != null)
                    {
                        var catId = product.Category.Id;
                        // walk up to parents
                        var visited = new HashSet<int>();
                        while (catId != 0 && !visited.Contains(catId))
                        {
                            visited.Add(catId);
                            var cat = await _categoryRepository.GetCategoryByIdAsync(catId);
                            if (cat == null) break;
                            if (!string.IsNullOrWhiteSpace(cat.Slug))
                                categories.Add(cat.Slug.Trim().ToLowerInvariant());

                            if (cat.ParentId == null) break;
                            catId = cat.ParentId.Value;
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to build category ancestry for product {ProductId}", product.Id);
                }

                docs.Add(new ProductSearchDocument
                {
                    Id = product.Id,
                    Name = product.Name,
                    Slug = product.Slug?.ToLowerInvariant() ?? string.Empty,
                    Brand = product.Brand?.Name?.ToLowerInvariant() ?? string.Empty,
                    Category = product.Category?.Name?.ToLowerInvariant() ?? string.Empty,
                    Categories = categories.Distinct().ToList(),
                    Price = product.BasePrice,
                    DiscountPercent = (int)(product.DiscountPercent ?? 0),
                    ImageUrl = imageUrl,
                    Specs = specsList,
                    CreatedAt = product.CreatedAt
                });
            }

            // Bulk index
            var bulkResponse = await _client.BulkAsync(b => b
                .Index(IndexName)
                .IndexMany(docs)
                .Refresh(Elasticsearch.Net.Refresh.True) // namespace cần using Nest
            );

            if (bulkResponse.Errors)
            {
                foreach (var item in bulkResponse.ItemsWithErrors)
                {
                    _logger.LogError("❌ Failed to index doc {Id}: {Error}", item.Id, item.Error?.Reason);
                }

                throw new Exception("Some documents failed during bulk indexing.");
            }

            _logger.LogInformation("✅ [Elastic] Bulk indexing completed: {Count} products indexed.", docs.Count);
        }
    }
}
