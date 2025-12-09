using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Nest;
using Tekno.Application.Catalog.DTOs.Products;
using Tekno.Application.Catalog.Interface;
using Tekno.Application.Common.Paging;
using Tekno.Domain.Catalog;

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
                Specs = new List<ProductAttributeDto>(),
                CreatedAt = product.CreatedAt
            };

            try
            {
                var fullProduct = await _productRepository.GetProductBySlugAsync(product.Slug);
                // set created date from DB product (fallback to UtcNow)
                doc.CreatedAt = fullProduct?.CreatedAt ?? DateTime.UtcNow;

                var specsJson = fullProduct?.Specs;

                bool fallbackParse = false;

                if (!string.IsNullOrWhiteSpace(specsJson))
                {
                    // try deserialize to list of ProductAttributeDto (Name + Value[])
                    try
                    {
                        var list = JsonSerializer.Deserialize<List<ProductAttributeDto>>(specsJson);
                        if (list != null && list.Any())
                        {
                            foreach (var item in list)
                            {
                                var name = (item.Name ?? string.Empty).Trim().ToLowerInvariant();
                                var values = (item.Value ?? new List<string>()).Where(v => !string.IsNullOrWhiteSpace(v))
                                                                          .Select(v => v.Trim().ToLowerInvariant())
                                                                          .Distinct()
                                                                          .ToList();
                                if (!string.IsNullOrEmpty(name) && values.Any())
                                {
                                    doc.Specs.Add(new ProductAttributeDto { Name = name, Value = values });
                                }
                            }
                        }
                        else
                        {
                            // fallback to other shapes below
                            fallbackParse = true;
                        }
                    }
                    catch (JsonException)
                    {
                        // not array-of-objects -> try other shapes
                        fallbackParse = true;
                    }

                    if (fallbackParse)
                    {
                        try
                        {
                            // try dictionary<string,string[]>
                            var dictArr = JsonSerializer.Deserialize<Dictionary<string, string[]>>(specsJson);
                            if (dictArr != null && dictArr.Any())
                            {
                                foreach (var kv in dictArr)
                                {
                                    var name = (kv.Key ?? string.Empty).Trim().ToLowerInvariant();
                                    var values = (kv.Value ?? Array.Empty<string>()).Where(v => !string.IsNullOrWhiteSpace(v))
                                                                               .Select(v => v.Trim().ToLowerInvariant())
                                                                               .Distinct()
                                                                               .ToList();
                                    if (!string.IsNullOrEmpty(name) && values.Any())
                                    {
                                        doc.Specs.Add(new ProductAttributeDto { Name = name, Value = values });
                                    }
                                }
                            }
                            else
                            {
                                // try dictionary<string,string>
                                var dict = JsonSerializer.Deserialize<Dictionary<string, string>>(specsJson);
                                if (dict != null && dict.Any())
                                {
                                    foreach (var kv in dict)
                                    {
                                        var name = (kv.Key ?? string.Empty).Trim().ToLowerInvariant();
                                        var value = (kv.Value ?? string.Empty).Trim().ToLowerInvariant();
                                        if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(value))
                                        {
                                            doc.Specs.Add(new ProductAttributeDto { Name = name, Value = new List<string> { value } });
                                        }
                                    }
                                }
                            }
                        }
                        catch
                        {
                            // ignore parse errors; will fallback to attribute relation below if needed
                        }
                    }
                }

                // 2) Fallback: if no JSON specs, build specs from Product -> Variants -> VariantAttributes -> Attribute & Value
                if ((doc.Specs == null) || !doc.Specs.Any())
                {
                    if (fullProduct != null)
                    {
                        var map = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);

                        // collect attribute values from variants
                        if (fullProduct.Variants != null)
                        {
                            foreach (var variant in fullProduct.Variants)
                            {
                                if (variant.VariantAttributes == null) continue;
                                foreach (var va in variant.VariantAttributes)
                                {
                                    var attrName = va.Attribute?.Name?.Trim();
                                    var val = va.Value?.Value?.Trim(); // AttributeValue.Value property name assumed 'Value'
                                    if (string.IsNullOrEmpty(attrName) || string.IsNullOrEmpty(val)) continue;

                                    var key = attrName.ToLowerInvariant();
                                    var v = val.ToLowerInvariant();

                                    if (!map.TryGetValue(key, out var set))
                                    {
                                        set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                                        map[key] = set;
                                    }
                                    set.Add(v);
                                }
                            }
                        }

                        // map -> doc.Specs
                        foreach (var kv in map)
                        {
                            doc.Specs.Add(new ProductAttributeDto
                            {
                                Name = kv.Key.Trim().ToLowerInvariant(),
                                Value = kv.Value.Select(x => x.Trim().ToLowerInvariant()).Distinct().ToList()
                            });
                        }
                    }
                }
            }
            catch
            {
                // ignore DB errors here (optionally log)
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
        public async Task<bool> IsProductExistBySlug(string slug)
        { 
            var normalizedSlug = (slug ?? string.Empty).Trim().ToLowerInvariant();

            if (string.IsNullOrEmpty(normalizedSlug))
            {
                return false;
            }

            var resp = await _client.CountAsync<ProductSearchDocument>(c => c
                .Index(IndexName)
                .Query(q => q
                    .Term(t => t.Field(p => p.Slug).Value(normalizedSlug))
                )
            );

            if (!resp.IsValid)
            {
                return false;
            }
            return resp.Count > 0;
        }

        public async Task<PagedResult<ProductSummaryDto>> SearchProductsAsync(
            string? keyword,
            string? categorySlug,
            string? brandSlug,
            Dictionary<string, string>? filters,
            decimal? minPrice,
            decimal? maxPrice,
            string? sort,
            int page,
            int pageSize)
        {
            page = Math.Max(1, page);
            var from = (page - 1) * pageSize;

            var mustQueries = new List<QueryContainer>();
            var filterQueries = new List<QueryContainer>();

            // Enhanced keyword search with partial matching support
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var trimmedKeyword = keyword.Trim();
                
                // Multi-field search with different strategies for better matching
                mustQueries.Add(new BoolQuery
                {
                    Should = new List<QueryContainer>
                    {
                        // 1. Exact phrase match (highest priority)
                        new MatchPhraseQuery
                        {
                            Field = Infer.Field<ProductSearchDocument>(p => p.Name),
                            Query = trimmedKeyword,
                            Boost = 5.0
                        },
                        // 2. Standard match with fuzziness (handles typos)
                        new MatchQuery
                        {
                            Field = Infer.Field<ProductSearchDocument>(p => p.Name),
                            Query = trimmedKeyword,
                            Fuzziness = Fuzziness.Auto,
                            Boost = 3.0
                        },
                        // 3. N-gram match for partial matching (e.g., "mac" matches "macbook")
                        new MatchQuery
                        {
                            Field = "name.ngram",
                            Query = trimmedKeyword,
                            Boost = 2.0
                        },
                        // 4. Edge n-gram for prefix matching (autocomplete-style)
                        new MatchQuery
                        {
                            Field = "name.edge",
                            Query = trimmedKeyword,
                            Boost = 1.5
                        },
                        // 5. Wildcard search as fallback
                        new WildcardQuery
                        {
                            Field = Infer.Field<ProductSearchDocument>(p => p.Name),
                            Value = $"*{trimmedKeyword.ToLowerInvariant()}*",
                            Boost = 1.0
                        }
                    },
                    MinimumShouldMatch = 1
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

            // Enhanced nested spec filters with multi-value support (UNION/OR logic)
            // Example: filters[ram]=8gb,16gb will match products with RAM 8GB OR 16GB
            if (filters != null && filters.Any())
            {
                foreach (var kv in filters)
                {
                    var specName = (kv.Key ?? string.Empty).Trim().ToLowerInvariant();
                    var specValueRaw = (kv.Value ?? string.Empty).Trim();

                    if (string.IsNullOrEmpty(specName) || string.IsNullOrEmpty(specValueRaw))
                        continue;

                    // Split by comma to support multiple values: "8gb,16gb"
                    var specValues = specValueRaw.Split(new[] { ',', '|' }, StringSplitOptions.RemoveEmptyEntries)
                                                  .Select(v => v.Trim().ToLowerInvariant())
                                                  .Where(v => !string.IsNullOrEmpty(v))
                                                  .Distinct()
                                                  .ToList();

                    if (!specValues.Any())
                        continue;

                    if (specValues.Count == 1)
                    {
                        // Single value: use simple term query
                        var nestedSpec = new NestedQuery
                        {
                            Path = "specs",
                            Query = new BoolQuery
                            {
                                Must = new QueryContainer[]
                                {
                                    new TermQuery { Field = "specs.name", Value = specName },
                                    new TermQuery { Field = "specs.value", Value = specValues[0] }
                                }
                            }
                        };
                        filterQueries.Add(nestedSpec);
                    }
                    else
                    {
                        // Multiple values: use OR logic (UNION)
                        // Product must have this spec name AND at least one of the specified values
                        var valueQueries = specValues.Select(specValue => 
                            (QueryContainer)new TermQuery { Field = "specs.value", Value = specValue }
                        ).ToList();

                        var nestedSpec = new NestedQuery
                        {
                            Path = "specs",
                            Query = new BoolQuery
                            {
                                Must = new QueryContainer[]
                                {
                                    new TermQuery { Field = "specs.name", Value = specName }
                                },
                                Should = valueQueries,
                                MinimumShouldMatch = 1
                            }
                        };
                        filterQueries.Add(nestedSpec);
                    }
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

            // capture sort parts
            var sortField = "date";
            var sortDir = "desc";
            if (!string.IsNullOrWhiteSpace(sort))
            {
                var parts = sort.ToLowerInvariant().Split(new[] { '_', ':' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length > 0) sortField = parts[0];
                if (parts.Length > 1) sortDir = parts[1] == "asc" ? "asc" : "desc";
            }

            var resp = await _client.SearchAsync<ProductSearchDocument>(s => s
                .Index(IndexName)
                .From(from)
                .Size(pageSize)
                .TrackTotalHits()
                .Query(q => finalQuery)
                .Sort(sd =>
                {
                    // If no explicit sort and keyword exists, sort by relevance (_score)
                    if (string.IsNullOrWhiteSpace(sort))
                    {
                        if (!string.IsNullOrWhiteSpace(keyword))
                            return sd.Descending(SortSpecialField.Score);
                        // default: no sort (or you can choose price/created default)
                        return sd;
                    }

                    // map supported sort fields (price or date)
                    switch (sortField)
                    {
                        case "price":
                            return sortDir == "asc"
                                ? sd.Field(f => f.Field(p => p.Price).Order(SortOrder.Ascending))
                                : sd.Field(f => f.Field(p => p.Price).Order(SortOrder.Descending));
                        case "date":
                        case "created":
                        case "createdat":
                            return sortDir == "asc"
                                ? sd.Field(f => f.Field(p => p.CreatedAt).Order(SortOrder.Ascending))
                                : sd.Field(f => f.Field(p => p.CreatedAt).Order(SortOrder.Descending));
                        // keep other cases if needed
                        case "discount":
                        case "discountpercent":
                            return sortDir == "asc"
                                ? sd.Field(f => f.Field(p => p.DiscountPercent).Order(SortOrder.Ascending))
                                : sd.Field(f => f.Field(p => p.DiscountPercent).Order(SortOrder.Descending));
                        case "rating":
                            return sortDir == "asc"
                                ? sd.Field(f => f.Field(p => p.Rating).Order(SortOrder.Ascending))
                                : sd.Field(f => f.Field(p => p.Rating).Order(SortOrder.Descending));
                        default:
                            // unknown sort field -> fallback to score if keyword present
                            if (!string.IsNullOrWhiteSpace(keyword))
                                return sd.Descending(SortSpecialField.Score);
                            return sd;
                    }
                })
            );

            if (!resp.IsValid)
                throw new Exception($"Elasticsearch search error: {resp.ServerError?.ToString() ?? resp.OriginalException?.Message}");

            var docs = resp.Hits.Select(h => h.Source!).ToList();

            // when mapping search results back to DTOs, assign CreatedAt
            var mapped = docs.Select(d => new ProductSummaryDto
            {
                Id = d.Id,
                Name = d.Name,
                Slug = d.Slug,
                BrandName = d.Brand,
                CategoryName = d.Category,
                BasePrice = d.Price,
                DiscountPercent = d.DiscountPercent,
                CreatedAt = d.CreatedAt,
                PrimaryImagePath = d.ImageUrl
            }).ToList();

            var total = resp.Total > 0 ? (int)resp.Total : mapped.Count;
            return new PagedResult<ProductSummaryDto>(mapped, total, page, pageSize);
        }
    }
}
