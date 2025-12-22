using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Tekno.Application.Promotion.DTOs
{
    public class PromotionDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Type { get; set; } = string.Empty; // "Percentage" or "FixedAmount"
        public decimal Value { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; } = string.Empty; // "Scheduled", "Active", "Paused", "Expired"
        public int Priority { get; set; }
        public bool StackableWithCoupons { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        
        public List<int> ApplicableCategoryIds { get; set; } = new();
        public List<int> ApplicableProductIds { get; set; } = new();
        
        // Computed fields
        public bool IsActive { get; set; }
        public int AffectedProductsCount { get; set; }
    }

    public class CreatePromotionDto
    {
        [Required(ErrorMessage = "Promotion name is required")]
        [StringLength(200, ErrorMessage = "Promotion name must not exceed 200 characters")]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Description must not exceed 1000 characters")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Promotion type is required")]
        public string Type { get; set; } = "Percentage"; // "Percentage" or "FixedAmount"

        [Required(ErrorMessage = "Discount value is required")]
        [Range(0.01, 100, ErrorMessage = "For percentage discounts, value must be between 0.01 and 100")]
        public decimal Value { get; set; }

        [Required(ErrorMessage = "Start date is required")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "End date is required")]
        public DateTime EndDate { get; set; }

        [Range(0, 100, ErrorMessage = "Priority must be between 0 and 100")]
        public int Priority { get; set; } = 0;

        public bool StackableWithCoupons { get; set; } = true;

        public List<int> ApplicableCategoryIds { get; set; } = new();
        public List<int> ApplicableProductIds { get; set; } = new();
    }

    public class UpdatePromotionDto
    {
        [Required(ErrorMessage = "Promotion name is required")]
        [StringLength(200, ErrorMessage = "Promotion name must not exceed 200 characters")]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Description must not exceed 1000 characters")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Discount value is required")]
        [Range(0.01, 100, ErrorMessage = "For percentage discounts, value must be between 0.01 and 100")]
        public decimal Value { get; set; }

        [Required(ErrorMessage = "Start date is required")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "End date is required")]
        public DateTime EndDate { get; set; }

        [Range(0, 100, ErrorMessage = "Priority must be between 0 and 100")]
        public int Priority { get; set; } = 0;

        public bool StackableWithCoupons { get; set; } = true;

        public List<int> ApplicableCategoryIds { get; set; } = new();
        public List<int> ApplicableProductIds { get; set; } = new();
    }

    public class PromotionStatsDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal Value { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int AffectedProductsCount { get; set; }
        public int AffectedCategoriesCount { get; set; }
        public decimal EstimatedDiscountTotal { get; set; }
        public int DaysRemaining { get; set; }
    }
}
