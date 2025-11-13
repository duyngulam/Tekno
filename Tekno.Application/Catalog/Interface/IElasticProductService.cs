using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tekno.Application.Catalog.DTOs.Products;
using Tekno.Application.Common.Paging;
using Tekno.Domain.Catalog;

namespace Tekno.Application.Catalog.Interface
{
    public interface IElasticProductService
    {
        Task IndexProductAsync(ProductSummaryDto product);
        Task DeleteProductAsync(int id);
        Task<PagedResult<ProductSummaryDto>> SearchProductsAsync(
            string? keyword,
            string? categorySlug,
            string? brandSlug,
            Dictionary<string, string>? filters,
            decimal? minPrice,
            decimal? maxPrice,
            string? sort,
            int page,
            int pageSize);
        Task<bool> IsProductExistBySlug(string slug);
    }
}
