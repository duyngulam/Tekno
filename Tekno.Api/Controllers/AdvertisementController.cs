using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Tekno.Api.Common.Responses;
using Tekno.Application.Catalog.DTOs.Advertisement;
using Tekno.Application.Catalog.Services;

namespace Tekno.Api.Controllers
{
    /// <summary>
    /// Public endpoints for viewing product advertisement banners
    /// </summary>
    [ApiController]
    [Route("api/advertisements")]
    public class AdvertisementController : ControllerBase
    {
        private readonly AdvertisementService _advertisementService;

        public AdvertisementController(AdvertisementService advertisementService)
        {
            _advertisementService = advertisementService;
        }

        /// <summary>
        /// Get all currently active advertisement banners
        /// </summary>
        [HttpGet("active")]
        public async Task<IActionResult> GetActive()
        {
            var advertisements = await _advertisementService.GetCurrentlyActiveAsync();
            return Ok(ApiResponse<System.Collections.Generic.List<ProductAdvertisementDto>>.Ok(advertisements));
        }

        /// <summary>
        /// Get advertisement banners by position
        /// </summary>
        /// <param name="position">Position: HomeTop, HomeMiddle, HomeBottom, CategoryTop, ProductSidebar</param>
        /// <remarks>
        /// Returns only currently active banners for the specified position,
        /// sorted by priority (highest first)
        /// 
        /// Examples:
        /// - GET /api/advertisements/position/HomeTop - Homepage hero banners
        /// - GET /api/advertisements/position/CategoryTop - Category page banners
        /// - GET /api/advertisements/position/ProductSidebar - Product detail sidebar
        /// </remarks>
        [HttpGet("position/{position}")]
        public async Task<IActionResult> GetByPosition(string position)
        {
            var advertisements = await _advertisementService.GetByPositionAsync(position);
            return Ok(ApiResponse<System.Collections.Generic.List<ProductAdvertisementDto>>.Ok(advertisements));
        }
    }
}
