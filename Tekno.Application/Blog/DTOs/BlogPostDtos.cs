using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Tekno.Application.Blog.DTOs
{
    /// <summary>
    /// Blog post summary for list views
    /// </summary>
    public class BlogPostSummaryDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
        public string FeaturedImageUrl { get; set; } = string.Empty;
        public string AuthorName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int ViewCount { get; set; }
        public DateTime PublishedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<string> Tags { get; set; } = new();
    }

    /// <summary>
    /// Full blog post details
    /// </summary>
    public class BlogPostDetailDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string FeaturedImageUrl { get; set; } = string.Empty;
        public string AuthorName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int ViewCount { get; set; }
        public DateTime PublishedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<string> Tags { get; set; } = new();
        public List<RelatedProductDto> RelatedProducts { get; set; } = new();
    }

    public class RelatedProductDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductSlug { get; set; } = string.Empty;
        public string ProductImage { get; set; } = string.Empty;
        public decimal ProductPrice { get; set; }
    }

    /// <summary>
    /// Create blog post
    /// </summary>
    public class CreateBlogPostDto
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, MinimumLength = 10, ErrorMessage = "Title must be between 10 and 200 characters")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Slug is required")]
        [StringLength(200, MinimumLength = 5, ErrorMessage = "Slug must be between 5 and 200 characters")]
        [RegularExpression(@"^[a-z0-9-]+$", ErrorMessage = "Slug can only contain lowercase letters, numbers, and hyphens")]
        public string Slug { get; set; } = string.Empty;

        [Required(ErrorMessage = "Summary is required")]
        [StringLength(500, MinimumLength = 20, ErrorMessage = "Summary must be between 20 and 500 characters")]
        public string Summary { get; set; } = string.Empty;

        [Required(ErrorMessage = "Content is required")]
        [MinLength(100, ErrorMessage = "Content must be at least 100 characters")]
        public string Content { get; set; } = string.Empty;

        [Required(ErrorMessage = "Featured image is required")]
        public IFormFile FeaturedImage { get; set; } = null!;

        public List<string> Tags { get; set; } = new();

        public List<int> RelatedProductIds { get; set; } = new();

        public bool PublishImmediately { get; set; } = false;
    }

    /// <summary>
    /// Update blog post
    /// </summary>
    public class UpdateBlogPostDto
    {
        [Required]
        [StringLength(200, MinimumLength = 10)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(500, MinimumLength = 20)]
        public string Summary { get; set; } = string.Empty;

        [Required]
        [MinLength(100)]
        public string Content { get; set; } = string.Empty;

        public IFormFile? FeaturedImage { get; set; } // Optional - only if changing image

        public List<string> Tags { get; set; } = new();

        public List<int> RelatedProductIds { get; set; } = new();
    }

    /// <summary>
    /// Query parameters for blog posts
    /// </summary>
    public class BlogPostQueryDto
    {
        public string? Tag { get; set; }
        public string? Status { get; set; }
        public string? Keyword { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
