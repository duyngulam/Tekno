using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Tekno.Api.Commons.Responses;
using Tekno.Infrastructure.Seeding;

namespace Tekno.Api.Controllers.Admin
{
    /// <summary>
    /// Admin endpoints for seeding training data
    /// </summary>
    [ApiController]
    [Route("api/admin/training-seed")]
    [Authorize(Roles = "Admin")]
    public class TrainingSeedController : ControllerBase
    {
        private readonly TrainingUserSeeder _seeder;
        private readonly ILogger<TrainingSeedController> _logger;

        public TrainingSeedController(
            TrainingUserSeeder seeder,
            ILogger<TrainingSeedController> logger)
        {
            _seeder = seeder;
            _logger = logger;
        }

        /// <summary>
        /// Seed training users
        /// </summary>
        /// <remarks>
        /// ## Description
        /// Seeds 30 training users for recommendation model:
        /// 
        /// **30 Training Users:**
        /// - Email: training1@tekno.com to training30@tekno.com
        /// - Password: training1 to training30 (SHA256 hash)
        /// - Role: Customer
        /// - Phone: 0900000001 to 0900000030
        /// - Fullname: Training User 1 to Training User 30
        /// 
        /// ## Safety
        /// - Idempotent: Safe to run multiple times
        /// - Skips existing users
        /// - Only creates users, no other data
        /// 
        /// ## Next Steps
        /// After running this endpoint:
        /// 1. Run the SQL script to create training products with variants
        /// 2. Run another SQL script to create orders with recommendation patterns
        /// 
        /// ## Example Response
        /// ```json
        /// {
        ///   "success": true,
        ///   "message": "Training users seeded successfully...",
        ///   "data": {
        ///     "success": true,
        ///     "message": "Seeding completed successfully",
        ///     "usersCreated": 30
        ///   }
        /// }
        /// ```
        /// </remarks>
        /// <response code="200">Successfully seeded users</response>
        /// <response code="500">Server error during seeding</response>
        [HttpPost("users")]
        [ProducesResponseType(typeof(ApiResponse<SeedResult>), 200)]
        [ProducesResponseType(typeof(ApiResponse<string>), 500)]
        public async Task<IActionResult> SeedUsers()
        {
            try
            {
                _logger.LogInformation("Admin requesting training users seed");

                var result = await _seeder.SeedAsync();

                if (!result.Success)
                {
                    return StatusCode(500, ApiResponse<SeedResult>.Fail(result.Message));
                }

                return Ok(ApiResponse<SeedResult>.Ok(
                    result,
                    "Training users seeded successfully. " +
                    "Now run the SQL script to create products and orders."));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to seed training users");
                return StatusCode(500, ApiResponse<string>.Fail($"Failed: {ex.Message}"));
            }
        }
    }
}
