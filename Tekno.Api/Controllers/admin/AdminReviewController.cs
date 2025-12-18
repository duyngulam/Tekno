using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using Tekno.Api.Commons.Responses;
using Tekno.Application.Review.DTOs;
using Tekno.Application.Review.Services;

namespace Tekno.Api.Controllers.Admin
{
    /// <summary>
    /// Admin endpoints for managing product reviews
    /// </summary>
    [ApiController]
    [Route("api/admin/reviews")]
    //[Authorize(Roles = "Admin")]
    public class AdminReviewController : ControllerBase
    {
        private readonly ReviewService _reviewService;

        public AdminReviewController(ReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        /// <summary>
        /// Get all reviews for a product (including pending/rejected)
        /// </summary>
        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetProductReviews(
            int productId,
            [FromQuery] string? status = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var reviews = await _reviewService.GetProductReviewsAsync(productId, null, page, pageSize);
            return Ok(ApiResponse<ReviewListDto>.Ok(reviews));
        }

        /// <summary>
        /// Approve a review
        /// </summary>
        [HttpPatch("{reviewId:int}/approve")]
        public async Task<IActionResult> ApproveReview(int reviewId)
        {
            var adminId = GetCurrentUserId();
            var review = await _reviewService.ApproveReviewAsync(reviewId, adminId);

            if (review == null)
                return NotFound(ApiResponse<ProductReviewDto>.Fail("Review not found"));

            return Ok(ApiResponse<ProductReviewDto>.Ok(review, "Review approved successfully"));
        }

        /// <summary>
        /// Reject a review
        /// </summary>
        [HttpPatch("{reviewId:int}/reject")]
        public async Task<IActionResult> RejectReview(int reviewId)
        {
            var adminId = GetCurrentUserId();
            var review = await _reviewService.RejectReviewAsync(reviewId, adminId);

            if (review == null)
                return NotFound(ApiResponse<ProductReviewDto>.Fail("Review not found"));

            return Ok(ApiResponse<ProductReviewDto>.Ok(review, "Review rejected successfully"));
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
