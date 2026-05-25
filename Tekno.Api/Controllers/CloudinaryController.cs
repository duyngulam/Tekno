using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Tekno.Api.Models.Cloudinary;
using Tekno.Application.Common.Media.Services;

namespace Tekno.Api.Controllers
{
    [ApiController]
    [Route("api/cloudinary")]
    public class CloudinaryController : ControllerBase
    {
        private readonly MediaService _mediaService;

        public CloudinaryController(MediaService mediaService)
        {
            _mediaService = mediaService;
        }

        /// <summary>
        /// Upload category icon
        /// </summary>
        [HttpPost("category-icon")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> UploadCategoryIcon([FromForm] FileUploadRequest request)
        {
            if (request.File == null || request.File.Length == 0)
                return BadRequest("No file uploaded.");

            var url = await _mediaService.UploadCategoryIconAsync(request.File);
            return Ok(new { Url = url });
        }

        /// <summary>
        /// Upload product image
        /// </summary>
        [HttpPost("product-image")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> UploadProductImage([FromForm] FileUploadRequest request)
        {
            if (request.File == null || request.File.Length == 0)
                return BadRequest("No file uploaded.");

            var url = await _mediaService.UploadProductImageAsync(request.File);
            return Ok(new { Url = url });
        }
        [HttpDelete]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteImage([FromQuery] string Url)
        {
            if (string.IsNullOrEmpty(Url))
                return BadRequest("PublicId is required.");
            var result = await _mediaService.DeleteImageAsync(Url);
            if (result)
                return Ok(new { Message = "Image deleted successfully." });
            else
                return NotFound(new { Message = "Image not found." });
        }
    }
}
