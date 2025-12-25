using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tekno.Application.Catalog.Interface;
using Tekno.Application.Common.Paging;
using Tekno.Domain.Catalog;
using Tekno.Infrastructure.Persistence;

namespace Tekno.Infrastructure.Catalog
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext _context;
        public CategoryRepository(AppDbContext context)
        {
            _context = context;
        }
        
        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            return await _context.Categories.AsNoTracking().ToListAsync();
        }

        public async Task<PagedResult<Category>> GetPagedAsync(string? search, PagingParams paging)
        {
            var query = _context.Categories.AsNoTracking();

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(c => 
                    c.Name.Contains(search) || 
                    c.Slug.Contains(search) ||
                    (c.Description != null && c.Description.Contains(search)));
            }

            // Order by name
            query = query.OrderBy(c => c.Name);

            // Get total count
            var totalRecords = await query.CountAsync();

            // Apply pagination
            var data = await query
                .Skip((paging.Page - 1) * paging.PageSize)
                .Take(paging.PageSize)
                .ToListAsync();

            return new PagedResult<Category>(data, totalRecords, paging.Page, paging.PageSize);
        }
        
        public async Task<Category?> GetCategoryBySlugAsync(string slug)
        {
            return await _context.Categories
                .Include(c => c.SubCategories)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Slug == slug);
        }

        public async Task<Category?> GetCategoryByIdAsync(int id)
        {
            return await _context.Categories
                .Include(c => c.SubCategories)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Category> CreateAsync(Category category)
        {
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<bool> UpdateAsync(Category category)
        {
            var existing = await _context.Categories.FindAsync(category.Id);
            if (existing == null) return false;

            existing.Name = category.Name;
            existing.Slug = category.Slug;
            existing.IconPath = category.IconPath;
            existing.ImageUrl = category.ImageUrl; // NEW
            existing.ParentId = category.ParentId;
            existing.Description = category.Description; // NEW
            existing.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return false;

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return true;
        }

        // NEW: return attributes that are specific to given category (include values)
        public async Task<List<ProductAttribute>> GetAttributesForCategoryAsync(int categoryId)
        {
            return await _context.Set<ProductAttribute>()
                .Include(a => a.Values)
                .Where(a => !a.IsGlobal && a.CategoryId == categoryId)
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task<List<ProductAttribute>> GetAttributesForCategoryBySlugAsync(string slug)
        {
            return await _context.Set<ProductAttribute>()
                .Include(a => a.Values)
                .Where(a => !a.IsGlobal && a.Category != null && a.Category.Slug == slug)
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task<List<ProductAttribute>> GetGlobalAttributesAsync()
        {
            return await _context.Set<ProductAttribute>()
                .Include(a => a.Values)
                .Where(a => a.IsGlobal)
                .AsNoTracking()
                .ToListAsync();
        }

        // NEW: Transaction support
        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }
    }
}
