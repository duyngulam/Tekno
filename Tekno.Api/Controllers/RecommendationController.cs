using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Tekno.Application.Recommendation;

namespace Tekno.Api.Controllers
{
    [ApiController]
    [Route("api/recommend")]
    public class RecommendationController : ControllerBase
    {
        private readonly IRecommendationService _recommendationService;

        public RecommendationController(IRecommendationService recommendationService)
        {
            _recommendationService = recommendationService;
        }

        [HttpGet("predict/{productId}")]
        public async Task<IActionResult> Predict(int productId)
        {
            var (category, brand) = await _recommendationService.PredictAsync(productId);
            return Ok(new { category, brand });
        }

        [HttpGet("cf/{userId}")]
        public async Task<IActionResult> RecommendCf(int userId, [FromQuery] int k = 10)
        {
            var list = await _recommendationService.RecommendCfAsync(userId, k);
            return Ok(new { recommendations = list });
        }

        [HttpGet("content/{userId}")]
        public async Task<IActionResult> RecommendContent(int userId, [FromQuery] int k = 10)
        {
            var list = await _recommendationService.RecommendContentAsync(userId, k);
            return Ok(new { recommendations = list });
        }

        [HttpGet("cf/products/{userId}")]
        public async Task<IActionResult> RecommendCfWithProducts(int userId, [FromQuery] int k = 10)
        {
            var products = await _recommendationService.RecommendCfWithProductsAsync(userId, k);
            return Ok(new { recommendations = products });
        }

        [HttpGet("content/products/{userId}")]
        public async Task<IActionResult> RecommendContentWithProducts(int userId, [FromQuery] int k = 10)
        {
            var products = await _recommendationService.RecommendContentWithProductsAsync(userId, k);
            return Ok(new { recommendations = products });
        }
    }
}