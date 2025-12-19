using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Tekno.Api.Commons.Responses;
using Tekno.Application.Location.DTOs;
using Tekno.Application.Location.Services;

namespace Tekno.Api.Controllers.Admin
{
    /// <summary>
    /// Admin endpoints for managing Vietnam administrative divisions (provinces, districts, wards)
    /// </summary>
    [ApiController]
    [Route("api/admin/locations")]
    //[Authorize(Roles = "Admin")]
    public class AdminLocationController : ControllerBase
    {
        private readonly LocationService _locationService;
        private readonly ILogger<AdminLocationController> _logger;

        public AdminLocationController(LocationService locationService, ILogger<AdminLocationController> logger)
        {
            _locationService = locationService;
            _logger = logger;
        }

        /// <summary>
        /// Import provinces, districts, and wards from JSON file
        /// </summary>
        /// <remarks>
        /// This endpoint imports data from the auto-downloaded JSON file located at:
        /// `data/vietnam-divisions.json`
        /// 
        /// The background service fetches this data once on first startup from:
        /// https://provinces.open-api.vn/api/?depth=3
        /// 
        /// **One-time operation**: After the first import, duplicate entries are skipped.
        /// 
        /// Sample response:
        /// 
        ///     {
        ///       "success": true,
        ///       "data": {
        ///         "provincesImported": 63,
        ///         "districtsImported": 705,
        ///         "wardsImported": 10599,
        ///         "message": "Successfully imported..."
        ///       }
        ///     }
        /// 
        /// </remarks>
        [HttpPost("import")]
        public async Task<IActionResult> ImportFromJson()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "data", "vietnam-divisions.json");

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound(ApiResponse<ImportResultDto>.Fail(
                    $"JSON file not found at: {filePath}. Please wait for background service to fetch data or manually place the file."));
            }

            try
            {
                var result = await _locationService.ImportFromJsonFileAsync(filePath);
                return Ok(ApiResponse<ImportResultDto>.Ok(result, "Import completed successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to import locations from JSON");
                return StatusCode(500, ApiResponse<ImportResultDto>.Fail($"Import failed: {ex.Message}"));
            }
        }
    }
}
