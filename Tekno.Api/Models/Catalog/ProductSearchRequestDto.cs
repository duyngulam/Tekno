using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Tekno.Api.Models.Catalog
{
    /// <summary>
    /// Product search and filter request for API endpoints.
    /// Filters should be passed using `filters[AttributeName]=value` or `filters.AttributeName=value`.
    /// Use comma-separated values for OR semantics: `filters[Color]=Black,White`
    /// Repeated parameters are also supported:
    /// `filters[Color]=Black&filters[Color]=White` -> treated as Black,White
    /// You can also pass a JSON encoded filters object in the query string using `filtersJson`.
    /// Example (URL-encoded): ?filtersJson={"RAM":["16GB","32GB"],"Color":["Black"]}
    /// </summary>
    public class ProductSearchRequestDto
    {
        public string? Keyword { get; set; }
        public string? Category { get; set; }
        public string? Brand { get; set; }
        public string? Sort { get; set; }

        /// <summary>
        /// Minimum price (VND)
        /// </summary>
        public decimal? MinPrice { get; set; }

        /// <summary>
        /// Maximum price (VND)
        /// </summary>
        public decimal? MaxPrice { get; set; }

        /// <summary>
        /// Specification filters bound from query: ?filters[Color]=Black&filters[Size]=XL
        /// </summary>
        [ModelBinder(BinderType = typeof(ProductFiltersModelBinder))]
        public Dictionary<string, string>? Filters { get; set; }

        /// <summary>
        /// Alternative: JSON encoded filters object. Example: {"RAM":["16GB","32GB"]}
        /// This value must be URL-encoded when sent as a query parameter.
        /// </summary>
        public string? FiltersJson { get; set; }

        [Range(1, int.MaxValue)]
        public int Page { get; set; } = 1;

        [Range(1, 100)]
        public int PageSize { get; set; } = 20;

        public bool Suggest { get; set; } = false;
    }
}
