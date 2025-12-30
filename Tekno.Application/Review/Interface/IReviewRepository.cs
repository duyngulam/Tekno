using System.Collections.Generic;
using System.Threading.Tasks;
using Tekno.Application.Common;
using Tekno.Application.Common.Paging;
using Tekno.Domain.Review;

namespace Tekno.Application.Review.Interface
{
    public interface IReviewRepository
    {
        Task<ProductReview?> GetByIdAsync(int id);
        Task<ProductReview?> GetUserReviewForProductAsync(int userId, int productId);
        Task<PagedResult<ProductReview>> GetProductReviewsAsync(
            int productId, 
            string? status, 
            bool? isVerifiedPurchase,
            PagingParams paging);
        Task<List<ProductReview>> GetAllProductReviewsAsync(int productId);
        Task<List<ProductReview>> GetAllReviewsByStatusAsync(int productId);
        Task<List<ProductReview>> GetAllReviewsAsync();
        Task<ProductReview> CreateAsync(ProductReview review);
        Task<ProductReview> UpdateAsync(ProductReview review);
        Task<bool> DeleteAsync(int id);
        Task<bool> HasUserReviewedProductAsync(int userId, int productId);
        Task<ReviewHelpfulness?> GetUserVoteAsync(int reviewId, int userId);
        Task<ReviewHelpfulness> RecordVoteAsync(ReviewHelpfulness vote);
        Task<bool> RemoveVoteAsync(int reviewId, int userId);
    }
}
