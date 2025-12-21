using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tekno.Api.Commons.Responses;
using Tekno.Application.Location.DTOs;
using Tekno.Application.Location.Services;

namespace Tekno.Api.Controllers
{
    /// <summary>
    /// Public endpoints for querying Vietnam administrative divisions
    /// </summary>
    [ApiController]
    [Route("api/locations")]
    public class LocationController : ControllerBase
    {
        private readonly LocationService _locationService;

        public LocationController(LocationService locationService)
        {
            _locationService = locationService;
        }

        /// <summary>
        /// Get provinces or search provinces by keyword
        /// </summary>
        /// <remarks>
        /// If `keyword` query parameter is provided, performs a search against province name and codename.
        /// Otherwise returns the full list of provinces.
        /// 
        /// Examples:
        ///     GET /api/locations/provinces
        ///     GET /api/locations/provinces?keyword=Ha+Noi
        /// 
        /// </remarks>
        [HttpGet("provinces")]
        public async Task<IActionResult> GetProvinces([FromQuery] string? keyword)
        {
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var provinces = await _locationService.SearchProvincesAsync(keyword);
                return Ok(ApiResponse<List<ProvinceDto>>.Ok(provinces));
            }

            var all = await _locationService.GetAllProvincesAsync();
            return Ok(ApiResponse<List<ProvinceDto>>.Ok(all));
        }

        /// <summary>
        /// Get all districts of a province
        /// </summary>
        /// <param name="provinceCode">Province code (e.g., 1 for Hà N?i)</param>
        /// <remarks>
        /// Returns all districts/qu?n/huy?n belonging to the specified province.
        /// 
        /// Example: GET /api/locations/provinces/1/districts
        /// 
        /// Returns districts of Hà N?i (Ba ?ình, Hoàn Ki?m, etc.)
        /// </remarks>
        [HttpGet("provinces/{provinceCode:int}/districts")]
        public async Task<IActionResult> GetDistrictsByProvince(int provinceCode)
        {
            var districts = await _locationService.GetDistrictsByProvinceAsync(provinceCode);
            return Ok(ApiResponse<List<DistrictDto>>.Ok(districts));
        }

        /// <summary>
        /// Get all wards of a district
        /// </summary>
        /// <param name="districtCode">District code</param>
        /// <remarks>
        /// Returns all wards/ph??ng/xã/th? tr?n belonging to the specified district.
        /// 
        /// Example: GET /api/locations/districts/1/wards
        /// 
        /// Returns wards of qu?n Ba ?ình
        /// </remarks>
        [HttpGet("districts/{districtCode:int}/wards")]
        public async Task<IActionResult> GetWardsByDistrict(int districtCode)
        {
            var wards = await _locationService.GetWardsByDistrictAsync(districtCode);
            return Ok(ApiResponse<List<WardDto>>.Ok(wards));
        }
    }
}
