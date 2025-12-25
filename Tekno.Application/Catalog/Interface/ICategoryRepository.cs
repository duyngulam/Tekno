using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tekno.Domain.Catalog;
using Microsoft.EntityFrameworkCore.Storage;
using Tekno.Application.Common;
using Tekno.Application.Common.Paging;
using Tekno.Application.Catalog.DTOs.Products;

namespace Tekno.Application.Catalog.Interface
{
    public interface ICategoryRepository
    {
       Task<List<Category>> GetAllCategoriesAsync();
       Task<PagedResult<Category>> GetPagedAsync(string? search, PagingParams paging);
       Task<Category?> GetCategoryBySlugAsync(string slug);
       Task<Category?> GetCategoryByIdAsync(int id);
       Task<Category> CreateAsync(Category category);
       Task<bool> UpdateAsync(Category category);
       Task<bool> DeleteAsync(int id);
       Task<List<ProductAttribute>> GetAttributesForCategoryAsync(int categoryId);
       Task<List<ProductAttribute>> GetAttributesForCategoryBySlugAsync(string slug);
       Task <List<ProductAttribute>> GetGlobalAttributesAsync();

        // NEW: Transaction support
        Task<IDbContextTransaction> BeginTransactionAsync();

    }
}