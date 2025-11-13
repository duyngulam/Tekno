using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tekno.Domain.Catalog
{
    public class ProductFilterDto
    {
        public string? CategorySlug { get; set; }
        public string? BrandSlug { get; set; }
        public string? Search { get; set; }
        public Dictionary<string, string>? Attributes { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string? Sort { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 12;
    }
}
