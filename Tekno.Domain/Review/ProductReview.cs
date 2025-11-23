using System;

namespace Tekno.Domain.Review
{
    public class ProductReview
    {
        public int Id { get; private set; }
        public int ProductId { get; private set; }
        public int UserId { get; private set; }
        public int? OrderId { get; private set; } // Track which order this review is for
        public int Rating { get; private set; } // 1-5 stars
        public string Title { get; private set; } = string.Empty;
        public string Comment { get; private set; } = string.Empty;
        public ReviewStatus Status { get; private set; } = ReviewStatus.Pending;
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; private set; }
        public DateTime? ApprovedAt { get; private set; }
        public int? ApprovedByUserId { get; private set; }
        
        // Helpful votes
        public int HelpfulCount { get; private set; } = 0;
        public int NotHelpfulCount { get; private set; } = 0;
        
        // Verification
        public bool IsVerifiedPurchase { get; private set; }
        public int? VariantId { get; private set; } // Which variant was purchased

        // Navigation properties (loaded separately)
        // public Product Product
        // public User User
        // public Order Order

        public ProductReview() { }

        public ProductReview(
            int productId,
            int userId,
            int rating,
            string title,
            string comment,
            int? orderId = null,
            int? variantId = null,
            bool isVerifiedPurchase = false)
        {
            if (rating < 1 || rating > 5)
                throw new ArgumentException("Rating must be between 1 and 5", nameof(rating));

            if (string.IsNullOrWhiteSpace(comment))
                throw new ArgumentException("Comment cannot be empty", nameof(comment));

            ProductId = productId;
            UserId = userId;
            Rating = rating;
            Title = title?.Trim() ?? string.Empty;
            Comment = comment.Trim();
            OrderId = orderId;
            VariantId = variantId;
            IsVerifiedPurchase = isVerifiedPurchase;
            Status = ReviewStatus.Pending;
            CreatedAt = DateTime.UtcNow;
        }

        public void Update(int rating, string title, string comment)
        {
            if (rating < 1 || rating > 5)
                throw new ArgumentException("Rating must be between 1 and 5", nameof(rating));

            if (string.IsNullOrWhiteSpace(comment))
                throw new ArgumentException("Comment cannot be empty", nameof(comment));

            Rating = rating;
            Title = title?.Trim() ?? string.Empty;
            Comment = comment.Trim();
            UpdatedAt = DateTime.UtcNow;
            
            // Reset to pending if updated after approval
            if (Status == ReviewStatus.Approved)
            {
                Status = ReviewStatus.Pending;
                ApprovedAt = null;
                ApprovedByUserId = null;
            }
        }

        public void Approve(int approvedByUserId)
        {
            Status = ReviewStatus.Approved;
            ApprovedAt = DateTime.UtcNow;
            ApprovedByUserId = approvedByUserId;
        }

        public void Reject(int rejectedByUserId)
        {
            Status = ReviewStatus.Rejected;
            ApprovedByUserId = rejectedByUserId;
        }

        public void IncrementHelpful()
        {
            HelpfulCount++;
        }

        public void IncrementNotHelpful()
        {
            NotHelpfulCount++;
        }
    }

    public enum ReviewStatus
    {
        Pending = 1,
        Approved = 2,
        Rejected = 3
    }
}
