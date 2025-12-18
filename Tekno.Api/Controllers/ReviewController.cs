using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using Tekno.Api.Commons.Responses;
using Tekno.Application.Review.DTOs;
using Tekno.Application.Review.Services;

namespace Tekno.Api.Controllers
{
    /// <summary>
    /// Product review endpoints for customers
    /// Only users who purchased the product can leave reviews
    /// </summary>
    [ApiController]
    [Route("api/products/{productId}/reviews")]
    public class ReviewController : ControllerBase
    {
        private readonly ReviewService _reviewService;

        public ReviewController(ReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        /// <summary>
        /// Get reviews for a product (public)
        /// </summary>
        /// <param name="productId">Product ID</param>
        /// <param name="verifiedOnly">Filter to show only verified purchase reviews</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Items per page</param>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetProductReviews(
            int productId,
            [FromQuery] bool? verifiedOnly = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var reviews = await _reviewService.GetProductReviewsAsync(productId, verifiedOnly, page, pageSize);
            return Ok(ApiResponse<ReviewListDto>.Ok(reviews));
        }

        /// <summary>
        /// Get review summary for a product (public)
        /// </summary>
        /// <param name="productId">Product ID</param>
        [HttpGet("summary")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProductSummary(int productId)
        {
            var summary = await _reviewService.GetProductSummaryAsync(productId);
            return Ok(ApiResponse<ProductReviewSummaryDto>.Ok(summary));
        }

        /// <summary>
        /// Check if current user can review a product
        /// </summary>
        /// <param name="productId">Product ID</param>
        [HttpGet("can-review")]
        [Authorize]
        public async Task<IActionResult> CanReview(int productId)
        {
            var userId = GetCurrentUserId();
            var result = await _reviewService.CanUserReviewProductAsync(userId, productId);
            return Ok(ApiResponse<CanReviewResultDto>.Ok(result));
        }

        /// <summary>
        /// Create a review for a product
        /// </summary>
        /// <remarks>
        /// User must have purchased the product to leave a review.
        /// 
        /// Sample request:
        /// 
        ///     POST /api/products/1/reviews
        ///     {
        ///       "productId": 1,
        ///       "rating": 5,
        ///       "title": "Excellent laptop!",
        ///       "comment": "This is the best laptop I've ever owned. Fast, reliable, and great battery life."
        ///     }
        /// 
        /// </remarks>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateReview(int productId, [FromBody] CreateReviewDto dto)
        {
            // Ensure productId in route matches DTO
            dto.ProductId = productId;

            var userId = GetCurrentUserId();
            var review = await _reviewService.CreateReviewAsync(userId, dto);
            return CreatedAtAction(
                nameof(GetProductReviews),
                new { productId = productId },
                ApiResponse<ProductReviewDto>.Ok(review, "Review submitted successfully. It will be visible after approval."));
        }

        /// <summary>
        /// Update user's own review
        /// </summary>
        [HttpPut("{reviewId:int}")]
        [Authorize]
        public async Task<IActionResult> UpdateReview(int productId, int reviewId, [FromBody] UpdateReviewDto dto)
        {
            var userId = GetCurrentUserId();
            var review = await _reviewService.UpdateReviewAsync(userId, reviewId, dto);

            if (review == null)
                return NotFound(ApiResponse<ProductReviewDto>.Fail("Review not found"));

            return Ok(ApiResponse<ProductReviewDto>.Ok(review, "Review updated successfully"));
        }

        /// <summary>
        /// Delete user's own review
        /// </summary>
        [HttpDelete("{reviewId:int}")]
        [Authorize]
        public async Task<IActionResult> DeleteReview(int productId, int reviewId)
        {
            var userId = GetCurrentUserId();
            var success = await _reviewService.DeleteReviewAsync(userId, reviewId);

            if (!success)
                return NotFound(ApiResponse<bool>.Fail("Review not found"));

            return Ok(ApiResponse<bool>.Ok(true, "Review deleted successfully"));
        }

        /// <summary>
        /// Vote on review helpfulness
        /// </summary>
        /// <param name="productId">Product ID</param>
        /// <param name="reviewId">Review ID</param>
        /// <param name="dto">Vote data (isHelpful: true/false)</param>
        [HttpPost("{reviewId:int}/vote")]
        [Authorize]
        public async Task<IActionResult> VoteHelpfulness(
            int productId,
            int reviewId,
            [FromBody] ReviewHelpfulnessDto dto)
        {
            var userId = GetCurrentUserId();
            await _reviewService.VoteReviewHelpfulnessAsync(userId, reviewId, dto.IsHelpful);
            return Ok(ApiResponse<bool>.Ok(true, "Vote recorded successfully"));
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                throw new UnauthorizedAccessException("User not authenticated");
            }
            return userId;
        }
    }
}
