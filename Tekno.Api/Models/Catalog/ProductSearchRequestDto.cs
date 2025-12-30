using System.ComponentModel.DataAnnotations;

namespace Tekno.Api.Models.Catalog
{
    /// <summary>
    /// Product search and filter request for API endpoints.
    /// Use 'filters' parameter with JSON format: {"AttributeName":["value1","value2"]}
    /// </summary>
    public class ProductSearchRequestDto
    {
        /// <summary>
        /// Search keyword for product name
        /// </summary>
        public string? Keyword { get; set; }

        /// <summary>
        /// Category slug filter (e.g., "laptops", "smartphones")
        /// </summary>
        public string? Category { get; set; }

        /// <summary>
        /// Brand slug filter (e.g., "apple", "samsung")
        /// </summary>
        public string? Brand { get; set; }

        /// <summary>
        /// Sort order. Options: price, -price, name, -name, created, -created, popular, rating
        /// </summary>
        public string? Sort { get; set; }

        /// <summary>
        /// Minimum price in VND (e.g., 10000000 = 10 million VND)
        /// </summary>
        public decimal? MinPrice { get; set; }

        /// <summary>
        /// Maximum price in VND (e.g., 50000000 = 50 million VND)
        /// </summary>
        public decimal? MaxPrice { get; set; }

        /// <summary>
        /// JSON encoded filters object. 
        /// Format: {"AttributeName":["value1","value2"]}
        /// Example: {"GPU":["RTX 4070"],"RAM":["16GB","32GB"]}
        /// Must be URL-encoded when sent as query parameter.
        /// </summary>
        public string? Filters { get; set; }

        /// <summary>
        /// Page number (starts from 1)
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = "Page must be at least 1")]
        public int Page { get; set; } = 1;

        /// <summary>
        /// Number of items per page (1-100)
        /// </summary>
        [Range(1, 100, ErrorMessage = "PageSize must be between 1 and 100")]
        public int PageSize { get; set; } = 20;

        /// <summary>
        /// Enable search suggestions (experimental feature)
        /// </summary>
        public bool Suggest { get; set; } = false;
    }
}
