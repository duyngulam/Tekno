using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Tekno.Application.Common.Validation;

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
        [Required(ErrorMessage = "Coupon code is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Coupon code must be between 3 and 50 characters")]
        public string Code { get; set; } = string.Empty;

        [Required(ErrorMessage = "Coupon name is required")]
        [StringLength(200, ErrorMessage = "Coupon name must not exceed 200 characters")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Coupon type is required")]
        [CouponTypeValidation]
        public string Type { get; set; } = "FixedAmount"; // FixedAmount, Percentage, FreeShipping

        [Required(ErrorMessage = "Coupon value is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Coupon value must be greater than 0")]
        public decimal Value { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Max usage per user must be at least 1")]
        public int? MaxUsagePerUser { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Minimum purchase amount must be 0 or greater")]
        public decimal? MinPurchaseAmount { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Maximum discount amount must be 0 or greater")]
        public decimal? MaxDiscountAmount { get; set; }

        [Required(ErrorMessage = "Start date is required")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "End date is required")]
        [DateRangeValidation(StartDateProperty = "StartDate")]
        public DateTime EndDate { get; set; }

        [StringLength(500, ErrorMessage = "Note must not exceed 500 characters")]
        public string? Note { get; set; }

        public List<int> ApplicableCategoryIds { get; set; } = new();
        public List<int> ApplicableProductIds { get; set; } = new();
    }

    public class UpdateCouponDto
    {
        [Required(ErrorMessage = "Coupon name is required")]
        [StringLength(200, ErrorMessage = "Coupon name must not exceed 200 characters")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Coupon value is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Coupon value must be greater than 0")]
        public decimal Value { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Max usage per user must be at least 1")]
        public int? MaxUsagePerUser { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Minimum purchase amount must be 0 or greater")]
        public decimal? MinPurchaseAmount { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Maximum discount amount must be 0 or greater")]
        public decimal? MaxDiscountAmount { get; set; }

        [Required(ErrorMessage = "Start date is required")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "End date is required")]
        [DateRangeValidation(StartDateProperty = "StartDate")]
        public DateTime EndDate { get; set; }

        [StringLength(500, ErrorMessage = "Note must not exceed 500 characters")]
        public string? Note { get; set; }

        public List<int> ApplicableCategoryIds { get; set; } = new();
        public List<int> ApplicableProductIds { get; set; } = new();
    }

    public class ValidateCouponDto
    {
        [Required(ErrorMessage = "Coupon code is required")]
        public string Code { get; set; } = string.Empty;

        [Required(ErrorMessage = "Order amount is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Order amount must be greater than 0")]
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
