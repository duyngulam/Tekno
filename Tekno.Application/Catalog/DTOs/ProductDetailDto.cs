namespace Tekno.Application.Catalog.DTOs
{
    public class ProductDetailDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;

        public decimal BasePrice { get; set; }
        public decimal? DiscountPercent { get; set; }

        public decimal FinalPrice =>
            DiscountPercent.HasValue
                ? Math.Round(BasePrice * (1 - DiscountPercent.Value / 100), 2)
                : BasePrice;

        public string? Overview { get; set; }
        public string? Description { get; set; }
        public string? WarrantyInfo { get; set; }

        public Dictionary<string, string>? Specs { get; set; }

        public List<string> Images { get; set; } = new();
        public List<ProductVariantDto> Variants { get; set; } = new();
    }
}
