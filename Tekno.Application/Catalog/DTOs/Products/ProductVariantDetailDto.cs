using System;
using System.Collections.Generic;

namespace Tekno.Application.Catalog.DTOs.Products
{
    public class ProductVariantDetailDto
    {
        public int Id { get; set; }
        public string Sku { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string Status { get; set; } = "available";
        public DateTime CreatedAt { get; set; }
        
        // Product information
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductSlug { get; set; } = string.Empty;
        public string BrandName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        
        // Variant attributes (e.g., Color: Black, RAM: 16GB, Storage: 1TB)
        public List<VariantAttributeDto> Attributes { get; set; } = new();
    }

    public class VariantAttributeDto
    {
        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }
}