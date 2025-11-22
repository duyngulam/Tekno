using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Tekno.Api.Common.Responses;
using Tekno.Application.Blog.DTOs;
using Tekno.Application.Blog.Services;
using Tekno.Application.Common.Paging;

namespace Tekno.Api.Controllers
{
    /// <summary>
    /// Public endpoints for viewing blog posts
    /// </summary>
    [ApiController]
    [Route("api/blog")]
    public class BlogController : ControllerBase
    {
        private readonly BlogPostService _blogPostService;

        public BlogController(BlogPostService blogPostService)
        {
            _blogPostService = blogPostService;
        }

        /// <summary>
        /// Get published blog posts (paginated)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] BlogPostQueryDto query)
        {
            // Force status to Published for public API
            query.Status = "Published";
            
            var result = await _blogPostService.GetPagedAsync(query);
            return Ok(ApiResponse<PagedResult<BlogPostSummaryDto>>.Ok(result));
        }

        /// <summary>
        /// Get blog post by slug
        /// </summary>
        /// <param name="slug">Blog post slug (e.g., "introducing-dell-xps-13")</param>
        /// <remarks>
        /// Returns full blog post details including content, tags, and related products.
        /// Automatically increments view count.
        /// 
        /// Example:
        /// - GET /api/blog/introducing-dell-xps-13
        /// </remarks>
        [HttpGet("{slug}")]
        public async Task<IActionResult> GetBySlug(string slug)
        {
            var blogPost = await _blogPostService.GetBySlugAsync(slug);

            if (blogPost == null)
                return NotFound(ApiResponse<BlogPostDetailDto>.Fail("Blog post not found"));

            // Only show published posts to public
            if (blogPost.Status != "Published")
                return NotFound(ApiResponse<BlogPostDetailDto>.Fail("Blog post not found"));

            return Ok(ApiResponse<BlogPostDetailDto>.Ok(blogPost));
        }

        /// <summary>
        /// Get recent blog posts
        /// </summary>
        /// <param name="count">Number of posts to return (default: 5, max: 20)</param>
        [HttpGet("recent")]
        public async Task<IActionResult> GetRecent([FromQuery] int count = 5)
        {
            if (count > 20) count = 20;
            if (count < 1) count = 5;

            var blogPosts = await _blogPostService.GetRecentPostsAsync(count);
            return Ok(ApiResponse<List<BlogPostSummaryDto>>.Ok(blogPosts));
        }

        /// <summary>
        /// Get related blog posts
        /// </summary>
        /// <param name="id">Blog post ID</param>
        /// <param name="count">Number of related posts (default: 3, max: 10)</param>
        [HttpGet("{id:int}/related")]
        public async Task<IActionResult> GetRelated(int id, [FromQuery] int count = 3)
        {
            if (count > 10) count = 10;
            if (count < 1) count = 3;

            var blogPosts = await _blogPostService.GetRelatedPostsAsync(id, count);
            return Ok(ApiResponse<List<BlogPostSummaryDto>>.Ok(blogPosts));
        }

        /// <summary>
        /// Get blog posts by tag
        /// </summary>
        /// <param name="tag">Tag name (e.g., "review", "new-product")</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Items per page</param>
        [HttpGet("tag/{tag}")]
        public async Task<IActionResult> GetByTag(
            string tag,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var query = new BlogPostQueryDto
            {
                Tag = tag,
                Status = "Published",
                Page = page,
                PageSize = pageSize
            };

            var result = await _blogPostService.GetPagedAsync(query);
            return Ok(ApiResponse<PagedResult<BlogPostSummaryDto>>.Ok(result));
        }
    }
}
