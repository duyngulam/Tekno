using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Tekno.Application.Catalog.DTOs.Advertisement
{
    /// <summary>
    /// Advertisement banner with product link
    /// </summary>
    public class ProductAdvertisementDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductSlug { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public int Priority { get; set; }
        public bool IsActive { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsCurrentlyActive { get; set; }
    }

    /// <summary>
    /// Create advertisement - just image and product ID
    /// </summary>
    public class CreateAdvertisementDto
    {
        [Required(ErrorMessage = "Image is required")]
        public IFormFile Image { get; set; } = null!;

        [Required(ErrorMessage = "Product ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Product ID must be greater than 0")]
        public int ProductId { get; set; }

        /// <summary>
        /// Display position: HomeTop, HomeMiddle, HomeBottom, CategoryTop, ProductSidebar
        /// </summary>
        public string Position { get; set; } = "HomeTop";

        /// <summary>
        /// Display priority (0-100). Higher number = shown first
        /// </summary>
        [Range(0, 100)]
        public int Priority { get; set; } = 0;

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool IsActive { get; set; } = true;
    }

    /// <summary>
    /// Update advertisement
    /// </summary>
    public class UpdateAdvertisementDto
    {
        /// <summary>
        /// New image (optional - only if changing image)
        /// </summary>
        public IFormFile? Image { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int ProductId { get; set; }

        public string Position { get; set; } = "HomeTop";

        [Range(0, 100)]
        public int Priority { get; set; } = 0;

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool IsActive { get; set; } = true;
    }

    /// <summary>
    /// Query parameters for filtering advertisements
    /// </summary>
    public class AdvertisementQueryDto
    {
        public string? Position { get; set; }
        public bool? IsActive { get; set; }
        public bool OnlyCurrentlyActive { get; set; } = false;
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
