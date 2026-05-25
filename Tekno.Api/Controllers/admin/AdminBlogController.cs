using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using Tekno.Api.Commons.Responses;
using Tekno.Application.Blog.DTOs;
using Tekno.Application.Blog.Services;

namespace Tekno.Api.Controllers.Admin
{
    /// <summary>
    /// Admin endpoints for managing blog posts
    /// </summary>
    [ApiController]
    [Route("api/admin/blog")]
    //[Authorize(Roles = "Admin")]
    public class AdminBlogController : ControllerBase
    {
        private readonly BlogPostService _blogPostService;

        public AdminBlogController(BlogPostService blogPostService)
        {
            _blogPostService = blogPostService;
        }

        /// <summary>
        /// Get all blog posts (paginated)
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<Application.Common.Paging.PagedResult<BlogPostSummaryDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<string>), 400)]
        [ProducesResponseType(typeof(ApiResponse<string>), 500)]
        public async Task<IActionResult> GetAll([FromQuery] BlogPostQueryDto query)
        {
            var result = await _blogPostService.GetPagedAsync(query);
            return Ok(ApiResponse<Application.Common.Paging.PagedResult<BlogPostSummaryDto>>.Ok(result));
        }

        /// <summary>
        /// Get blog post by ID
        /// </summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<BlogPostDetailDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<string>), 400)]
        [ProducesResponseType(typeof(ApiResponse<string>), 500)]
        public async Task<IActionResult> GetById(int id)
        {
            var blogPost = await _blogPostService.GetByIdAsync(id);

            if (blogPost == null)
                return NotFound(ApiResponse<BlogPostDetailDto>.Fail("Blog post not found"));

            return Ok(ApiResponse<BlogPostDetailDto>.Ok(blogPost));
        }

        /// <summary>
        /// Create new blog post
        /// </summary>
        /// <remarks>
        /// Create a blog post for product introduction, review, or analysis
        /// 
        /// Sample request:
        /// 
        ///     POST /api/admin/blog
        ///     Content-Type: multipart/form-data
        ///     
        ///     title: "Introducing the New Dell XPS 13"
        ///     slug: "introducing-dell-xps-13"
        ///     summary: "A comprehensive look at Dell's latest ultrabook..."
        ///     content: "&lt;h2&gt;Overview&lt;/h2&gt;&lt;p&gt;The Dell XPS 13...&lt;/p&gt;"
        ///     featuredImage: [image-file.jpg]
        ///     tags: ["new-product", "laptop", "review"]
        ///     relatedProductIds: [5, 12]
        ///     publishImmediately: true
        /// 
        /// </remarks>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<BlogPostDetailDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<string>), 400)]
        [ProducesResponseType(typeof(ApiResponse<string>), 500)]
        public async Task<IActionResult> Create([FromForm] CreateBlogPostDto dto)
        {
            // Get author ID from JWT token
            var authorIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(authorIdClaim) || !int.TryParse(authorIdClaim, out var authorId))
            {
                return Unauthorized(ApiResponse<BlogPostDetailDto>.Fail("User not authenticated"));
            }

            var blogPost = await _blogPostService.CreateAsync(dto, authorId);
            
            return CreatedAtAction(
                nameof(GetById),
                new { id = blogPost.Id },
                ApiResponse<BlogPostDetailDto>.Ok(blogPost, "Blog post created successfully"));
        }

        /// <summary>
        /// Update blog post
        /// </summary>
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<BlogPostDetailDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<string>), 400)]
        [ProducesResponseType(typeof(ApiResponse<string>), 500)]
        public async Task<IActionResult> Update(int id, [FromForm] UpdateBlogPostDto dto)
        {
            var blogPost = await _blogPostService.UpdateAsync(id, dto);

            if (blogPost == null)
                return NotFound(ApiResponse<BlogPostDetailDto>.Fail("Blog post not found"));

            return Ok(ApiResponse<BlogPostDetailDto>.Ok(blogPost, "Blog post updated successfully"));
        }

        /// <summary>
        /// Delete blog post
        /// </summary>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        [ProducesResponseType(typeof(ApiResponse<string>), 400)]
        [ProducesResponseType(typeof(ApiResponse<string>), 500)]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _blogPostService.DeleteAsync(id);

            if (!success)
                return NotFound(ApiResponse<bool>.Fail("Blog post not found"));

            return Ok(ApiResponse<bool>.Ok(true, "Blog post deleted successfully"));
        }

        /// <summary>
        /// Publish blog post
        /// </summary>
        [HttpPatch("{id:int}/publish")]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        [ProducesResponseType(typeof(ApiResponse<string>), 400)]
        [ProducesResponseType(typeof(ApiResponse<string>), 500)]
        public async Task<IActionResult> Publish(int id)
        {
            var success = await _blogPostService.PublishAsync(id);

            if (!success)
                return NotFound(ApiResponse<bool>.Fail("Blog post not found"));

            return Ok(ApiResponse<bool>.Ok(true, "Blog post published"));
        }

        /// <summary>
        /// Unpublish blog post (set to draft)
        /// </summary>
        [HttpPatch("{id:int}/unpublish")]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        [ProducesResponseType(typeof(ApiResponse<string>), 400)]
        [ProducesResponseType(typeof(ApiResponse<string>), 500)]
        public async Task<IActionResult> Unpublish(int id)
        {
            var success = await _blogPostService.UnpublishAsync(id);

            if (!success)
                return NotFound(ApiResponse<bool>.Fail("Blog post not found"));

            return Ok(ApiResponse<bool>.Ok(true, "Blog post unpublished"));
        }
    }
}
