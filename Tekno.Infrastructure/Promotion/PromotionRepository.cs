using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tekno.Application.Common.Paging;
using Tekno.Application.Promotion.Interface;
using Tekno.Domain.Promotion;
using Tekno.Infrastructure.Persistence;
using PromotionEntity = Tekno.Domain.Promotion.Promotion;

namespace Tekno.Infrastructure.Promotion
{
    public class PromotionRepository : IPromotionRepository
    {
        private readonly AppDbContext _context;

        public PromotionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PromotionEntity?> GetByIdAsync(int id)
        {
            return await _context.Set<PromotionEntity>()
                .Include(p => p.ApplicableCategories)
                .Include(p => p.ApplicableProducts)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<PagedResult<PromotionEntity>> GetPagedAsync(
            string? search,
            string? status,
            DateTime? startDate,
            DateTime? endDate,
            PagingParams paging)
        {
            var query = _context.Set<PromotionEntity>()
                .Include(p => p.ApplicableCategories)
                .Include(p => p.ApplicableProducts)
                .AsNoTracking()
                .AsQueryable();

            // Search filter
            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchLower = search.ToLower();
                query = query.Where(p =>
                    p.Name.ToLower().Contains(searchLower) ||
                    (p.Description != null && p.Description.ToLower().Contains(searchLower)));
            }

            // Status filter
            if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<PromotionStatus>(status, true, out var statusEnum))
            {
                query = query.Where(p => p.Status == statusEnum);
            }

            // Date range filter
            if (startDate.HasValue)
            {
                query = query.Where(p => p.StartDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(p => p.EndDate <= endDate.Value);
            }

            // Order by priority (highest first) then start date
            query = query.OrderByDescending(p => p.Priority).ThenByDescending(p => p.StartDate);

            var totalRecords = await query.CountAsync();

            var data = await query
                .Skip((paging.Page - 1) * paging.PageSize)
                .Take(paging.PageSize)
                .ToListAsync();

            return new PagedResult<PromotionEntity>(data, totalRecords, paging.Page, paging.PageSize);
        }

        public async Task<IEnumerable<PromotionEntity>> GetActivePromotionsAsync()
        {
            var now = DateTime.UtcNow;
            return await _context.Set<PromotionEntity>()
                .Include(p => p.ApplicableCategories)
                .Include(p => p.ApplicableProducts)
                .AsNoTracking()
                .Where(p =>
                    p.Status == PromotionStatus.Active &&
                    p.StartDate <= now &&
                    p.EndDate >= now)
                .OrderByDescending(p => p.Priority)
                .ToListAsync();
        }

        public async Task<IEnumerable<PromotionEntity>> GetPromotionsForProductAsync(int productId)
        {
            var now = DateTime.UtcNow;
            return await _context.Set<PromotionEntity>()
                .Include(p => p.ApplicableCategories)
                .Include(p => p.ApplicableProducts)
                .AsNoTracking()
                .Where(p =>
                    p.Status == PromotionStatus.Active &&
                    p.StartDate <= now &&
                    p.EndDate >= now &&
                    p.ApplicableProducts.Any(pp => pp.ProductId == productId))
                .OrderByDescending(p => p.Priority)
                .ToListAsync();
        }

        public async Task<IEnumerable<PromotionEntity>> GetPromotionsForCategoryAsync(int categoryId)
        {
            var now = DateTime.UtcNow;
            return await _context.Set<PromotionEntity>()
                .Include(p => p.ApplicableCategories)
                .Include(p => p.ApplicableProducts)
                .AsNoTracking()
                .Where(p =>
                    p.Status == PromotionStatus.Active &&
                    p.StartDate <= now &&
                    p.EndDate >= now &&
                    p.ApplicableCategories.Any(pc => pc.CategoryId == categoryId))
                .OrderByDescending(p => p.Priority)
                .ToListAsync();
        }

        public async Task<IEnumerable<PromotionEntity>> GetScheduledPromotionsToActivateAsync()
        {
            var now = DateTime.UtcNow;
            return await _context.Set<PromotionEntity>()
                .Where(p => p.Status == PromotionStatus.Scheduled && p.StartDate <= now)
                .ToListAsync();
        }

        public async Task<IEnumerable<PromotionEntity>> GetActivePromotionsToExpireAsync()
        {
            var now = DateTime.UtcNow;
            return await _context.Set<PromotionEntity>()
                .Where(p => p.Status == PromotionStatus.Active && p.EndDate < now)
                .ToListAsync();
        }

        public async Task<PromotionEntity> CreateAsync(PromotionEntity promotion)
        {
            _context.Set<PromotionEntity>().Add(promotion);
            await _context.SaveChangesAsync();
            return promotion;
        }

        public async Task<PromotionEntity> UpdateAsync(PromotionEntity promotion)
        {
            _context.Set<PromotionEntity>().Update(promotion);
            await _context.SaveChangesAsync();
            return promotion;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var promotion = await _context.Set<PromotionEntity>().FindAsync(id);
            if (promotion == null) return false;

            _context.Set<PromotionEntity>().Remove(promotion);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
