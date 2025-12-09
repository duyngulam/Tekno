using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Tekno.Application.Catalog.DTOs.Admin
{
    /// <summary>
    /// Admin product list view with full details including variants and images
    /// </summary>
    public class AdminProductListDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string BrandName { get; set; } = string.Empty;
        public decimal BasePrice { get; set; }
        public int? DiscountPercent { get; set; }
        public decimal FinalPrice { get; set; }
        public string Status { get; set; } = string.Empty;
        
        // Full variant objects
        public List<AdminProductVariantDto> Variants { get; set; } = new();
        
        // Full image objects
        public List<ProductImageDto> Images { get; set; } = new();
        
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    /// <summary>
    /// Variant information for admin product list
    /// </summary>
    public class AdminProductVariantDto
    {
        public int Id { get; set; }
        public string Sku { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string Status { get; set; } = string.Empty;
        public List<Products.VariantAttributeDto> Attributes { get; set; } = new();
    }

    /// <summary>
    /// Product image information
    /// </summary>
    public class ProductImageDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsPrimary { get; set; }
        public int SortOrder { get; set; }
    }

    /// <summary>
    /// Add new image to product
    /// </summary>
    public class AddProductImageDto
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int ProductId { get; set; }
        
        [Required]
        public Microsoft.AspNetCore.Http.IFormFile ImageFile { get; set; } = null!;
        
        public bool IsPrimary { get; set; } = false;
    }

    /// <summary>
    /// Update existing product image
    /// </summary>
    public class UpdateProductImageDto
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int ImageId { get; set; }
        
        public bool? IsPrimary { get; set; }
        public int? SortOrder { get; set; }
    }

    /// <summary>
    /// Reorder product images
    /// </summary>
    public class ReorderImagesDto
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int ProductId { get; set; }
        
        [Required]
        [MinLength(1)]
        public List<int> ImageIds { get; set; } = new();
    }

    /// <summary>
    /// Add new variant to product
    /// </summary>
    public class AddProductVariantDto
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int ProductId { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Sku { get; set; } = string.Empty;
        
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }
        
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Stock cannot be negative")]
        public int Stock { get; set; }
        
        public string Status { get; set; } = "available";
        
        /// <summary>
        /// Attribute value selections: AttributeId -> Value string
        /// Example: { 1: "Black", 2: "16GB" } means AttributeId=1 uses value "Black"
        /// If the value doesn't exist for that attribute in the product's category, it will be created automatically.
        /// </summary>
        [Required]
        [MinLength(1, ErrorMessage = "At least one attribute value must be selected")]
        public Dictionary<int, string> AttributeValues { get; set; } = new();
    }

    /// <summary>
    /// Admin product search/filter request
    /// </summary>
    public class AdminProductSearchDto
    {
        public string? Search { get; set; }
        public string? Category { get; set; }
        public string? Brand { get; set; }
        public string? Status { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
