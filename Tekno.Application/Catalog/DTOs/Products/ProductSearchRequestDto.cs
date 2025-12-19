namespace Tekno.Application.Catalog.DTOs.Products
{
    public class ProductSearchRequestDto
    {
        public string? Keyword { get; set; }
        public string? Category { get; set; }
        public string? Brand { get; set; }
        public string? Sort { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        // Spec filters bound from query: ?filters[Color]=Black&filters[Size]=XL
        public Dictionary<string, string>? Filters { get; set; } = new();

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 12;
        public bool Suggest { get; set; } = false;
    }
}
