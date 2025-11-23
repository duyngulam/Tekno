using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Tekno.Application.Review.DTOs
{
    public class ProductReviewDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int UserId { get; set; }
        public string UserEmail { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Comment { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsVerifiedPurchase { get; set; }
        public int HelpfulCount { get; set; }
        public int NotHelpfulCount { get; set; }
        
        // Variant details (if available)
        public string? VariantSku { get; set; }
        public List<string>? VariantAttributes { get; set; }
    }

    public class CreateReviewDto
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int ProductId { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int Rating { get; set; }

        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public string? Title { get; set; }

        [Required]
        [StringLength(2000, MinimumLength = 10, ErrorMessage = "Comment must be between 10 and 2000 characters")]
        public string Comment { get; set; } = string.Empty;

        /// <summary>
        /// Optional: Specify which order this review is for
        /// System will auto-detect if not provided
        /// </summary>
        public int? OrderId { get; set; }
    }

    public class UpdateReviewDto
    {
        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int Rating { get; set; }

        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public string? Title { get; set; }

        [Required]
        [StringLength(2000, MinimumLength = 10, ErrorMessage = "Comment must be between 10 and 2000 characters")]
        public string Comment { get; set; } = string.Empty;
    }

    public class ReviewHelpfulnessDto
    {
        [Required]
        public bool IsHelpful { get; set; }
    }

    public class ProductReviewSummaryDto
    {
        public int ProductId { get; set; }
        public int TotalReviews { get; set; }
        public double AverageRating { get; set; }
        public Dictionary<int, int> RatingDistribution { get; set; } = new(); // Star -> Count
        public int VerifiedPurchaseCount { get; set; }
    }

    public class ReviewListDto
    {
        public List<ProductReviewDto> Reviews { get; set; } = new();
        public ProductReviewSummaryDto Summary { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }

    public class CanReviewResultDto
    {
        public bool CanReview { get; set; }
        public string Message { get; set; } = string.Empty;
        public bool HasPurchased { get; set; }
        public bool HasAlreadyReviewed { get; set; }
        public List<PurchaseInfoDto> EligibleOrders { get; set; } = new();
    }

    public class PurchaseInfoDto
    {
        public int OrderId { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public DateTime PurchaseDate { get; set; }
        public int VariantId { get; set; }
        public string VariantSku { get; set; } = string.Empty;
    }
}
