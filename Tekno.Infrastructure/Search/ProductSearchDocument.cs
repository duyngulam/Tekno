using Nest;
using System;
using System.Collections.Generic;
using Tekno.Application.Catalog.DTOs.Products;

namespace Tekno.Infrastructure.Search
{
    public class ProductSearchDocument
    {
        public int Id { get; set; }

        [Text(Name = "name")]
        public string Name { get; set; } = string.Empty;

        [Keyword(Name = "slug")]
        public string Slug { get; set; } = string.Empty;

        [Keyword(Name = "brand")]
        public string Brand { get; set; } = string.Empty;

        // Primary category (single) - kept for backward compatibility
        [Keyword(Name = "category")]
        public string Category { get; set; } = string.Empty;

        // Categories list contains product's category plus all ancestor category slugs
        [Keyword(Name = "categories")]
        public List<string> Categories { get; set; } = new();

        [Number(NumberType.Double, Name = "price")]
        public decimal Price { get; set; }

        [Number(NumberType.Integer, Name = "discountPercent")]
        public int DiscountPercent { get; set; }

        [Keyword(Name = "imageUrl")]
        public string ImageUrl { get; set; } = string.Empty;

        // ✅ specs can be filtered as nested
        [Nested(Name = "specs")]
        public List<ProductAttributeDto> Specs { get; set; } = new();

        [Number(NumberType.Double, Name = "rating")]
        public double? Rating { get; set; }

        // Add CreatedAt so we can sort by date
        [Date(Name = "createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

