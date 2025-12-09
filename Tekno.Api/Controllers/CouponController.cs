using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Tekno.Api.Common.Responses;
using Tekno.Application.Common.Paging;
using Tekno.Application.Promotion.DTOs;
using Tekno.Application.Promotion.Services;

namespace Tekno.Api.Controllers
{
    /// <summary>
    /// Public coupon endpoints for customers
    /// </summary>
    [ApiController]
    [Route("api/coupons")]
    public class CouponController : ControllerBase
    {
        private readonly CouponService _couponService;

        public CouponController(CouponService couponService)
        {
            _couponService = couponService;
        }

        /// <summary>
        /// Get paginated list of active coupons (public)
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetPaged(
            [FromQuery] string? search,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            // Force status to Active for public API
            var result = await _couponService.GetPagedCouponsAsync(
                search, 
                "Active", 
                null, 
                null, 
                page, 
                pageSize);

            // Filter to only show currently valid coupons
            var now = System.DateTime.UtcNow;
            var validCoupons = result.Data
                .Where(c => c.StartDate <= now && c.EndDate >= now && c.RemainingQuantity > 0)
                .ToList();

            var filteredResult = new PagedResult<CouponDto>(
                validCoupons, 
                validCoupons.Count, 
                page, 
                pageSize);

            return Ok(ApiResponse<PagedResult<CouponDto>>.Ok(filteredResult, "Coupons loaded successfully"));
        }

        /// <summary>
        /// Get all active and valid coupons (public) - kept for backward compatibility
        /// </summary>
        /// <remarks>
        /// Returns only coupons that are:
        /// - Currently active
        /// - Within valid date range
        /// - Have remaining quantity
        /// </remarks>
        [HttpGet("active")]
        [AllowAnonymous]
        public async Task<IActionResult> GetActiveCoupons()
        {
            var coupons = await _couponService.GetActiveCouponsAsync();
            return Ok(ApiResponse<System.Collections.Generic.List<CouponDto>>.Ok(
                coupons, 
                $"Found {coupons.Count} active coupon(s)"));
        }

        /// <summary>
        /// Get coupon details by code (public - for preview/validation)
        /// </summary>
        /// <param name="code">Coupon code (e.g., PHVC000003)</param>
        [HttpGet("{code}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByCode(string code)
        {
            var coupon = await _couponService.GetCouponByCodeAsync(code);
            if (coupon == null)
                return NotFound(ApiResponse<CouponDto>.Fail("Coupon code not found"));

            // Don't expose internal details to public
            return Ok(ApiResponse<CouponDto>.Ok(coupon));
        }

        /// <summary>
        /// Validate coupon for current cart
        /// </summary>
        /// <remarks>
        /// Validates if coupon can be applied to the current cart/order.
        /// 
        /// Sample request:
        /// 
        ///     POST /api/coupons/validate
        ///     {
        ///       "code": "PHVC000003",
        ///       "orderAmount": 500000,
        ///       "userId": 1,
        ///       "productIds": [1, 2, 3],
        ///       "categoryIds": [5, 8]
        ///     }
        /// 
        /// Response includes:
        /// - isValid: whether coupon can be applied
        /// - message: explanation (success or error reason)
        /// - discountAmount: how much will be saved
        /// </remarks>
        [HttpPost("validate")]
        [AllowAnonymous]
        public async Task<IActionResult> ValidateCoupon([FromBody] ValidateCouponDto dto)
        {
            // Optionally get userId from claims if authenticated
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out var userId))
            {
                dto.UserId = userId;
            }

            var result = await _couponService.ValidateCouponAsync(dto);
            
            if (!result.IsValid)
            {
                return BadRequest(ApiResponse<CouponValidationResult>.Fail(
                    result.Message, 
                    result));
            }

            return Ok(ApiResponse<CouponValidationResult>.Ok(
                result, 
                result.Message));
        }

        /// <summary>
        /// Check if coupon code exists (quick check without full validation)
        /// </summary>
        /// <param name="code">Coupon code to check</param>
        [HttpGet("check/{code}")]
        [AllowAnonymous]
        public async Task<IActionResult> CheckCouponExists(string code)
        {
            var coupon = await _couponService.GetCouponByCodeAsync(code);
            
            if (coupon == null)
            {
                return Ok(ApiResponse<object>.Ok(new { exists = false, message = "Coupon code not found" }));
            }

            var isUsable = coupon.Status == "Active" 
                && coupon.RemainingQuantity > 0 
                && System.DateTime.UtcNow >= coupon.StartDate 
                && System.DateTime.UtcNow <= coupon.EndDate;

            return Ok(ApiResponse<object>.Ok(new 
            { 
                exists = true,
                usable = isUsable,
                name = coupon.Name,
                type = coupon.Type,
                value = coupon.Value,
                minPurchaseAmount = coupon.MinPurchaseAmount,
                message = isUsable 
                    ? "Coupon is available" 
                    : "Coupon exists but cannot be used (inactive, expired, or out of stock)"
            }));
        }

        /// <summary>
        /// Get user's coupon usage history (requires authentication)
        /// </summary>
        [HttpGet("my-usage")]
        [Authorize]
        public async Task<IActionResult> GetMyUsage(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized(ApiResponse<object>.Fail("User not authenticated"));

            if (!int.TryParse(userIdClaim.Value, out var userId))
                return BadRequest(ApiResponse<object>.Fail("Invalid user ID"));

            // Note: You may want to add a GetUserCouponUsageAsync method to the service
            // For now, return a placeholder message
            return Ok(ApiResponse<object>.Ok(new 
            { 
                message = "User coupon usage history endpoint - implementation pending",
                userId = userId,
                page = page,
                pageSize = pageSize
            }));
        }

        /// <summary>
        /// Get available coupons for a specific product (public)
        /// </summary>
        /// <param name="productId">Product ID</param>
        [HttpGet("for-product/{productId:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCouponsForProduct(int productId)
        {
            var allActiveCoupons = await _couponService.GetActiveCouponsAsync();
            
            // Filter coupons applicable to this product
            var applicableCoupons = allActiveCoupons
                .Where(c => 
                    c.ApplicableProductIds.Count == 0 || // Universal coupons
                    c.ApplicableProductIds.Contains(productId)) // Product-specific
                .ToList();

            return Ok(ApiResponse<System.Collections.Generic.List<CouponDto>>.Ok(
                applicableCoupons,
                $"Found {applicableCoupons.Count} applicable coupon(s) for this product"));
        }

        /// <summary>
        /// Get available coupons for a specific category (public)
        /// </summary>
        /// <param name="categoryId">Category ID</param>
        [HttpGet("for-category/{categoryId:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCouponsForCategory(int categoryId)
        {
            var allActiveCoupons = await _couponService.GetActiveCouponsAsync();
            
            // Filter coupons applicable to this category
            var applicableCoupons = allActiveCoupons
                .Where(c => 
                    c.ApplicableCategoryIds.Count == 0 || // Universal coupons
                    c.ApplicableCategoryIds.Contains(categoryId)) // Category-specific
                .ToList();

            return Ok(ApiResponse<System.Collections.Generic.List<CouponDto>>.Ok(
                applicableCoupons,
                $"Found {applicableCoupons.Count} applicable coupon(s) for this category"));
        }
    }
}
