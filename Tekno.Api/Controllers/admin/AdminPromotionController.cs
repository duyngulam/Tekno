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
    /// <summary>
    /// Admin endpoints for managing bulk promotions (automatic discounts)
    /// Unlike coupons, promotions are automatically applied to products/categories
    /// </summary>
    [ApiController]
    [Route("api/admin/promotions")]
    //[Authorize(Roles = "Admin")]
    public class AdminPromotionController : ControllerBase
    {
        private readonly PromotionService _promotionService;

        public AdminPromotionController(PromotionService promotionService)
        {
            _promotionService = promotionService;
        }

        /// <summary>
        /// Get paginated list of promotions with filtering
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<PromotionDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<string>), 400)]
        [ProducesResponseType(typeof(ApiResponse<string>), 500)]
        public async Task<IActionResult> GetPaged(
            [FromQuery] string? search,
            [FromQuery] string? status,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _promotionService.GetPagedPromotionsAsync(
                search, status, startDate, endDate, page, pageSize);
            return Ok(ApiResponse<PagedResult<PromotionDto>>.Ok(result));
        }

        /// <summary>
        /// Get promotion by ID
        /// </summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<PromotionDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<string>), 400)]
        [ProducesResponseType(typeof(ApiResponse<string>), 500)]
        public async Task<IActionResult> GetById(int id)
        {
            var promotion = await _promotionService.GetPromotionByIdAsync(id);
            if (promotion == null)
                return NotFound(ApiResponse<PromotionDto>.Fail("Promotion not found"));

            return Ok(ApiResponse<PromotionDto>.Ok(promotion));
        }

        /// <summary>
        /// Get all active promotions
        /// </summary>
        [HttpGet("active")]
        [ProducesResponseType(typeof(ApiResponse<System.Collections.Generic.List<PromotionDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<string>), 400)]
        [ProducesResponseType(typeof(ApiResponse<string>), 500)]
        public async Task<IActionResult> GetActive()
        {
            var promotions = await _promotionService.GetActivePromotionsAsync();
            return Ok(ApiResponse<System.Collections.Generic.List<PromotionDto>>.Ok(promotions));
        }

        /// <summary>
        /// Create a new promotion
        /// </summary>
        /// <remarks>
        /// Sample request for category-wide promotion:
        /// 
        ///     POST /api/admin/promotions
        ///     {
        ///       "name": "Smartphone Week",
        ///       "description": "10% off all smartphones",
        ///       "type": "Percentage",
        ///       "value": 10,
        ///       "startDate": "2025-01-15T00:00:00Z",
        ///       "endDate": "2025-01-22T23:59:59Z",
        ///       "priority": 10,
        ///       "stackableWithCoupons": true,
        ///       "applicableCategoryIds": [1],
        ///       "applicableProductIds": []
        ///     }
        /// 
        /// If startDate is in the future, status will be "Scheduled"
        /// If startDate is now or past, promotion is immediately "Active" and applied to products
        /// </remarks>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<PromotionDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<string>), 400)]
        [ProducesResponseType(typeof(ApiResponse<string>), 500)]
        public async Task<IActionResult> Create([FromBody] CreatePromotionDto dto)
        {
            var promotion = await _promotionService.CreatePromotionAsync(dto);
            return CreatedAtAction(
                nameof(GetById),
                new { id = promotion.Id },
                ApiResponse<PromotionDto>.Ok(promotion, "Promotion created successfully"));
        }

        /// <summary>
        /// Update existing promotion
        /// </summary>
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<PromotionDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<string>), 400)]
        [ProducesResponseType(typeof(ApiResponse<string>), 500)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdatePromotionDto dto)
        {
            var promotion = await _promotionService.UpdatePromotionAsync(id, dto);
            if (promotion == null)
                return NotFound(ApiResponse<PromotionDto>.Fail("Promotion not found"));

            return Ok(ApiResponse<PromotionDto>.Ok(promotion, "Promotion updated successfully"));
        }

        /// <summary>
        /// Delete promotion
        /// </summary>
        /// <remarks>
        /// Warning: This will remove discounts from all affected products
        /// </remarks>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        [ProducesResponseType(typeof(ApiResponse<string>), 400)]
        [ProducesResponseType(typeof(ApiResponse<string>), 500)]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _promotionService.DeletePromotionAsync(id);
            if (!success)
                return NotFound(ApiResponse<bool>.Fail("Promotion not found"));

            return Ok(ApiResponse<bool>.Ok(true, "Promotion deleted successfully"));
        }

        /// <summary>
        /// Manually activate a promotion
        /// </summary>
        /// <remarks>
        /// Activates the promotion and applies discounts to products immediately
        /// </remarks>
        [HttpPatch("{id:int}/activate")]
        [ProducesResponseType(typeof(ApiResponse<PromotionDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<string>), 400)]
        [ProducesResponseType(typeof(ApiResponse<string>), 500)]
        public async Task<IActionResult> Activate(int id)
        {
            var promotion = await _promotionService.ActivatePromotionAsync(id);
            if (promotion == null)
                return NotFound(ApiResponse<PromotionDto>.Fail("Promotion not found"));

            return Ok(ApiResponse<PromotionDto>.Ok(promotion, "Promotion activated and applied to products"));
        }

        /// <summary>
        /// Pause a promotion
        /// </summary>
        /// <remarks>
        /// Pauses the promotion and removes discounts from products
        /// </remarks>
        [HttpPatch("{id:int}/pause")]
        [ProducesResponseType(typeof(ApiResponse<PromotionDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<string>), 400)]
        [ProducesResponseType(typeof(ApiResponse<string>), 500)]
        public async Task<IActionResult> Pause(int id)
        {
            var promotion = await _promotionService.PausePromotionAsync(id);
            if (promotion == null)
                return NotFound(ApiResponse<PromotionDto>.Fail("Promotion not found"));

            return Ok(ApiResponse<PromotionDto>.Ok(promotion, "Promotion paused and discounts removed"));
        }
    }
}
