using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tekno.Application.Catalog.Interface;
using Tekno.Application.Common;
using Tekno.Application.Common.Paging;
using Tekno.Domain.Catalog;
using Tekno.Infrastructure.Persistence;

namespace Tekno.Infrastructure.Catalog
{
    public class AdvertisementRepository : IAdvertisementRepository
    {
        private readonly AppDbContext _context;

        public AdvertisementRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ProductAdvertisement?> GetByIdAsync(int id)
        {
            return await _context.Set<ProductAdvertisement>()
                .Include(a => a.Product)
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<PagedResult<ProductAdvertisement>> GetPagedAsync(
            string? position,
            bool? isActive,
            bool onlyCurrentlyActive,
            PagingParams paging)
        {
            var query = _context.Set<ProductAdvertisement>()
                .Include(a => a.Product)
                .AsNoTracking()
                .AsQueryable();

            // Filter by position
            if (!string.IsNullOrEmpty(position))
            {
                query = query.Where(a => a.Position == position);
            }

            // Filter by active status
            if (isActive.HasValue)
            {
                query = query.Where(a => a.IsActive == isActive.Value);
            }

            // Filter by currently active (within date range)
            if (onlyCurrentlyActive)
            {
                var now = DateTime.UtcNow;
                query = query.Where(a =>
                    a.IsActive &&
                    (!a.StartDate.HasValue || a.StartDate.Value <= now) &&
                    (!a.EndDate.HasValue || a.EndDate.Value >= now));
            }

            // Order by priority (highest first), then by creation date
            query = query.OrderByDescending(a => a.Priority)
                        .ThenByDescending(a => a.CreatedAt);

            var totalRecords = await query.CountAsync();

            var data = await query
                .Skip((paging.Page - 1) * paging.PageSize)
                .Take(paging.PageSize)
                .ToListAsync();

            return new PagedResult<ProductAdvertisement>(data, totalRecords, paging.Page, paging.PageSize);
        }

        public async Task<List<ProductAdvertisement>> GetActiveByPositionAsync(string position)
        {
            var now = DateTime.UtcNow;

            return await _context.Set<ProductAdvertisement>()
                .Include(a => a.Product)
                .AsNoTracking()
                .Where(a =>
                    a.Position == position &&
                    a.IsActive &&
                    (!a.StartDate.HasValue || a.StartDate.Value <= now) &&
                    (!a.EndDate.HasValue || a.EndDate.Value >= now))
                .OrderByDescending(a => a.Priority)
                .ThenByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<ProductAdvertisement>> GetCurrentlyActiveAsync()
        {
            var now = DateTime.UtcNow;

            return await _context.Set<ProductAdvertisement>()
                .Include(a => a.Product)
                .AsNoTracking()
                .Where(a =>
                    a.IsActive &&
                    (!a.StartDate.HasValue || a.StartDate.Value <= now) &&
                    (!a.EndDate.HasValue || a.EndDate.Value >= now))
                .OrderByDescending(a => a.Priority)
                .ThenBy(a => a.Position)
                .ToListAsync();
        }

        public async Task<ProductAdvertisement> CreateAsync(ProductAdvertisement advertisement)
        {
            _context.Set<ProductAdvertisement>().Add(advertisement);
            await _context.SaveChangesAsync();
            return advertisement;
        }

        public async Task<ProductAdvertisement> UpdateAsync(ProductAdvertisement advertisement)
        {
            _context.Set<ProductAdvertisement>().Update(advertisement);
            await _context.SaveChangesAsync();
            return advertisement;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var advertisement = await _context.Set<ProductAdvertisement>().FindAsync(id);
            if (advertisement == null) return false;

            _context.Set<ProductAdvertisement>().Remove(advertisement);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
