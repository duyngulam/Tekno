using Elasticsearch.Net;
using Nest;
using Tekno.Domain.Catalog;
using Tekno.Infrastructure.Search;

namespace Tekno.Infrastructure.Services
{
    public static class ElasticSearchConfig
    {
        public static IElasticClient CreateClient(string uri)
        {
            var settings = new ConnectionSettings(new Uri(uri))
                .DefaultIndex("products") // default index name
                .PrettyJson()
                .DisableDirectStreaming()
                .DefaultMappingFor<ProductSearchDocument>(m => m
                    .IdProperty(p => p.Id)
                    .IndexName("products")
                );

            return new ElasticClient(settings);
        }
    }
}
//public async Task IndexProductAsync(Product product)
//{
//    var doc = new ProductSearchDocument
//    {
//        Id = product.Id,
//        Name = product.Name,
//        Slug = product.Slug,
//        Category = product.Category.Name,
//        Brand = product.Brand.Name,
//        Price = product.BasePrice,
//        Specs = JsonSerializer.Deserialize<Dictionary<string, string>>(product.Detail?.Specs ?? "{}"),
//        ImageUrl = product.Images.FirstOrDefault(i => i.IsPrimary)?.ImageUrl ?? "",
//        DiscountPercent = product.DiscountPercent ?? 0
//    };

//    await _client.IndexDocumentAsync(doc);
//}
