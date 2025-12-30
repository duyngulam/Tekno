using System.Collections.Generic;

namespace Tekno.Application.Catalog.DTOs.Products
{
    /// <summary>
    /// Product search request DTO for Application layer
    /// </summary>
    public class ProductSearchRequestDto
    {
        public string? Keyword { get; set; }
        public string? Category { get; set; }
        public string? Brand { get; set; }
        public string? Sort { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        
        /// <summary>
        /// Filters dictionary: Key = attribute name, Value = comma-separated values
        /// Example: { "GPU": "RTX 4070,RTX 4080", "RAM": "16GB,32GB" }
        /// </summary>
        public Dictionary<string, string>? Filters { get; set; }

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public bool Suggest { get; set; } = false;
    }
}
