using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tekno.Application.Common;
using Tekno.Application.Common.Paging;
using Tekno.Domain.Promotion;

namespace Tekno.Application.Promotion.Interface
{
    public interface ICouponRepository
    {
        Task<Coupon?> GetByIdAsync(int id);
        Task<Coupon?> GetByCodeAsync(string code);
        Task<PagedResult<Coupon>> GetPagedAsync(
            string? search,
            string? status,
            DateTime? startDate,
            DateTime? endDate,
            PagingParams paging);
        Task<List<Coupon>> GetActiveCouponsAsync();
        Task<IEnumerable<Coupon>> GetExpiredActiveCouponsAsync();
        Task<IEnumerable<Coupon>> GetExpiredActiveProductDiscountsAsync();
        Task<bool> IsCodeExistsAsync(string code);
        Task<Coupon> CreateAsync(Coupon coupon);
        Task<Coupon> UpdateAsync(Coupon coupon);
        Task<bool> DeleteAsync(int id);
        Task<int> GetUserCouponUsageCountAsync(int couponId, int userId);
        Task<CouponUsage> RecordUsageAsync(CouponUsage usage);
        Task<List<CouponUsage>> GetUsageHistoryAsync(int couponId, PagingParams paging);
    }
}
