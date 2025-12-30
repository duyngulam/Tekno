using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<ElasticProductService> _logger;
        private const string IndexName = "products";
        // cache mapping check result to avoid repeated mapping lookups
        private static bool? _specsIsNestedCache = null;

        public ElasticProductService(IElasticClient client, IProductRepository productRepository, ILogger<ElasticProductService> logger)
        {
            _client = client;
            _productRepository = productRepository;
            _logger = logger;
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

            // log what we are indexing for troubleshooting spec filters
            try
            {
                if (doc.Specs != null && doc.Specs.Any())
                    _logger.LogDebug("Indexing product {ProductId} specs: {Specs}", doc.Id, string.Join(',', doc.Specs.Select(s => s.Name + ":" + string.Join('|', s.Value))));
                else
                    _logger.LogDebug("Indexing product {ProductId} with no specs", doc.Id);
            }
            catch { }

            var indexResp = await _client.IndexAsync(doc, i => i.Index(IndexName).Id(doc.Id).Refresh(Elasticsearch.Net.Refresh.True));
            if (!indexResp.IsValid)
            {
                throw new Exception($"Elasticsearch index error: {indexResp.ServerError?.ToString() ?? indexResp.OriginalException?.Message}");
            }
        }

        public async Task DeleteProductAsync(int id)
        {
            var resp = await _client.DeleteAsync<ProductSearchDocument>(id, d => d.Index(IndexName).Refresh(Elasticsearch.Net.Refresh.True));
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
            try
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
                    // Use multi-value 'Categories' field which contains the product's category plus ancestor slugs
                    // use keyword field for exact term match
                    filterQueries.Add(new TermQuery { Field = Infer.Field<ProductSearchDocument>(p => p.Categories), Value = categorySlug.ToLowerInvariant() });

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

                if (filters != null && filters.Any())
                {
                    // determine if 'specs' is mapped as nested in the index
                    bool useNested = await IsSpecsMappedAsNestedAsync();
                    if (useNested)
                    {
                        _logger.LogDebug("Specs field is mapped as nested; building nested queries");
                    }
                    else
                    {
                        _logger.LogDebug("Specs field is NOT nested; building non-nested queries");
                    }

                    foreach (var kv in filters)
                    {
                        // Normalize filter name and values to match indexed keywords (lowercase + ascii folding)
                        var specName = NormalizeTerm(kv.Key ?? string.Empty);
                        var specValueRaw = (kv.Value ?? string.Empty).Trim();

                        if (string.IsNullOrEmpty(specName) || string.IsNullOrEmpty(specValueRaw))
                            continue;

                        List<string> specValues;
                        // Frontend may send JSON array string (e.g. ["RTX 4060"]). Try to parse it first.
                        if (specValueRaw.StartsWith("[") && specValueRaw.EndsWith("]"))
                        {
                            try
                            {
                                var arr = JsonSerializer.Deserialize<string[]>(specValueRaw);
                                specValues = (arr ?? Array.Empty<string>()).Select(v => v?.Trim() ?? string.Empty)
                                                           .Where(v => !string.IsNullOrEmpty(v))
                                                           .Select(v => v.ToLowerInvariant())
                                                           .Distinct()
                                                           .ToList();
                            }
                            catch
                            {
                                // fallback to splitting
                                specValues = specValueRaw.Split(new[] { ',', '|' }, StringSplitOptions.RemoveEmptyEntries)
                                                           .Select(v => v.Trim().ToLowerInvariant())
                                                           .Where(v => !string.IsNullOrEmpty(v))
                                                           .Distinct()
                                                           .ToList();
                            }
                        }
                        else
                        {
                            specValues = specValueRaw.Split(new[] { ',', '|' }, StringSplitOptions.RemoveEmptyEntries)
                                                       .Select(v => v.Trim().ToLowerInvariant())
                                                       .Where(v => !string.IsNullOrEmpty(v))
                                                       .Distinct()
                                                       .ToList();
                        }

                        if (!specValues.Any())
                            continue;

                        if (useNested)
                        {
                            // Build nested query using TermQuery on keyword fields and IgnoreUnmapped to avoid shard failures
                            var nestedShould = new List<QueryContainer>();

                            foreach (var value in specValues)
                            {
                                var nested = new NestedQuery
                                {
                                    Path = Infer.Field<ProductSearchDocument>(p => p.Specs),
                                    IgnoreUnmapped = true,
                                    Query = new BoolQuery
                                    {
                                        Must = new QueryContainer[]
                                        {
                                            // match attribute name exactly (keyword)
                                            new TermQuery { Field = Infer.Field<ProductSearchDocument>(p => p.Specs.First().Name), Value = specName },
                                            // match attribute value exactly (keyword)
                                            new TermQuery { Field = Infer.Field<ProductSearchDocument>(p => p.Specs.First().Value.First()), Value = value }
                                        }
                                    }
                                };

                                nestedShould.Add(nested);
                            }

                            if (nestedShould.Count == 1)
                            {
                                filterQueries.Add(nestedShould[0]);
                            }
                            else if (nestedShould.Count > 1)
                            {
                                filterQueries.Add(new BoolQuery
                                {
                                    Should = nestedShould,
                                    MinimumShouldMatch = 1
                                });
                            }
                        }
                        else
                        {
                            // Non-nested mapping: build term queries against specs.name.keyword and specs.value.keyword
                            var nonNestedShould = new List<QueryContainer>();
                            foreach (var value in specValues)
                            {
                                var bq = new BoolQuery
                                {
                                    Must = new QueryContainer[]
                                    {
                                        new TermQuery { Field = "specs.name.keyword", Value = specName },
                                        new TermQuery { Field = "specs.value.keyword", Value = value }
                                    }
                                };

                                nonNestedShould.Add(bq);
                            }

                            if (nonNestedShould.Count == 1)
                                filterQueries.Add(nonNestedShould[0]);
                            else if (nonNestedShould.Count > 1)
                                filterQueries.Add(new BoolQuery { Should = nonNestedShould, MinimumShouldMatch = 1 });
                        }

                        _logger.LogDebug("Applying spec filter '{SpecName}' -> values: {Values}", specName, string.Join(',', specValues));
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

                var sortField = "date";
                var sortDir = "desc";
                if (!string.IsNullOrWhiteSpace(sort))
                {
                    var parts = sort.ToLowerInvariant().Split(new[] { '_', ':' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length > 0) sortField = parts[0];
                    if (parts.Length > 1) sortDir = parts[1] == "asc" ? "asc" : "desc";
                }

                // Execute search with try/catch around client call to capture server errors
                ISearchResponse<ProductSearchDocument> resp;
                try
                {
                    resp = await _client.SearchAsync<ProductSearchDocument>(s => s
                        .Index(IndexName)
                        .From(from)
                        .Size(pageSize)
                        .TrackTotalHits()
                        .Query(q => finalQuery)
                        .Sort(sd =>
                        {
                            if (string.IsNullOrWhiteSpace(sort))
                            {
                                if (!string.IsNullOrWhiteSpace(keyword))
                                    return sd.Descending(SortSpecialField.Score);
                                return sd;
                            }

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
                                    if (!string.IsNullOrWhiteSpace(keyword))
                                        return sd.Descending(SortSpecialField.Score);
                                    return sd;
                            }
                        })
                    );
                }
                catch (Exception ex)
                {
                    // convert client exceptions into a controlled exception to be handled by outer catch
                    _logger.LogError(ex, "Elasticsearch client threw during SearchAsync");
                    throw new Exception("Elasticsearch client error during search", ex);
                }

                if (!resp.IsValid)
                    throw new Exception($"Elasticsearch search error: {resp.ServerError?.ToString() ?? resp.OriginalException?.Message}");

                _logger.LogDebug("Elasticsearch returned {TotalHits} hits (took {Took}ms)", resp.Total, resp.Took);

                var docs = resp.Hits.Select(h => h.Source!).ToList();

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
            catch (Exception ex)
            {
                // Log and fallback to DB query to keep API robust
                _logger.LogError(ex, "Elasticsearch query failed, falling back to database search");

                // Fallback: use repository DB search (note: DB search does not support nested spec filters)
                try
                {
                    var paging = new PagingParams(page, pageSize);
                    var dbResult = await _productRepository.GetPagedProductAsync(
                        categorySlug,
                        brandSlug,
                        keyword,
                        sort,
                        minPrice?.ToString(),
                        maxPrice?.ToString(),
                        paging);

                    var mapped = dbResult.Data.Select(p => new ProductSummaryDto
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Slug = p.Slug,
                        BrandName = p.Brand?.Name,
                        CategoryName = p.Category?.Name,
                        BasePrice = p.BasePrice,
                        DiscountPercent = p.DiscountPercent.HasValue ? (int?)Math.Round(p.DiscountPercent.Value) : null,
                        CreatedAt = p.CreatedAt,
                        PrimaryImagePath = p.Images?.FirstOrDefault()?.ImageUrl
                    }).ToList();

                    return new PagedResult<ProductSummaryDto>(mapped, dbResult.TotalRecords, page, pageSize);
                }
                catch (Exception dbEx)
                {
                    _logger.LogError(dbEx, "Database fallback search also failed");
                    throw; // rethrow original exception flow
                }
            }
        }

        private static string NormalizeTerm(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return string.Empty;
            var s = input.Trim().ToLowerInvariant();
            // remove diacritics
            var normalized = s.Normalize(System.Text.NormalizationForm.FormD);
            var sb = new System.Text.StringBuilder();
            foreach (var ch in normalized)
            {
                var uc = CharUnicodeInfo.GetUnicodeCategory(ch);
                if (uc != UnicodeCategory.NonSpacingMark)
                    sb.Append(ch);
            }
            return sb.ToString().Normalize(System.Text.NormalizationForm.FormC);
        }

        private async Task<bool> IsSpecsMappedAsNestedAsync()
        {
            if (_specsIsNestedCache.HasValue) return _specsIsNestedCache.Value;

            try
            {
                var mapping = await _client.Indices.GetMappingAsync(new GetMappingRequest(IndexName));
                if (!mapping.IsValid) return false;

                var indexMap = mapping.Indices.FirstOrDefault().Value;
                if (indexMap == null) return false;

                // attempt to locate 'specs' property and check its 'type'
                var props = indexMap.Mappings.Properties;
                if (props != null && props.TryGetValue("specs", out var specProp))
                {
                    try
                    {
                        // Try to infer nested from the property type string
                        var typeStr = specProp.Type?.ToString() ?? string.Empty;
                        _specsIsNestedCache = typeStr.Equals("Nested", StringComparison.OrdinalIgnoreCase) || typeStr.Equals("nested", StringComparison.OrdinalIgnoreCase);
                        return _specsIsNestedCache.Value;
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to get index mapping for {Index}", IndexName);
            }

            _specsIsNestedCache = false;
            return false;
        }
    }
}
