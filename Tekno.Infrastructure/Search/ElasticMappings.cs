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
                    )
                )
                .Map<ProductSearchDocument>(m => m
                    .AutoMap()
                    .Properties(p => p
                        // full-text
                        .Text(t => t.Name(n => n.Name).Analyzer("standard"))
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
