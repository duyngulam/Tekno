using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tekno.Domain.Catalog;

namespace Tekno.Application.Catalog.Interface
{
    public interface IBrandRepository
    {
        Task<List<Brand?>> GetAllBrandsAsync();
        Task<Brand?> GetBrandBySlugAsync(string slug);
        Task<Brand> CreateAsync(Brand brand);
        Task<bool> UpdateAsync(Brand brand);
        Task<bool> DeleteAsync(int id);
    }
}
