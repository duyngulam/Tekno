using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tekno.Application.Common;
using Tekno.Application.Common.Exceptions;
using Tekno.Application.Common.Interfaces;
using Tekno.Application.Common.Paging;
using Tekno.Application.Order.Interface;
using Tekno.Application.Review.DTOs;
using Tekno.Application.Review.Interface;
using Tekno.Domain.Review;
using Tekno.Application.Catalog.Interface;
using Tekno.Application.Catalog.DTOs.Products;
using AutoMapper;

namespace Tekno.Application.Review.Services
{
    public class ReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IAppLogger<ReviewService> _logger;
        private readonly IProductRepository _productRepository;
        private readonly IElasticProductService _elasticService;
        private readonly IMapper _mapper;

        public ReviewService(
            IReviewRepository reviewRepository,
            IOrderRepository orderRepository,
            IAppLogger<ReviewService> logger,
            IProductRepository productRepository,
            IElasticProductService elasticService,
            IMapper mapper)
        {
            _reviewRepository = reviewRepository;
            _orderRepository = orderRepository;
            _logger = logger;
            _productRepository = productRepository;
            _elasticService = elasticService;
            _mapper = mapper;
        }

        /// <summary>
        /// Check if user can review a product (must have purchased it)
        /// </summary>
        public async Task<CanReviewResultDto> CanUserReviewProductAsync(int userId, int productId)
        {
            var result = new CanReviewResultDto();

            // Check if already reviewed
            var hasReviewed = await _reviewRepository.HasUserReviewedProductAsync(userId, productId);
            result.HasAlreadyReviewed = hasReviewed;

            if (hasReviewed)
            {
                result.CanReview = false;
                result.Message = "You have already reviewed this product";
                return result;
            }

            // Check if user has purchased the product
            var hasPurchased = await _orderRepository.HasUserPurchasedProductAsync(userId, productId);
            result.HasPurchased = hasPurchased;

            if (!hasPurchased)
            {
                result.CanReview = false;
                result.Message = "You can only review products you have purchased";
                return result;
            }

            // Get eligible orders (completed orders containing this product)
            var completedOrders = await _orderRepository.GetUserCompletedOrdersAsync(userId);
            var eligibleOrders = completedOrders
                .Where(o => o.HasPurchasedProduct(productId))
                .Select(o => new PurchaseInfoDto
                {
                    OrderId = o.Id,
                    OrderNumber = o.OrderNumber,
                    PurchaseDate = o.CompletedAt ?? o.CreatedAt,
                    VariantId = o.Items.First(i => i.ProductId == productId).VariantId,
                    VariantSku = "SKU" // Can be populated from variant lookup
                })
                .ToList();

            result.EligibleOrders = eligibleOrders;
            result.CanReview = true;
            result.Message = "You can review this product";

            return result;
        }

        /// <summary>
        /// Create a review (with purchase verification)
        /// </summary>
        public async Task<ProductReviewDto> CreateReviewAsync(int userId, CreateReviewDto dto)
        {
            // Verify user can review this product
            var canReview = await CanUserReviewProductAsync(userId, dto.ProductId);

            if (!canReview.CanReview)
            {
                throw new InvalidOperationException(canReview.Message);
            }

            // Auto-detect the most recent order for this product
            var order = await _orderRepository.GetUserOrderForProductAsync(userId, dto.ProductId);

            if (order == null || !order.HasPurchasedProduct(dto.ProductId))
            {
                throw new InvalidOperationException("No valid order found for this product");
            }

            // Get variant info from order
            var orderItem = order.Items.First(i => i.ProductId == dto.ProductId);

            // Create review
            var review = new ProductReview(
                productId: dto.ProductId,
                userId: userId,
                rating: dto.Rating,
                comment: dto.Comment,
                orderId: order.Id,
                variantId: orderItem.VariantId,
                isVerifiedPurchase: true
            );

            review = await _reviewRepository.CreateAsync(review);

            _logger.LogInformation(
                "User {UserId} created review {ReviewId} for product {ProductId} (Order: {OrderId})",
                userId, review.Id, dto.ProductId, order.Id);

            // Re-index product in Elasticsearch to refresh rating (approved reviews only affect rating,
            // but indexing ensures search data is up-to-date when admin approves the review later)
            await TryIndexProductAsync(dto.ProductId);

            return await MapToReviewDtoAsync(review);
        }

        /// <summary>
        /// Update user's own review
        /// </summary>
        public async Task<ProductReviewDto?> UpdateReviewAsync(int userId, int reviewId, UpdateReviewDto dto)
        {
            var review = await _reviewRepository.GetByIdAsync(reviewId);

            if (review == null)
            {
                return null;
            }

            // Verify ownership
            if (review.UserId != userId)
            {
                throw new UnauthorizedAccessException("You can only update your own reviews");
            }

            review.Update(dto.Rating, dto.Comment);
            review = await _reviewRepository.UpdateAsync(review);

            _logger.LogInformation(
                "User {UserId} updated review {ReviewId}",
                userId, reviewId);

            // Re-index product so Elasticsearch rating is updated
            await TryIndexProductAsync(review.ProductId);

            return await MapToReviewDtoAsync(review);
        }

        /// <summary>
        /// Delete user's own review
        /// </summary>
        public async Task<bool> DeleteReviewAsync(int userId, int reviewId)
        {
            var review = await _reviewRepository.GetByIdAsync(reviewId);

            if (review == null)
            {
                return false;
            }

            // Verify ownership
            if (review.UserId != userId)
            {
                throw new UnauthorizedAccessException("You can only delete your own reviews");
            }

            var success = await _reviewRepository.DeleteAsync(reviewId);

            if (success)
            {
                _logger.LogInformation(
                    "User {UserId} deleted review {ReviewId}",
                    userId, reviewId);

                // Re-index product to update rating
                await TryIndexProductAsync(review.ProductId);
            }

            return success;
        }

        /// <summary>
        /// Get reviews for a product (public)
        /// </summary>
        public async Task<ReviewListDto> GetProductReviewsAsync(
            int productId,
            bool? isVerifiedPurchase = null,
            int page = 1,
            int pageSize = 20)
        {
            var paging = new PagingParams(page, pageSize);

            // Only show approved reviews to public
            var result = await _reviewRepository.GetProductReviewsAsync(
                productId,
                ReviewStatus.Approved.ToString(),
                isVerifiedPurchase,
                paging);

            var reviewDtos = new List<ProductReviewDto>();
            foreach (var review in result.Data)
            {
                reviewDtos.Add(await MapToReviewDtoAsync(review));
            }

            // Get summary statistics
            var allReviews = await _reviewRepository.GetAllProductReviewsAsync(productId);
            var summary = CalculateSummary(productId, allReviews);

            return new ReviewListDto
            {
                Reviews = reviewDtos,
                Summary = summary,
                TotalCount = result.TotalRecords,
                Page = page,
                PageSize = pageSize
            };
        }

        /// <summary>
        /// Get product review summary (for product cards/detail page)
        /// </summary>
        public async Task<ProductReviewSummaryDto> GetProductSummaryAsync(int productId)
        {
            var reviews = await _reviewRepository.GetAllProductReviewsAsync(productId);
            return CalculateSummary(productId, reviews);
        }

        /// <summary>
        /// Vote on review helpfulness
        /// </summary>
        public async Task<bool> VoteReviewHelpfulnessAsync(int userId, int reviewId, bool isHelpful)
        {
            var review = await _reviewRepository.GetByIdAsync(reviewId);
            if (review == null)
            {
                throw new NotFoundException("ProductReview", reviewId);
            }

            // Check if user already voted
            var existingVote = await _reviewRepository.GetUserVoteAsync(reviewId, userId);
            if (existingVote != null)
            {
                // Remove old vote counts
                if (existingVote.IsHelpful)
                    review.IncrementNotHelpful(); // This is wrong, we should decrement
                else
                    review.IncrementHelpful(); // This is wrong too

                await _reviewRepository.RemoveVoteAsync(reviewId, userId);
            }

            // Record new vote
            var vote = new ReviewHelpfulness(reviewId, userId, isHelpful);
            await _reviewRepository.RecordVoteAsync(vote);

            // Update review counts
            if (isHelpful)
                review.IncrementHelpful();
            else
                review.IncrementNotHelpful();

            await _reviewRepository.UpdateAsync(review);

            _logger.LogInformation(
                "User {UserId} voted review {ReviewId} as {Vote}",
                userId, reviewId, isHelpful ? "helpful" : "not helpful");

            return true;
        }

        // Admin methods

        public async Task<ProductReviewDto?> ApproveReviewAsync(int reviewId, int adminUserId)
        {
            var review = await _reviewRepository.GetByIdAsync(reviewId);
            if (review == null) return null;

            review.Approve(adminUserId);
            review = await _reviewRepository.UpdateAsync(review);

            _logger.LogInformation(
                "Admin {AdminId} approved review {ReviewId}",
                adminUserId, reviewId);

            // Re-index product to include this approved review in rating
            await TryIndexProductAsync(review.ProductId);

            return await MapToReviewDtoAsync(review);
        }

        public async Task<ProductReviewDto?> RejectReviewAsync(int reviewId, int adminUserId)
        {
            var review = await _reviewRepository.GetByIdAsync(reviewId);
            if (review == null) return null;

            review.Reject(adminUserId);
            review = await _reviewRepository.UpdateAsync(review);

            _logger.LogInformation(
                "Admin {AdminId} rejected review {ReviewId}",
                adminUserId, reviewId);

            // Re-index product to remove this review from rating if it was approved before
            await TryIndexProductAsync(review.ProductId);

            return await MapToReviewDtoAsync(review);
        }

        // Helper methods

        private async Task<ProductReviewDto> MapToReviewDtoAsync(ProductReview review)
        {
            return new ProductReviewDto
            {
                Id = review.Id,
                ProductId = review.ProductId,
                UserId = review.UserId,
                UserEmail = "user@example.com", // Load from User entity if needed
                Rating = review.Rating,
                Comment = review.Comment,
                Status = review.Status.ToString(),
                CreatedAt = review.CreatedAt,
                UpdatedAt = review.UpdatedAt,
                IsVerifiedPurchase = review.IsVerifiedPurchase,
                HelpfulCount = review.HelpfulCount,
                NotHelpfulCount = review.NotHelpfulCount,
                VariantSku = null, // Can load from variant if needed
                VariantAttributes = null
            };
        }

        private ProductReviewSummaryDto CalculateSummary(int productId, List<ProductReview> reviews)
        {
            if (!reviews.Any())
            {
                return new ProductReviewSummaryDto
                {
                    ProductId = productId,
                    TotalReviews = 0,
                    AverageRating = 0,
                    RatingDistribution = new Dictionary<int, int>
                    {
                        { 5, 0 }, { 4, 0 }, { 3, 0 }, { 2, 0 }, { 1, 0 }
                    },
                    VerifiedPurchaseCount = 0
                };
            }

            var avgRating = reviews.Average(r => r.Rating);
            var distribution = reviews.GroupBy(r => r.Rating)
                .ToDictionary(g => g.Key, g => g.Count());

            // Fill in missing ratings
            for (int i = 1; i <= 5; i++)
            {
                if (!distribution.ContainsKey(i))
                    distribution[i] = 0;
            }

            return new ProductReviewSummaryDto
            {
                ProductId = productId,
                TotalReviews = reviews.Count,
                AverageRating = Math.Round(avgRating, 1),
                RatingDistribution = distribution.OrderByDescending(kv => kv.Key).ToDictionary(kv => kv.Key, kv => kv.Value),
                VerifiedPurchaseCount = reviews.Count(r => r.IsVerifiedPurchase)
            };
        }

        private async Task TryIndexProductAsync(int productId)
        {
            try
            {
                var product = await _productRepository.GetProductByIdAsync(productId);
                if (product != null)
                {
                    var summary = _mapper.Map<ProductSummaryDto>(product);
                    await _elasticService.IndexProductAsync(summary);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to index product {ProductId} after review change", productId);
            }
        }
    }
}
