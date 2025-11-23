using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tekno.Application.Blog.Interface;
using Tekno.Application.Common;
using Tekno.Application.Common.Paging;
using Tekno.Domain.Blog;
using Tekno.Infrastructure.Persistence;

namespace Tekno.Infrastructure.Blog
{
    public class BlogPostRepository : IBlogPostRepository
    {
        private readonly AppDbContext _context;

        public BlogPostRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<BlogPost?> GetByIdAsync(int id)
        {
            return await _context.Set<BlogPost>()
                .Include(b => b.Tags)
                .Include(b => b.RelatedProducts)
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<BlogPost?> GetBySlugAsync(string slug)
        {
            if (string.IsNullOrWhiteSpace(slug)) return null;

            return await _context.Set<BlogPost>()
                .Include(b => b.Tags)
                .Include(b => b.RelatedProducts)
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Slug == slug.ToLowerInvariant());
        }

        public async Task<bool> IsSlugExistsAsync(string slug)
        {
            if (string.IsNullOrWhiteSpace(slug)) return false;

            return await _context.Set<BlogPost>()
                .AsNoTracking()
                .AnyAsync(b => b.Slug == slug.ToLowerInvariant());
        }

        public async Task<PagedResult<BlogPost>> GetPagedAsync(
            string? tag,
            BlogPostStatus? status,
            string? keyword,
            PagingParams paging)
        {
            var query = _context.Set<BlogPost>()
                .Include(b => b.Tags)
                .AsNoTracking()
                .AsQueryable();

            // Filter by tag
            if (!string.IsNullOrWhiteSpace(tag))
            {
                var normalizedTag = tag.ToLowerInvariant();
                query = query.Where(b => b.Tags.Any(t => t.Tag == normalizedTag));
            }

            // Filter by status
            if (status.HasValue)
            {
                query = query.Where(b => b.Status == status.Value);
            }

            // Filter by keyword (search in title and summary)
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(b =>
                    EF.Functions.ILike(b.Title, $"%{keyword}%") ||
                    EF.Functions.ILike(b.Summary, $"%{keyword}%"));
            }

            // Order by published date (newest first)
            query = query.OrderByDescending(b => b.PublishedAt)
                        .ThenByDescending(b => b.CreatedAt);

            var totalRecords = await query.CountAsync();

            var data = await query
                .Skip((paging.Page - 1) * paging.PageSize)
                .Take(paging.PageSize)
                .ToListAsync();

            return new PagedResult<BlogPost>(data, totalRecords, paging.Page, paging.PageSize);
        }

        public async Task<List<BlogPost>> GetRecentPublishedAsync(int count)
        {
            return await _context.Set<BlogPost>()
                .Include(b => b.Tags)
                .AsNoTracking()
                .Where(b => b.Status == BlogPostStatus.Published)
                .OrderByDescending(b => b.PublishedAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task<List<BlogPost>> GetRelatedPostsAsync(int blogPostId, int count)
        {
            // Get the current post's tags
            var currentPost = await _context.Set<BlogPost>()
                .Include(b => b.Tags)
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Id == blogPostId);

            if (currentPost == null || !currentPost.Tags.Any())
            {
                // Return recent posts if no tags
                return await GetRecentPublishedAsync(count);
            }

            var currentTags = currentPost.Tags.Select(t => t.Tag).ToList();

            // Find posts with similar tags
            return await _context.Set<BlogPost>()
                .Include(b => b.Tags)
                .AsNoTracking()
                .Where(b =>
                    b.Id != blogPostId &&
                    b.Status == BlogPostStatus.Published &&
                    b.Tags.Any(t => currentTags.Contains(t.Tag)))
                .OrderByDescending(b => b.Tags.Count(t => currentTags.Contains(t.Tag))) // Most similar first
                .ThenByDescending(b => b.PublishedAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task<BlogPost> CreateAsync(BlogPost blogPost)
        {
            _context.Set<BlogPost>().Add(blogPost);
            await _context.SaveChangesAsync();
            return blogPost;
        }

        public async Task<BlogPost> UpdateAsync(BlogPost blogPost)
        {
            _context.Set<BlogPost>().Update(blogPost);
            await _context.SaveChangesAsync();
            return blogPost;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var blogPost = await _context.Set<BlogPost>().FindAsync(id);
            if (blogPost == null) return false;

            _context.Set<BlogPost>().Remove(blogPost);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> IncrementViewCountAsync(int id)
        {
            var blogPost = await _context.Set<BlogPost>().FindAsync(id);
            if (blogPost == null) return false;

            blogPost.IncrementViewCount();
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
