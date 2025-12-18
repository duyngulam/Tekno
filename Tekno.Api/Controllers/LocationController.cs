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
        /// Get all provinces
        /// </summary>
        /// <remarks>
        /// Returns complete list of all provinces in Vietnam.
        /// 
        /// Example response:
        /// 
        ///     [
        ///       {
        ///         "code": 1,
        ///         "name": "Thành ph? Hà N?i",
        ///         "codename": "thanh_pho_ha_noi",
        ///         "divisionType": "thành ph? trung ??ng",
        ///         "phoneCode": 24
        ///       }
        ///     ]
        /// 
        /// </remarks>
        [HttpGet("provinces")]
        public async Task<IActionResult> GetProvinces()
        {
            var provinces = await _locationService.GetAllProvincesAsync();
            return Ok(ApiResponse<List<ProvinceDto>>.Ok(provinces));
        }

        /// <summary>
        /// Search provinces by keyword
        /// </summary>
        /// <param name="keyword">Search keyword (searches in name and codename)</param>
        [HttpGet("provinces/search")]
        public async Task<IActionResult> SearchProvinces([FromQuery] string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return BadRequest(ApiResponse<List<ProvinceDto>>.Fail("Keyword is required"));

            var provinces = await _locationService.SearchProvincesAsync(keyword);
            return Ok(ApiResponse<List<ProvinceDto>>.Ok(provinces));
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
