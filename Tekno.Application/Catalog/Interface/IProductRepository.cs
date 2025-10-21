using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tekno.Domain.Catalog;
using Tekno.Application.Common.Paging;

namespace Tekno.Application.Catalog.Interface
{
    public interface IProductRepository
    {
        Task<PagedResult<Product?>> GetPagedProductAsync(string? categorySlug,
            string? brandSlug,
            string? search,
            string? sort,
            string? minPrice,
            string? maxPrice,
            int pageNumber,
            int pageSize);
        Task<Product?> GetProductBySlugAsync(string slug);
    }
}
