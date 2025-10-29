using System.Text.Json.Serialization;

namespace Tekno.Api.Models.Catalog
{
    public class ProductSummaryApiDto
    {
        public int Id { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string Slug { get; private set; } = string.Empty;
        public decimal BasePrice { get; private set; }
        public decimal? DiscountPercent { get; set; }
        [JsonIgnore]
        public decimal FinalPrice => DiscountPercent.HasValue
       ? Math.Round(BasePrice * (1 - DiscountPercent.Value / 100), 2)
                   : BasePrice;
        public string? Overview { get; private set; }
        public string? PrimaryImagePath { get; set; } = "https://i.pinimg.com/736x/bd/e2/b8/bde2b888e9f57b2eee6f5ce3c90ce400.jpg";
    }
}
