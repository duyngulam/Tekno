using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tekno.Application.Common;
using Tekno.Application.Common.Paging;
using Tekno.Application.Review.Interface;
using Tekno.Domain.Review;
using Tekno.Infrastructure.Persistence;

namespace Tekno.Infrastructure.Review
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly AppDbContext _context;

        public ReviewRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ProductReview?> GetByIdAsync(int id)
        {
            return await _context.Set<ProductReview>()
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<ProductReview?> GetUserReviewForProductAsync(int userId, int productId)
        {
            return await _context.Set<ProductReview>()
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.UserId == userId && r.ProductId == productId);
        }

        public async Task<PagedResult<ProductReview>> GetProductReviewsAsync(
            int productId,
            string? status,
            bool? isVerifiedPurchase,
            PagingParams paging)
        {
            var query = _context.Set<ProductReview>()
                .AsNoTracking()
                .Where(r => r.ProductId == productId);

            // Filter by status
            //if (!string.IsNullOrWhiteSpace(status) && System.Enum.TryParse<ReviewStatus>(status, true, out var statusEnum))
            //{
            //    query = query.Where(r => r.Status == statusEnum);
            //}

            // Filter by verified purchase
            if (isVerifiedPurchase.HasValue)
            {
                query = query.Where(r => r.IsVerifiedPurchase == isVerifiedPurchase.Value);
            }

            // Order by helpful votes and date
            query = query.OrderByDescending(r => r.HelpfulCount)
                        .ThenByDescending(r => r.CreatedAt);

            var totalRecords = await query.CountAsync();

            var data = await query
                .Skip((paging.Page - 1) * paging.PageSize)
                .Take(paging.PageSize)
                .ToListAsync();

            return new PagedResult<ProductReview>(data, totalRecords, paging.Page, paging.PageSize);
        }

        public async Task<List<ProductReview>> GetAllProductReviewsAsync(int productId)
        {
            return await _context.Set<ProductReview>()
                .AsNoTracking()
                .Where(r => r.ProductId == productId && r.Status == ReviewStatus.Approved)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<ProductReview> CreateAsync(ProductReview review)
        {
            _context.Set<ProductReview>().Add(review);
            await _context.SaveChangesAsync();
            return review;
        }

        public async Task<ProductReview> UpdateAsync(ProductReview review)
        {
            _context.Set<ProductReview>().Update(review);
            await _context.SaveChangesAsync();
            return review;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var review = await _context.Set<ProductReview>().FindAsync(id);
            if (review == null) return false;

            _context.Set<ProductReview>().Remove(review);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> HasUserReviewedProductAsync(int userId, int productId)
        {
            return await _context.Set<ProductReview>()
                .AsNoTracking()
                .AnyAsync(r => r.UserId == userId && r.ProductId == productId);
        }

        public async Task<ReviewHelpfulness?> GetUserVoteAsync(int reviewId, int userId)
        {
            return await _context.Set<ReviewHelpfulness>()
                .AsNoTracking()
                .FirstOrDefaultAsync(v => v.ReviewId == reviewId && v.UserId == userId);
        }

        public async Task<ReviewHelpfulness> RecordVoteAsync(ReviewHelpfulness vote)
        {
            _context.Set<ReviewHelpfulness>().Add(vote);
            await _context.SaveChangesAsync();
            return vote;
        }

        public async Task<bool> RemoveVoteAsync(int reviewId, int userId)
        {
            var vote = await _context.Set<ReviewHelpfulness>()
                .FirstOrDefaultAsync(v => v.ReviewId == reviewId && v.UserId == userId);

            if (vote == null) return false;

            _context.Set<ReviewHelpfulness>().Remove(vote);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
