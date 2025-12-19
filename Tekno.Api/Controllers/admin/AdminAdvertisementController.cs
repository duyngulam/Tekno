using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Tekno.Api.Commons.Responses;
using Tekno.Application.Catalog.DTOs.Advertisement;
using Tekno.Application.Catalog.Services;

namespace Tekno.Api.Controllers.Admin
{
    /// <summary>
    /// Admin endpoints for managing product advertisement banners
    /// </summary>
    [ApiController]
    [Route("api/admin/advertisements")]
    //[Authorize(Roles = "Admin")]
    public class AdminAdvertisementController : ControllerBase
    {
        private readonly AdvertisementService _advertisementService;

        public AdminAdvertisementController(AdvertisementService advertisementService)
        {
            _advertisementService = advertisementService;
        }

        /// <summary>
        /// Get all advertisements (paginated)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] AdvertisementQueryDto query)
        {
            var result = await _advertisementService.GetPagedAsync(query);
            return Ok(ApiResponse<Application.Common.Paging.PagedResult<ProductAdvertisementDto>>.Ok(result));
        }

        /// <summary>
        /// Get advertisement by ID
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var advertisement = await _advertisementService.GetByIdAsync(id);

            if (advertisement == null)
                return NotFound(ApiResponse<ProductAdvertisementDto>.Fail("Advertisement not found"));

            return Ok(ApiResponse<ProductAdvertisementDto>.Ok(advertisement));
        }

        /// <summary>
        /// Create new advertisement banner
        /// </summary>
        /// <remarks>
        /// Simple product banner - just upload an image and link to a product
        /// 
        /// Sample request:
        /// 
        ///     POST /api/admin/advertisements
        ///     Content-Type: multipart/form-data
        ///     
        ///     image: [banner-file.jpg]
        ///     productId: 5
        ///     position: "HomeTop"
        ///     priority: 10
        ///     startDate: "2025-06-01"
        ///     endDate: "2025-08-31"
        ///     isActive: true
        /// 
        /// Positions: HomeTop, HomeMiddle, HomeBottom, CategoryTop, ProductSidebar
        /// Priority: 0-100 (higher = shown first)
        /// </remarks>
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CreateAdvertisementDto dto)
        {
            var advertisement = await _advertisementService.CreateAsync(dto);
            return CreatedAtAction(
                nameof(GetById),
                new { id = advertisement.Id },
                ApiResponse<ProductAdvertisementDto>.Ok(advertisement, "Advertisement created successfully"));
        }

        /// <summary>
        /// Update advertisement banner
        /// </summary>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromForm] UpdateAdvertisementDto dto)
        {
            var advertisement = await _advertisementService.UpdateAsync(id, dto);

            if (advertisement == null)
                return NotFound(ApiResponse<ProductAdvertisementDto>.Fail("Advertisement not found"));

            return Ok(ApiResponse<ProductAdvertisementDto>.Ok(advertisement, "Advertisement updated successfully"));
        }

        /// <summary>
        /// Delete advertisement banner
        /// </summary>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _advertisementService.DeleteAsync(id);

            if (!success)
                return NotFound(ApiResponse<bool>.Fail("Advertisement not found"));

            return Ok(ApiResponse<bool>.Ok(true, "Advertisement deleted successfully"));
        }

        /// <summary>
        /// Activate advertisement
        /// </summary>
        [HttpPatch("{id:int}/activate")]
        public async Task<IActionResult> Activate(int id)
        {
            var success = await _advertisementService.ActivateAsync(id);

            if (!success)
                return NotFound(ApiResponse<bool>.Fail("Advertisement not found"));

            return Ok(ApiResponse<bool>.Ok(true, "Advertisement activated"));
        }

        /// <summary>
        /// Deactivate advertisement
        /// </summary>
        [HttpPatch("{id:int}/deactivate")]
        public async Task<IActionResult> Deactivate(int id)
        {
            var success = await _advertisementService.DeactivateAsync(id);

            if (!success)
                return NotFound(ApiResponse<bool>.Fail("Advertisement not found"));

            return Ok(ApiResponse<bool>.Ok(true, "Advertisement deactivated"));
        }
    }
}
