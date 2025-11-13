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
        Task<Product?> GetProductByIdAsync(int id);
        Task<bool> IsProductExistBySlug(string slug);
        Task<IEnumerable<Product>> GetAllProductsWithDetailAsync();
        Task<Product> AddProductAsync(Product newProduct);
        Task<Product> UpdateProductAsync(Product product);
        Task DeleteProductAsync(Product product);
    }
}
