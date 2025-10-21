using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using Tekno.Application.Catalog.Interface;
using Tekno.Application.Common;
using Tekno.Application.Common.Paging;
using Tekno.Domain.Catalog;
using Tekno.Infrastructure.Persistence;

namespace Tekno.Infrastructure.Catalog
{
    public class ProductRepository:IProductRepository
    {
        private readonly AppDbContext _context;
        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<PagedResult<Product>> GetPagedProductAsync(
            string? categorySlug,
            string? brandSlug,
            string? search,
            string? sort,
            string? minPrice,
            string? maxPrice,
            PagingParams paging)
        {
            // --- 1️⃣ Base query
            var query = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.Images)
                .AsQueryable();

            // --- 2️⃣ Filtering
            if (!string.IsNullOrWhiteSpace(categorySlug))
                query = query.Where(p => p.Category.Slug == categorySlug);

            if (!string.IsNullOrWhiteSpace(brandSlug))
                query = query.Where(p => p.Brand.Slug == brandSlug);

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(p => p.Name.ToLower().Contains(search.ToLower()));

            if (decimal.TryParse(minPrice, out var min))
                query = query.Where(p => p.BasePrice >= min);

            if (decimal.TryParse(maxPrice, out var max))
                query = query.Where(p => p.BasePrice <= max);

            // --- 3️⃣ Sorting
            query = sort switch
            {
                "price_asc" => query.OrderBy(p => p.BasePrice),
                "price_desc" => query.OrderByDescending(p => p.BasePrice),
                "name_asc" => query.OrderBy(p => p.Name),
                "name_desc" => query.OrderByDescending(p => p.Name),
                "newest" => query.OrderByDescending(p => p.CreatedAt),
                _ => query.OrderByDescending(p => p.CreatedAt)
            };

            // --- 4️⃣ Paging
            var totalRecords = await query.CountAsync();

            var data = await query
                .Skip((paging.Page - 1) * paging.PageSize)
                .Take(paging.PageSize)
                .ToListAsync();

            // --- 5️⃣ Return PagedResult
            return new PagedResult<Product>(data, totalRecords, paging.Page, paging.PageSize);
        }
        public Task<Product?> GetProductBySlugAsync(string slug)
        {
            throw new NotImplementedException();
        }
    }
}
