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
        //Key prefixes
        public static string CategoryKey => "cache:category:all";
        public static string BrandKey => "cache:brand:all";
        public static string ProductKey(int id) => $"cache:product:{id}";
        public static string ProductListKey(int catId) => $"cache:product:cat:{catId}";
        public static string SearchKey(string keyword) => $"cache:search:{keyword.ToLower()}";
        public static string BannerKey => "cache:banner:active";
    }
}
