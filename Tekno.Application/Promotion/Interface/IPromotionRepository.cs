using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tekno.Application.Common.Paging;
using PromotionEntity = Tekno.Domain.Promotion.Promotion;

namespace Tekno.Application.Promotion.Interface
{
    public interface IPromotionRepository
    {
        Task<PromotionEntity?> GetByIdAsync(int id);
        Task<PagedResult<PromotionEntity>> GetPagedAsync(
            string? search,
            string? status,
            DateTime? startDate,
            DateTime? endDate,
            PagingParams paging);
        
        // Get active promotions
        Task<IEnumerable<PromotionEntity>> GetActivePromotionsAsync();
        
        // Get promotions for specific product/category
        Task<IEnumerable<PromotionEntity>> GetPromotionsForProductAsync(int productId);
        Task<IEnumerable<PromotionEntity>> GetPromotionsForCategoryAsync(int categoryId);
        
        // Get promotions that should be activated/expired
        Task<IEnumerable<PromotionEntity>> GetScheduledPromotionsToActivateAsync();
        Task<IEnumerable<PromotionEntity>> GetActivePromotionsToExpireAsync();
        
        // CRUD operations
        Task<PromotionEntity> CreateAsync(PromotionEntity promotion);
        Task<PromotionEntity> UpdateAsync(PromotionEntity promotion);
        Task<bool> DeleteAsync(int id);
    }
}
