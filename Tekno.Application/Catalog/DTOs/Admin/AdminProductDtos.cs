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
        /// Attribute selections for the variant.
        /// Supports two formats:
        /// 
        /// 1. Using existing attribute by ID:
        ///    { "id": 1, "value": "Black" }
        /// 
        /// 2. Creating new attribute by name:
        ///    { "name": "Material", "value": "Aluminum" }
        /// 
        /// Examples:
        /// [
        ///   { "id": 1, "value": "Black" },           // Use existing Color attribute
        ///   { "id": 2, "value": "128GB" },           // Use existing Storage attribute
        ///   { "name": "Material", "value": "Metal" } // Create new Material attribute
        /// ]
        /// </summary>
        [Required]
        [MinLength(1, ErrorMessage = "At least one attribute must be specified")]
        public List<VariantAttributeInputDto> Attributes { get; set; } = new();
    }

    /// <summary>
    /// Input for variant attribute - supports both existing and new attributes
    /// </summary>
    public class VariantAttributeInputDto
    {
        /// <summary>
        /// Existing attribute ID (use this OR name, not both)
        /// </summary>
        public int? Id { get; set; }
        
        /// <summary>
        /// New attribute name (use this OR id, not both)
        /// If provided, a new attribute will be created in the product's category
        /// </summary>
        [StringLength(100)]
        public string? Name { get; set; }
        
        /// <summary>
        /// Attribute value (required)
        /// If the value doesn't exist for the attribute, it will be created
        /// </summary>
        [Required]
        [StringLength(200)]
        public string Value { get; set; } = string.Empty;

        /// <summary>
        /// Validate that either Id or Name is provided, but not both
        /// </summary>
        public bool IsValid(out string errorMessage)
        {
            if (!Id.HasValue && string.IsNullOrWhiteSpace(Name))
            {
                errorMessage = "Either 'Id' (existing attribute) or 'Name' (new attribute) must be provided";
                return false;
            }

            if (Id.HasValue && !string.IsNullOrWhiteSpace(Name))
            {
                errorMessage = "Cannot specify both 'Id' and 'Name'. Use one or the other";
                return false;
            }

            if (string.IsNullOrWhiteSpace(Value))
            {
                errorMessage = "Value is required";
                return false;
            }

            errorMessage = string.Empty;
            return true;
        }
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
