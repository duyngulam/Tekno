using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Nest;
using Tekno.Application.Catalog.DTOs.Products;
using Tekno.Application.Catalog.Interface;
using Tekno.Application.Common.Paging;

namespace Tekno.Infrastructure.Search
{
    public class ElasticProductService : IElasticProductService
    {
        private readonly IElasticClient _client;
        private readonly IProductRepository _productRepository;
        private const string IndexName = "products";

        public ElasticProductService(IElasticClient client, IProductRepository productRepository)
        {
            _client = client;
            _productRepository = productRepository;
        }

        public async Task IndexProductAsync(ProductSummaryDto product)
        {
            // normalize keyword fields
            var slug = (product.Slug ?? string.Empty).Trim().ToLowerInvariant();
            var brand = (product.BrandName ?? string.Empty).Trim().ToLowerInvariant();
            var category = (product.CategoryName ?? string.Empty).Trim().ToLowerInvariant();

            var doc = new ProductSearchDocument
            {
                Id = product.Id,
                Name = product.Name ?? string.Empty,
                Slug = slug,
                Brand = brand,
                Category = category,
                Price = product.FinalPrice,
                ImageUrl = product.PrimaryImagePath ?? string.Empty,
                DiscountPercent = product.DiscountPercent.HasValue ? (int)product.DiscountPercent.Value : 0,
                Specs = new List<ProductAttributeDto>()
            };

            try
            {
                var fullProduct = await _productRepository.GetProductBySlugAsync(product.Slug);
                var specsJson = fullProduct?.Detail?.Specs;

                if (!string.IsNullOrWhiteSpace(specsJson))
                {
                    // Deserialize as Dictionary<string, string[]> (array of values)
                    try
                    {
                        var dict = JsonSerializer.Deserialize<Dictionary<string, string[]>>(specsJson);
                        if (dict != null)
                        {
                            foreach (var kv in dict)
                            {
                                var name = kv.Key?.Trim().ToLowerInvariant() ?? string.Empty;
                                var values = kv.Value?.Where(v => !string.IsNullOrWhiteSpace(v))
                                                      .Select(v => v.Trim().ToLowerInvariant())
                                                      .ToList() ?? new List<string>();

                                if (!string.IsNullOrEmpty(name) && values.Any())
                                {
                                    doc.Specs.Add(new ProductAttributeDto
                                    {
                                        Name = name,
                                        Value = values
                                    });
                                }
                            }
                        }
                    }
                    catch (JsonException)
                    {
                        // fallback: single string values
                        var dict2 = JsonSerializer.Deserialize<Dictionary<string, string>>(specsJson);
                        if (dict2 != null)
                        {
                            foreach (var kv in dict2)
                            {
                                var name = kv.Key?.Trim().ToLowerInvariant() ?? string.Empty;
                                var value = kv.Value?.Trim().ToLowerInvariant() ?? string.Empty;
                                if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(value))
                                {
                                    doc.Specs.Add(new ProductAttributeDto
                                    {
                                        Name = name,
                                        Value = new List<string> { value }
                                    });
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                // ignore DB errors here
            }

            var indexResp = await _client.IndexAsync(doc, i => i.Index(IndexName).Id(doc.Id));
            if (!indexResp.IsValid)
            {
                throw new Exception($"Elasticsearch index error: {indexResp.ServerError?.ToString() ?? indexResp.OriginalException?.Message}");
            }
        }

        public async Task DeleteProductAsync(int id)
        {
            var resp = await _client.DeleteAsync<ProductSearchDocument>(id, d => d.Index(IndexName));
            if (!resp.IsValid && resp.Result != Result.NotFound)
            {
                throw new Exception($"Elasticsearch delete error: {resp.ServerError?.ToString() ?? resp.OriginalException?.Message}");
            }
        }

        public async Task<PagedResult<ProductSummaryDto>> SearchProductsAsync(
            string? keyword,
            string? categorySlug,
            string? brandSlug,
            Dictionary<string, string>? filters,
            decimal? minPrice,
            decimal? maxPrice,
            int page,
            int pageSize)
        {
            page = Math.Max(1, page);
            var from = (page - 1) * pageSize;

            var mustQueries = new List<QueryContainer>();
            var filterQueries = new List<QueryContainer>();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                mustQueries.Add(new MultiMatchQuery
                {
                    Fields = Infer.Fields<ProductSearchDocument>(p => p.Name),
                    Query = keyword,
                    Fuzziness = Fuzziness.Auto,
                    Operator = Operator.And
                });
            }

            if (!string.IsNullOrWhiteSpace(categorySlug))
                filterQueries.Add(new TermQuery { Field = Infer.Field<ProductSearchDocument>(p => p.Category), Value = categorySlug.ToLowerInvariant() });

            if (!string.IsNullOrWhiteSpace(brandSlug))
                filterQueries.Add(new TermQuery { Field = Infer.Field<ProductSearchDocument>(p => p.Brand), Value = brandSlug.ToLowerInvariant() });

            if (minPrice.HasValue || maxPrice.HasValue)
            {
                filterQueries.Add(new NumericRangeQuery
                {
                    Field = Infer.Field<ProductSearchDocument>(p => p.Price),
                    GreaterThanOrEqualTo = minPrice.HasValue ? (double?)minPrice.Value : null,
                    LessThanOrEqualTo = maxPrice.HasValue ? (double?)maxPrice.Value : null
                });
            }

            // Nested spec filters
            if (filters != null && filters.Any())
            {
                foreach (var kv in filters)
                {
                    var specName = (kv.Key ?? string.Empty).Trim().ToLowerInvariant();
                    var specValue = (kv.Value ?? string.Empty).Trim().ToLowerInvariant();

                    var nestedSpec = new NestedQuery
                    {
                        Path = "specs",
                        Query = new BoolQuery
                        {
                            Must = new QueryContainer[]
                            {
                                new TermQuery { Field = "specs.name", Value = specName },
                                new TermQuery { Field = "specs.value", Value = specValue }
                            }
                        }
                    };
                    filterQueries.Add(nestedSpec);
                }
            }

            QueryContainer finalQuery;
            if (mustQueries.Count == 0 && filterQueries.Count == 0)
                finalQuery = new MatchAllQuery();
            else
                finalQuery = new BoolQuery
                {
                    Must = mustQueries,
                    Filter = filterQueries
                };

            var resp = await _client.SearchAsync<ProductSearchDocument>(s => s
                .Index(IndexName)
                .From(from)
                .Size(pageSize)
                .TrackTotalHits()
                .Query(q => finalQuery)
            );

            if (!resp.IsValid)
                throw new Exception($"Elasticsearch search error: {resp.ServerError?.ToString() ?? resp.OriginalException?.Message}");

            var docs = resp.Hits.Select(h => h.Source!).ToList();

            var mapped = docs.Select(d => new ProductSummaryDto
            {
                Id = d.Id,
                Name = d.Name,
                Slug = d.Slug,
                BrandName = d.Brand,
                CategoryName = d.Category,
                BasePrice = d.Price,
                DiscountPercent = d.DiscountPercent
            }).ToList();

            var total = resp.Total > 0 ? (int)resp.Total : mapped.Count;
            return new PagedResult<ProductSummaryDto>(mapped, total, page, pageSize);
        }
    }
}
