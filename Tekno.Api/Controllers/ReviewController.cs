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
        /// <param name="productId">Product ID to get reviews for</param>
        /// <param name="verifiedOnly">Filter to show only verified purchase reviews</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Items per page</param>
        /// <remarks>
        /// Example:
        ///     GET /api/products/1/reviews?verifiedOnly=true&amp;page=1&amp;pageSize=20
        /// </remarks>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<ReviewListDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<string>), 400)]
        [ProducesResponseType(typeof(ApiResponse<string>), 500)]
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
        /// <param name="productId">Product ID to get summary for</param>
        /// <remarks>
        /// Example:
        ///     GET /api/products/1/reviews/summary
        /// </remarks>
        [HttpGet("summary")]
        [ProducesResponseType(typeof(ApiResponse<ProductReviewSummaryDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<string>), 400)]
        [ProducesResponseType(typeof(ApiResponse<string>), 500)]
        [AllowAnonymous]
        public async Task<IActionResult> GetProductSummary(int productId)
        {
            var summary = await _reviewService.GetProductSummaryAsync(productId);
            return Ok(ApiResponse<ProductReviewSummaryDto>.Ok(summary));
        }

        /// <summary>
        /// Check if current user can review a product
        /// </summary>
        /// <param name="productId">Product ID to check</param>
        /// <remarks>
        /// Returns information about whether user has purchased the product
        /// and if they've already submitted a review.
        /// 
        /// Example:
        ///     GET /api/products/1/reviews/can-review
        /// </remarks>
        [HttpGet("can-review")]
        [ProducesResponseType(typeof(ApiResponse<CanReviewResultDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<string>), 400)]
        [ProducesResponseType(typeof(ApiResponse<string>), 500)]
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
        /// The system will automatically detect which order to link the review to.
        /// 
        /// Sample request:
        /// 
        ///     POST /api/products/1/reviews
        ///     {
        ///       "rating": 5,
        ///       "comment": "This is the best laptop I've ever owned. Fast, reliable, and great battery life."
        ///     }
        /// 
        /// Note: ProductId is taken from the URL, not the request body.
        /// OrderId is automatically detected from user's purchase history.
        /// 
        /// Requirements:
        /// - Must be authenticated
        /// - Must have purchased the product (verified purchase)
        /// - Can only review each product once
        /// - Rating must be 1-5 stars
        /// - Comment must be 10-2000 characters
        /// 
        /// Response:
        /// - Returns the created review
        /// - Review will be pending approval by default
        /// </remarks>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<ProductReviewDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<string>), 400)]
        [ProducesResponseType(typeof(ApiResponse<string>), 500)]
        [Authorize]
        public async Task<IActionResult> CreateReview(int productId, [FromBody] CreateReviewDto dto)
        {
            // Set productId from URL route parameter
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
        /// <param name="productId">Product ID (for route consistency)</param>
        /// <param name="reviewId">Review ID to update</param>
        /// <param name="dto">Updated review data (rating and comment)</param>
        /// <remarks>
        /// User can only update their own reviews.
        /// 
        /// Example:
        ///     PUT /api/products/1/reviews/123
        ///     {
        ///       "rating": 4,
        ///       "comment": "Updated my review after using it for a month..."
        ///     }
        /// </remarks>
        [HttpPut("{reviewId:int}")]
        [ProducesResponseType(typeof(ApiResponse<ProductReviewDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<string>), 400)]
        [ProducesResponseType(typeof(ApiResponse<string>), 500)]
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
        /// <param name="productId">Product ID (for route consistency)</param>
        /// <param name="reviewId">Review ID to delete</param>
        /// <remarks>
        /// User can only delete their own reviews.
        /// 
        /// Example:
        ///     DELETE /api/products/1/reviews/123
        /// </remarks>
        [HttpDelete("{reviewId:int}")]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        [ProducesResponseType(typeof(ApiResponse<string>), 400)]
        [ProducesResponseType(typeof(ApiResponse<string>), 500)]
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
        /// <param name="productId">Product ID (for route consistency)</param>
        /// <param name="reviewId">Review ID to vote on</param>
        /// <param name="dto">Vote data (isHelpful: true/false)</param>
        /// <remarks>
        /// Mark a review as helpful or not helpful.
        /// Users can change their vote by voting again.
        /// 
        /// Example:
        ///     POST /api/products/1/reviews/123/vote
        ///     {
        ///       "isHelpful": true
        ///     }
        /// </remarks>
        [HttpPost("{reviewId:int}/vote")]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        [ProducesResponseType(typeof(ApiResponse<string>), 400)]
        [ProducesResponseType(typeof(ApiResponse<string>), 500)]
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
