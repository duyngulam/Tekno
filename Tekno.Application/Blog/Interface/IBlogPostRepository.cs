using System.Collections.Generic;
using System.Threading.Tasks;
using Tekno.Application.Common;
using Tekno.Application.Common.Paging;
using Tekno.Domain.Blog;

namespace Tekno.Application.Blog.Interface
{
    public interface IBlogPostRepository
    {
        Task<BlogPost?> GetByIdAsync(int id);
        Task<BlogPost?> GetBySlugAsync(string slug);
        Task<bool> IsSlugExistsAsync(string slug);
        Task<PagedResult<BlogPost>> GetPagedAsync(
            string? tag,
            BlogPostStatus? status,
            string? keyword,
            PagingParams paging);
        Task<List<BlogPost>> GetRecentPublishedAsync(int count);
        Task<List<BlogPost>> GetRelatedPostsAsync(int blogPostId, int count);
        Task<BlogPost> CreateAsync(BlogPost blogPost);
        Task<BlogPost> UpdateAsync(BlogPost blogPost);
        Task<bool> DeleteAsync(int id);
        Task<bool> IncrementViewCountAsync(int id);
    }
}
