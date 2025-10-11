using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tekno.Application.Auth.Interfaces
{
    public interface IProductRepository
    {
        Task<(IList<ProductListDto> Items, int Total)> GetProductsAsync(ProductQuery query, CancellationToken ct);
        Task<ProductDetailDto?> GetProductBySlugAsync(string slug);
        Task<IList<ProductDto>> GetFeaturedAsync(int page, int size);

    }
}
