using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tekno.Application.Common;
using Tekno.Application.Common.Paging;
using Tekno.Application.Promotion.Interface;
using Tekno.Domain.Promotion;
using Tekno.Infrastructure.Persistence;

namespace Tekno.Infrastructure.Promotion
{
    public class CouponRepository : ICouponRepository
    {
        private readonly AppDbContext _context;

        public CouponRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Coupon?> GetByIdAsync(int id)
        {
            return await _context.Set<Coupon>()
                .Include(c => c.ApplicableCategories)
                .Include(c => c.ApplicableProducts)
                .Include(c => c.Usages)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Coupon?> GetByCodeAsync(string code)
        {
            var normalizedCode = code.Trim().ToUpperInvariant();
            return await _context.Set<Coupon>()
                .Include(c => c.ApplicableCategories)
                .Include(c => c.ApplicableProducts)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Code.ToUpper() == normalizedCode);
        }

        public async Task<PagedResult<Coupon>> GetPagedAsync(
            string? search,
            string? status,
            DateTime? startDate,
            DateTime? endDate,
            PagingParams paging)
        {
            var query = _context.Set<Coupon>()
                .Include(c => c.ApplicableCategories)
                .Include(c => c.ApplicableProducts)
                .AsNoTracking()
                .AsQueryable();

            // Search filter
            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchLower = search.ToLower();
                query = query.Where(c => 
                    c.Code.ToLower().Contains(searchLower) || 
                    c.Name.ToLower().Contains(searchLower));
            }

            // Status filter
            if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<CouponStatus>(status, true, out var statusEnum))
            {
                query = query.Where(c => c.Status == statusEnum);
            }

            // Date range filter
            if (startDate.HasValue)
            {
                query = query.Where(c => c.StartDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(c => c.EndDate <= endDate.Value);
            }

            // Order by creation date (newest first)
            query = query.OrderByDescending(c => c.CreatedAt);

            // Get total count
            var totalRecords = await query.CountAsync();

            // Apply pagination
            var data = await query
                .Skip((paging.Page - 1) * paging.PageSize)
                .Take(paging.PageSize)
                .ToListAsync();

            return new PagedResult<Coupon>(data, totalRecords, paging.Page, paging.PageSize);
        }

        public async Task<List<Coupon>> GetActiveCouponsAsync()
        {
            var now = DateTime.UtcNow;
            return await _context.Set<Coupon>()
                .Include(c => c.ApplicableCategories)
                .Include(c => c.ApplicableProducts)
                .AsNoTracking()
                .Where(c => 
                    c.Status == CouponStatus.Active &&
                    c.StartDate <= now &&
                    c.EndDate >= now &&
                    c.UsedCount < c.Quantity)
                .ToListAsync();
        }

        public async Task<IEnumerable<Coupon>> GetExpiredActiveCouponsAsync()
        {
            var now = DateTime.UtcNow;
            return await _context.Set<Coupon>()
                .Where(c => c.Status == CouponStatus.Active && 
                            (c.EndDate < now || c.UsedCount >= c.Quantity))
                .ToListAsync();
        }

        public async Task<IEnumerable<Coupon>> GetExpiredActiveProductDiscountsAsync()
        {
            var now = DateTime.UtcNow;
            return await _context.Set<Coupon>()
                .Where(c => c.Status == CouponStatus.Active &&
                            c.EndDate < now)
                .ToListAsync();
        }

        public async Task<bool> IsCodeExistsAsync(string code)
        {
            var normalizedCode = code.Trim().ToUpperInvariant();
            return await _context.Set<Coupon>()
                .AsNoTracking()
                .AnyAsync(c => c.Code.ToUpper() == normalizedCode);
        }

        public async Task<Coupon> CreateAsync(Coupon coupon)
        {
            _context.Set<Coupon>().Add(coupon);
            await _context.SaveChangesAsync();
            return coupon;
        }

        public async Task<Coupon> UpdateAsync(Coupon coupon)
        {
            _context.Set<Coupon>().Update(coupon);
            await _context.SaveChangesAsync();
            return coupon;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var coupon = await _context.Set<Coupon>().FindAsync(id);
            if (coupon == null) return false;

            _context.Set<Coupon>().Remove(coupon);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> GetUserCouponUsageCountAsync(int couponId, int userId)
        {
            return await _context.Set<CouponUsage>()
                .AsNoTracking()
                .CountAsync(u => u.CouponId == couponId && u.UserId == userId);
        }

        public async Task<CouponUsage> RecordUsageAsync(CouponUsage usage)
        {
            _context.Set<CouponUsage>().Add(usage);
            await _context.SaveChangesAsync();
            return usage;
        }

        public async Task<List<CouponUsage>> GetUsageHistoryAsync(int couponId, PagingParams paging)
        {
            return await _context.Set<CouponUsage>()
                .AsNoTracking()
                .Where(u => u.CouponId == couponId)
                .OrderByDescending(u => u.UsedAt)
                .Skip((paging.Page - 1) * paging.PageSize)
                .Take(paging.PageSize)
                .ToListAsync();
        }
    }
}
