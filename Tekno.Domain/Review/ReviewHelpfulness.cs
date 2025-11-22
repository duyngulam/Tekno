using System;

namespace Tekno.Domain.Review
{
    /// <summary>
    /// Tracks which users found a review helpful or not helpful
    /// Prevents users from voting multiple times on the same review
    /// </summary>
    public class ReviewHelpfulness
    {
        public int Id { get; private set; }
        public int ReviewId { get; private set; }
        public int UserId { get; private set; }
        public bool IsHelpful { get; private set; } // true = helpful, false = not helpful
        public DateTime VotedAt { get; private set; } = DateTime.UtcNow;

        public ProductReview Review { get; private set; } = null!;

        public ReviewHelpfulness() { }

        public ReviewHelpfulness(int reviewId, int userId, bool isHelpful)
        {
            ReviewId = reviewId;
            UserId = userId;
            IsHelpful = isHelpful;
            VotedAt = DateTime.UtcNow;
        }
    }
}
