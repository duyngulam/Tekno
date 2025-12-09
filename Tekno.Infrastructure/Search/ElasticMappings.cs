using Nest;
using Tekno.Application.Catalog.DTOs.Products;

namespace Tekno.Infrastructure.Search
{
    public static class ElasticMappings
    {
        public static CreateIndexResponse CreateProductIndex(IElasticClient client)
        {
            return client.Indices.Create("products", c => c
                .Settings(s => s
                    .Analysis(a => a
                        .Normalizers(n => n
                            .Custom("lowercase_normalizer", cn => cn.Filters("lowercase"))
                        )
                        .Analyzers(an => an
                            // N-gram analyzer for partial matching (e.g., "mac" → "macbook")
                            .Custom("ngram_analyzer", ca => ca
                                .Tokenizer("standard")
                                .Filters("lowercase", "ngram_filter")
                            )
                            // Edge n-gram analyzer for autocomplete-style searches
                            .Custom("edge_ngram_analyzer", ca => ca
                                .Tokenizer("standard")
                                .Filters("lowercase", "edge_ngram_filter")
                            )
                        )
                        .TokenFilters(tf => tf
                            // N-gram filter for substring matching
                            .NGram("ngram_filter", ng => ng
                                .MinGram(3)
                                .MaxGram(10)
                            )
                            // Edge n-gram filter for prefix matching
                            .EdgeNGram("edge_ngram_filter", eng => eng
                                .MinGram(2)
                                .MaxGram(15)
                            )
                        )
                    )
                )
                .Map<ProductSearchDocument>(m => m
                    .AutoMap()
                    .Properties(p => p
                        // Multi-field for name: standard + n-gram for better partial matching
                        .Text(t => t
                            .Name(n => n.Name)
                            .Analyzer("standard")
                            .Fields(f => f
                                .Text(tt => tt.Name("ngram").Analyzer("ngram_analyzer"))
                                .Text(tt => tt.Name("edge").Analyzer("edge_ngram_analyzer"))
                            )
                        )
                        // exact match / aggregations — use normalizer so keyword comparisons are case-insensitive
                        .Keyword(k => k.Name(n => n.Slug).Normalizer("lowercase_normalizer"))
                        .Keyword(k => k.Name(n => n.Brand).Normalizer("lowercase_normalizer"))
                        .Keyword(k => k.Name(n => n.Category).Normalizer("lowercase_normalizer"))
                        .Number(nu => nu.Name(n => n.Price).Type(NumberType.Double))
                        .Number(nu => nu.Name(n => n.DiscountPercent).Type(NumberType.Integer))
                        .Keyword(k => k.Name(n => n.ImageUrl))
                        // specs as nested objects with keyword fields using the same normalizer
                        .Nested<ProductAttributeDto>(n => n
                            .Name(nn => nn.Specs)
                            .AutoMap()
                            .Properties(pp => pp
                                .Keyword(k => k.Name(a => a.Name).Normalizer("lowercase_normalizer"))
                                .Keyword(k => k.Name(a => a.Value).Normalizer("lowercase_normalizer"))
                            )
                        )
                        // created date for sorting
                        .Date(d => d.Name(n => n.CreatedAt).Format("strict_date_optional_time||epoch_millis"))
                        .Number(nu => nu.Name(n => n.Rating).Type(NumberType.Double))
                    )
                )
            );
        }

        public static CreateIndexResponse CreateProductDetailIndex(IElasticClient client)
        {
            return client.Indices.Create("product_details", c => c
                .Map<ProductDetailDto>(m => m
                    .AutoMap()
                    .Properties(p => p
                        .Keyword(k => k.Name(n => n.BrandName).Normalizer("lowercase_normalizer"))
                        .Keyword(k => k.Name(n => n.CategoryName).Normalizer("lowercase_normalizer"))
                        .Nested<ProductAttributeDto>(n => n
                            .Name(nn => nn.Specs)
                            .AutoMap()
                            .Properties(pp => pp
                                .Keyword(k => k.Name(a => a.Name).Normalizer("lowercase_normalizer"))
                                .Keyword(k => k.Name(a => a.Value).Normalizer("lowercase_normalizer"))
                            )
                        )
                    )
                )
            );
        }
    }
}
