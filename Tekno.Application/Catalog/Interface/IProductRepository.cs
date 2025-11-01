using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tekno.Application.Common;
using Tekno.Application.Common.Paging;
using Tekno.Domain.Catalog;

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
            PagingParams paging);
        Task<Product?> GetProductBySlugAsync(string slug);
        Task<IEnumerable<Product>> GetAllProductsWithDetailAsync();
    }
}
