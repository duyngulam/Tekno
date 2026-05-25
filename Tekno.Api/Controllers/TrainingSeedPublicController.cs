using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Tekno.Api.Commons.Responses;
using Tekno.Infrastructure.Seeding;

namespace Tekno.Api.Controllers
{
    [ApiController]
    [Route("api/training-seed")]
    [AllowAnonymous]
    public class TrainingSeedPublicController : ControllerBase
    {
        private readonly TrainingProductImportService _productImporter;
        private readonly IConfiguration _configuration;
        private readonly ILogger<TrainingSeedPublicController> _logger;
        private readonly IWebHostEnvironment _environment;

        public TrainingSeedPublicController(
            TrainingProductImportService productImporter,
            IConfiguration configuration,
            ILogger<TrainingSeedPublicController> logger,
            IWebHostEnvironment environment)
        {
            _productImporter = productImporter;
            _configuration = configuration;
            _logger = logger;
            _environment = environment;
        }

        [HttpPost("rebuild")]
        [ProducesResponseType(typeof(ApiResponse<TrainingSeedRunResult>), 200)]
        [ProducesResponseType(typeof(ApiResponse<string>), 500)]
        public async Task<IActionResult> RebuildAndRun()
        {
            try
            {
                var trainingRoot = GetTrainingRoot();
                var outputsDir = Path.Combine(trainingRoot, "outputs_run_2000x500_v2");

                var importCsvPath = Path.Combine(outputsDir, "import_products.csv");
                var importResult = await _productImporter.ImportFromCsvAsync(importCsvPath);

                var result = new TrainingSeedRunResult
                {
                    ProductsCreated = importResult.Created,
                    ProductsSkipped = importResult.Skipped,
                };

                return Ok(ApiResponse<TrainingSeedRunResult>.Ok(result, "Training seed + pipeline completed"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Training seed rebuild failed");
                return StatusCode(500, ApiResponse<string>.Fail($"Failed: {ex.Message}"));
            }
        }

        private string GetTrainingRoot()
        {
            var configured = _configuration["TRAINING_ROOT"];
            if (!string.IsNullOrWhiteSpace(configured))
            {
                return Path.GetFullPath(configured);
            }

            var contentRoot = _environment.ContentRootPath;
            return Path.GetFullPath(Path.Combine(contentRoot, "..", "Trainning"));
        }

    }

    public class TrainingSeedRunResult
    {
        public int ProductsCreated { get; set; }
        public int ProductsSkipped { get; set; }
    }
}
