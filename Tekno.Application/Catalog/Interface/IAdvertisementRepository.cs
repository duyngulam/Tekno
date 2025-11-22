using System.Collections.Generic;
using System.Threading.Tasks;
using Tekno.Application.Common;
using Tekno.Application.Common.Paging;
using Tekno.Domain.Catalog;

namespace Tekno.Application.Catalog.Interface
{
    public interface IAdvertisementRepository
    {
        Task<ProductAdvertisement?> GetByIdAsync(int id);
        Task<PagedResult<ProductAdvertisement>> GetPagedAsync(
            string? position,
            bool? isActive,
            bool onlyCurrentlyActive,
            PagingParams paging);
        Task<List<ProductAdvertisement>> GetActiveByPositionAsync(string position);
        Task<List<ProductAdvertisement>> GetCurrentlyActiveAsync();
        Task<ProductAdvertisement> CreateAsync(ProductAdvertisement advertisement);
        Task<ProductAdvertisement> UpdateAsync(ProductAdvertisement advertisement);
        Task<bool> DeleteAsync(int id);
    }
}
