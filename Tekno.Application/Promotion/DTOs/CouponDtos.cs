using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Tekno.Application.Promotion.DTOs
{
    public class CouponDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // "FixedAmount", "Percentage", "FreeShipping"
        public decimal Value { get; set; }
        public int Quantity { get; set; }
        public int UsedCount { get; set; }
        public int RemainingQuantity { get; set; }
        public int? MaxUsagePerUser { get; set; }
        public decimal? MinPurchaseAmount { get; set; }
        public decimal? MaxDiscountAmount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; } = string.Empty; // "Active", "Inactive", "Expired"
        public string? Note { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        
        public List<int> ApplicableCategoryIds { get; set; } = new();
        public List<int> ApplicableProductIds { get; set; } = new();
    }

    public class CreateCouponDto
    {
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Code { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Type { get; set; } = "FixedAmount"; // FixedAmount, Percentage, FreeShipping

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Value { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Range(1, int.MaxValue)]
        public int? MaxUsagePerUser { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? MinPurchaseAmount { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? MaxDiscountAmount { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [StringLength(500)]
        public string? Note { get; set; }

        public List<int> ApplicableCategoryIds { get; set; } = new();
        public List<int> ApplicableProductIds { get; set; } = new();
    }

    public class UpdateCouponDto
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Value { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Range(1, int.MaxValue)]
        public int? MaxUsagePerUser { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? MinPurchaseAmount { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? MaxDiscountAmount { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [StringLength(500)]
        public string? Note { get; set; }

        public List<int> ApplicableCategoryIds { get; set; } = new();
        public List<int> ApplicableProductIds { get; set; } = new();
    }

    public class ValidateCouponDto
    {
        [Required]
        public string Code { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal OrderAmount { get; set; }

        public int? UserId { get; set; }
        public List<int> ProductIds { get; set; } = new();
        public List<int> CategoryIds { get; set; } = new();
    }

    public class CouponValidationResult
    {
        public bool IsValid { get; set; }
        public string Message { get; set; } = string.Empty;
        public decimal DiscountAmount { get; set; }
        public CouponDto? Coupon { get; set; }
    }

    public class CouponUsageDto
    {
        public int Id { get; set; }
        public string CouponCode { get; set; } = string.Empty;
        public string CouponName { get; set; } = string.Empty;
        public int UserId { get; set; }
        public int OrderId { get; set; }
        public decimal DiscountAmount { get; set; }
        public DateTime UsedAt { get; set; }
    }
}
