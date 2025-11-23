using System;
using System.Collections.Generic;

namespace Tekno.Domain.Blog
{
    /// <summary>
    /// Blog post for product introductions, reviews, and analysis
    /// </summary>
    public class BlogPost
    {
        public int Id { get; private set; }
        public string Title { get; private set; } = string.Empty;
        public string Slug { get; private set; } = string.Empty;
        public string Summary { get; private set; } = string.Empty;
        public string Content { get; private set; } = string.Empty; // Long description/HTML content
        public string FeaturedImageUrl { get; private set; } = string.Empty;
        public int AuthorId { get; private set; }
        public BlogPostStatus Status { get; private set; } = BlogPostStatus.Draft;
        public int ViewCount { get; private set; } = 0;
        public DateTime PublishedAt { get; private set; }
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; private set; }

        // Navigation properties
        public ICollection<BlogPostTag> Tags { get; private set; } = new List<BlogPostTag>();
        public ICollection<BlogPostProduct> RelatedProducts { get; private set; } = new List<BlogPostProduct>();

        private BlogPost() { }

        public BlogPost(
            string title,
            string slug,
            string summary,
            string content,
            string featuredImageUrl,
            int authorId)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title cannot be empty", nameof(title));

            if (string.IsNullOrWhiteSpace(slug))
                throw new ArgumentException("Slug cannot be empty", nameof(slug));

            if (string.IsNullOrWhiteSpace(content))
                throw new ArgumentException("Content cannot be empty", nameof(content));

            Title = title.Trim();
            Slug = slug.Trim().ToLowerInvariant();
            Summary = summary?.Trim() ?? string.Empty;
            Content = content.Trim();
            FeaturedImageUrl = featuredImageUrl?.Trim() ?? string.Empty;
            AuthorId = authorId;
            Status = BlogPostStatus.Draft;
            CreatedAt = DateTime.UtcNow;
        }

        public void Update(string title, string summary, string content, string featuredImageUrl)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title cannot be empty", nameof(title));

            if (string.IsNullOrWhiteSpace(content))
                throw new ArgumentException("Content cannot be empty", nameof(content));

            Title = title.Trim();
            Summary = summary?.Trim() ?? string.Empty;
            Content = content.Trim();
            FeaturedImageUrl = featuredImageUrl?.Trim() ?? string.Empty;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Publish()
        {
            if (Status != BlogPostStatus.Published)
            {
                Status = BlogPostStatus.Published;
                PublishedAt = DateTime.UtcNow;
            }
        }

        public void Unpublish()
        {
            Status = BlogPostStatus.Draft;
        }

        public void IncrementViewCount()
        {
            ViewCount++;
        }

        public void AddTag(string tag)
        {
            if (string.IsNullOrWhiteSpace(tag)) return;
            
            var normalizedTag = tag.Trim().ToLowerInvariant();
            if (!Tags.Any(t => t.Tag.ToLowerInvariant() == normalizedTag))
            {
                Tags.Add(new BlogPostTag(Id, normalizedTag));
            }
        }

        public void AddRelatedProduct(int productId)
        {
            if (productId <= 0) return;
            
            if (!RelatedProducts.Any(p => p.ProductId == productId))
            {
                RelatedProducts.Add(new BlogPostProduct(Id, productId));
            }
        }

        public void ClearTags()
        {
            Tags.Clear();
        }

        public void ClearRelatedProducts()
        {
            RelatedProducts.Clear();
        }
    }

    public enum BlogPostStatus
    {
        Draft = 1,
        Published = 2,
        Archived = 3
    }

    /// <summary>
    /// Tags for blog posts (e.g., "review", "new-product", "comparison")
    /// </summary>
    public class BlogPostTag
    {
        public int Id { get; private set; }
        public int BlogPostId { get; private set; }
        public string Tag { get; private set; } = string.Empty;

        public BlogPost BlogPost { get; private set; } = null!;

        private BlogPostTag() { }

        public BlogPostTag(int blogPostId, string tag)
        {
            BlogPostId = blogPostId;
            Tag = tag.Trim().ToLowerInvariant();
        }
    }

    /// <summary>
    /// Link blog posts to related products
    /// </summary>
    public class BlogPostProduct
    {
        public int Id { get; private set; }
        public int BlogPostId { get; private set; }
        public int ProductId { get; private set; }

        public BlogPost BlogPost { get; private set; } = null!;

        private BlogPostProduct() { }

        public BlogPostProduct(int blogPostId, int productId)
        {
            BlogPostId = blogPostId;
            ProductId = productId;
        }
    }
}
