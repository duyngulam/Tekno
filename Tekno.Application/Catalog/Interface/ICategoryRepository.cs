using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tekno.Domain.Catalog;

namespace Tekno.Application.Catalog.Interface
{
    public interface ICategoryRepository
    {
       Task<List<Category>> GetAllCategoriesAsync();
       Task<Category?> GetCategoryBySlugAsync(string slug);
       Task<Category?> GetCategoryByIdAsync(int id);
       Task<Category> CreateAsync(Category category);
       Task<bool> UpdateAsync(Category category);
       Task<bool> DeleteAsync(int id);
    }
}