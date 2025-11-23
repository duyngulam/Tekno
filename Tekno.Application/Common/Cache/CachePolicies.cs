using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tekno.Application.Common.Cache
{
    public static class CachePolicies
    {
        //TTL values
        public static readonly TimeSpan CategoryTtl = TimeSpan.FromMinutes(60);
        public static readonly TimeSpan BrandTtl = TimeSpan.FromMinutes(60);
        public static readonly TimeSpan ProductTtl = TimeSpan.FromMinutes(15);
        public static readonly TimeSpan ProductListTtl = TimeSpan.FromMinutes(10);
        public static readonly TimeSpan SearchTtl = TimeSpan.FromMinutes(5);
        public static readonly TimeSpan BannerTtl = TimeSpan.FromHours(2);
        public static readonly TimeSpan NewProductsTtl = TimeSpan.FromMinutes(10);
        
        //Key prefixes
        public static string CategoryKey => "cache:category:all";
        public static string BrandKey => "cache:brand:all";
        public static string ProductKey(int id) => $"cache:product:{id}";
        public static string ProductListKey(int catId) => $"cache:product:cat:{catId}";
        public static string SearchKey(string keyword) => $"cache:search:{keyword.ToLower()}";
        public static string BannerKey => "cache:banner:active";
        public static string NewProductsKey(string categorySlug, int count) => 
            $"cache:products:new:{categorySlug}:{count}";
        
        /// <summary>
        /// Generate cache key for product search with all parameters
        /// </summary>
        public static string SearchProductsKey(
            string? keyword,
            string? category,
            string? brand,
            Dictionary<string, string>? filters,
            decimal? minPrice,
            decimal? maxPrice,
            string? sort,
            int page,
            int pageSize)
        {
            // Create a deterministic cache key based on all search parameters
            var keyParts = new List<string>
            {
                "cache:search:products",
                $"kw:{keyword?.ToLower() ?? "all"}",
                $"cat:{category?.ToLower() ?? "all"}",
                $"brand:{brand?.ToLower() ?? "all"}",
                $"minp:{minPrice?.ToString() ?? "0"}",
                $"maxp:{maxPrice?.ToString() ?? "0"}",
                $"sort:{sort?.ToLower() ?? "default"}",
                $"pg:{page}",
                $"ps:{pageSize}"
            };

            // Add filters to key
            if (filters != null && filters.Any())
            {
                var filterStr = string.Join(",", filters
                    .OrderBy(kv => kv.Key)
                    .Select(kv => $"{kv.Key.ToLower()}:{kv.Value.ToLower()}"));
                keyParts.Add($"f:{filterStr}");
            }

            return string.Join(":", keyParts);
        }
    }
}
