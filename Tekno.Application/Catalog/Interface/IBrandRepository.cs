using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tekno.Domain.Catalog;
using Tekno.Application.Common.Paging;
using Microsoft.EntityFrameworkCore.Storage;

namespace Tekno.Application.Catalog.Interface
{
    public interface IBrandRepository
    {
        Task<List<Brand?>> GetAllBrandsAsync();
        Task<PagedResult<Brand>> GetPagedAsync(string? search, PagingParams paging);
        Task<Brand?> GetBrandBySlugAsync(string slug);
        Task<Brand?> GetBrandByIdAsync(int id);
        Task<Brand> CreateAsync(Brand brand);
        Task<bool> UpdateAsync(Brand brand);
        Task<bool> DeleteAsync(int id);
        
        // Get brands by category (only brands with products in that category)
        Task<List<Brand>> GetBrandsByCategoryAsync(string categorySlug);
        
        // Transaction support
        Task<IDbContextTransaction> BeginTransactionAsync();
    }
}
