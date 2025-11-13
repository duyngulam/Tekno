using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Tekno.Application.Catalog.DTOs.Products
{
    //Tóm tắt sản phẩm dùng để hiện khi search, dạng thẻ
    public class ProductSummaryDto
    {
        public int Id { get; set; }
        public string BrandName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public decimal BasePrice { get; set; }
        public decimal? DiscountPercent { get; set; }
        public decimal FinalPrice => DiscountPercent.HasValue
       ? Math.Round(BasePrice * (1 - DiscountPercent.Value / 100), 2)
       : BasePrice;
        public string? Overview { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? PrimaryImagePath { get; set; } = "https://i.pinimg.com/736x/bd/e2/b8/bde2b888e9f57b2eee6f5ce3c90ce400.jpg";
    }
}
