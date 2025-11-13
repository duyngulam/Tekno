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
        private readonly IMapper _mapper;
        private readonly ILogger<ElasticBulkIndexer> _logger;
        private const string IndexName = "products";

        public ElasticBulkIndexer(
            IElasticClient client,
            IProductRepository productRepository,
            IMapper mapper,
            ILogger<ElasticBulkIndexer> logger)
        {
            _client = client;
            _productRepository = productRepository;
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
                var detail = product.Detail;
                var specsList = new List<ProductAttributeDto>();

                if (!string.IsNullOrEmpty(detail?.Specs))
                {
                    try
                    {
                        specsList = JsonSerializer.Deserialize<List<ProductAttributeDto>>(detail.Specs)
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

                docs.Add(new ProductSearchDocument
                {
                    Id = product.Id,
                    Name = product.Name,
                    Slug = product.Slug?.ToLowerInvariant() ?? string.Empty, 
                    Brand = product.Brand?.Name?.ToLowerInvariant() ?? string.Empty,
                    Category = product.Category?.Name?.ToLowerInvariant() ?? string.Empty,
                    Price = product.BasePrice,
                    DiscountPercent = (int)(product.DiscountPercent ?? 0),
                    ImageUrl = imageUrl,
                    Specs = specsList
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
