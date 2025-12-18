using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Tekno.Api.Commons.Responses;
using Tekno.Application.Common.Paging;
using Tekno.Application.Promotion.DTOs;
using Tekno.Application.Promotion.Services;

namespace Tekno.Api.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/coupons")]
    //[Authorize(Roles = "Admin")]
    public class AdminCouponController : ControllerBase
    {
        private readonly CouponService _couponService;

        public AdminCouponController(CouponService couponService)
        {
            _couponService = couponService;
        }

        /// <summary>
        /// Get paginated list of coupons with filtering
        /// </summary>
        /// <param name="search">Search by code or name</param>
        /// <param name="status">Filter by status: Active, Inactive, Expired</param>
        /// <param name="startDate">Filter by start date (from)</param>
        /// <param name="endDate">Filter by end date (to)</param>
        /// <param name="page">Page number (default: 1)</param>
        /// <param name="pageSize">Items per page (default: 20, max: 100)</param>
        [HttpGet]
        public async Task<IActionResult> GetPaged(
            [FromQuery] string? search,
            [FromQuery] string? status,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _couponService.GetPagedCouponsAsync(
                search, status, startDate, endDate, page, pageSize);
            return Ok(ApiResponse<PagedResult<CouponDto>>.Ok(result));
        }

        /// <summary>
        /// Get coupon by ID with full details
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var coupon = await _couponService.GetCouponByIdAsync(id);
            if (coupon == null)
                return NotFound(ApiResponse<CouponDto>.Fail("Coupon not found"));

            return Ok(ApiResponse<CouponDto>.Ok(coupon));
        }

        /// <summary>
        /// Create a new coupon
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /api/admin/coupons
        ///     {
        ///       "code": "PHVC000004",
        ///       "name": "Black Friday",
        ///       "type": "Percentage",
        ///       "value": 20,
        ///       "quantity": 100,
        ///       "maxUsagePerUser": 1,
        ///       "minPurchaseAmount": 1000000,
        ///       "maxDiscountAmount": 500000,
        ///       "startDate": "2025-11-25T00:00:00Z",
        ///       "endDate": "2025-11-30T23:59:59Z",
        ///       "note": "Black Friday sale",
        ///       "applicableCategoryIds": [1, 2],
        ///       "applicableProductIds": []
        ///     }
        /// 
        /// </remarks>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCouponDto dto)
        {
            var coupon = await _couponService.CreateCouponAsync(dto);
            return CreatedAtAction(
                nameof(GetById), 
                new { id = coupon.Id }, 
                ApiResponse<CouponDto>.Ok(coupon, "Coupon created successfully"));
        }

        /// <summary>
        /// Update existing coupon
        /// </summary>
        /// <remarks>
        /// Note: Cannot update coupon code. Create a new coupon if code needs to change.
        /// </remarks>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCouponDto dto)
        {
            var coupon = await _couponService.UpdateCouponAsync(id, dto);
            if (coupon == null)
                return NotFound(ApiResponse<CouponDto>.Fail("Coupon not found"));

            return Ok(ApiResponse<CouponDto>.Ok(coupon, "Coupon updated successfully"));
        }

        /// <summary>
        /// Delete coupon
        /// </summary>
        /// <remarks>
        /// Warning: This will also delete all usage history for this coupon.
        /// Consider deactivating instead of deleting if you need to preserve history.
        /// </remarks>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _couponService.DeleteCouponAsync(id);
            if (!success)
                return NotFound(ApiResponse<bool>.Fail("Coupon not found"));

            return Ok(ApiResponse<bool>.Ok(true, "Coupon deleted successfully"));
        }

        /// <summary>
        /// Get usage history for a specific coupon
        /// </summary>
        /// <param name="id">Coupon ID</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Items per page</param>
        [HttpGet("{id:int}/usage")]
        public async Task<IActionResult> GetUsageHistory(
            int id,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var history = await _couponService.GetUsageHistoryAsync(id, page, pageSize);
            return Ok(ApiResponse<System.Collections.Generic.List<CouponUsageDto>>.Ok(history));
        }

        /// <summary>
        /// Activate a coupon
        /// </summary>
        [HttpPatch("{id:int}/activate")]
        public async Task<IActionResult> Activate(int id)
        {
            var coupon = await _couponService.GetCouponByIdAsync(id);
            if (coupon == null)
                return NotFound(ApiResponse<CouponDto>.Fail("Coupon not found"));

            // Note: You may want to add an ActivateCoupon method to the service
            return Ok(ApiResponse<string>.Ok("Coupon activated", "Use PUT endpoint to update status"));
        }

        /// <summary>
        /// Deactivate a coupon
        /// </summary>
        [HttpPatch("{id:int}/deactivate")]
        public async Task<IActionResult> Deactivate(int id)
        {
            var coupon = await _couponService.GetCouponByIdAsync(id);
            if (coupon == null)
                return NotFound(ApiResponse<CouponDto>.Fail("Coupon not found"));

            // Note: You may want to add a DeactivateCoupon method to the service
            return Ok(ApiResponse<string>.Ok("Coupon deactivated", "Use PUT endpoint to update status"));
        }

        /// <summary>
        /// Get coupon statistics/analytics
        /// </summary>
        [HttpGet("{id:int}/statistics")]
        public async Task<IActionResult> GetStatistics(int id)
        {
            var coupon = await _couponService.GetCouponByIdAsync(id);
            if (coupon == null)
                return NotFound(ApiResponse<CouponDto>.Fail("Coupon not found"));

            var stats = new
            {
                coupon.Code,
                coupon.Name,
                TotalAvailable = coupon.Quantity,
                UsedCount = coupon.UsedCount,
                RemainingCount = coupon.RemainingQuantity,
                UsageRate = coupon.Quantity > 0 
                    ? Math.Round((double)coupon.UsedCount / coupon.Quantity * 100, 2) 
                    : 0,
                Status = coupon.Status,
                IsActive = coupon.Status == "Active",
                IsExpired = DateTime.UtcNow > coupon.EndDate || DateTime.UtcNow < coupon.StartDate,
                DaysRemaining = (coupon.EndDate - DateTime.UtcNow).Days
            };

            return Ok(ApiResponse<object>.Ok(stats));
        }
    }
}
