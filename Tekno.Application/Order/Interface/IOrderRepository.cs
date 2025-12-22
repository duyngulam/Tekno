using System.Collections.Generic;
using System.Threading.Tasks;
using Tekno.Application.Common.Paging;
using Tekno.Domain.Order;

namespace Tekno.Application.Order.Interface
{
    /// <summary>
    /// Order repository for order management and history
    /// </summary>
    public interface IOrderRepository
    {
        Task<Domain.Order.Order?> GetByIdAsync(int id);
        Task<Domain.Order.Order?> GetByOrderNumberAsync(string orderNumber);
        Task<List<Domain.Order.Order>> GetByUserIdAsync(int userId);
        Task<List<Domain.Order.Order>> GetUserOrdersAsync(int userId);
        Task<List<Domain.Order.Order>> GetUserCompletedOrdersAsync(int userId);
        Task<bool> HasUserPurchasedProductAsync(int userId, int productId);
        Task<Domain.Order.Order?> GetUserOrderForProductAsync(int userId, int productId);
        Task<Domain.Order.Order> CreateAsync(Domain.Order.Order order);
        Task<Domain.Order.Order> UpdateAsync(Domain.Order.Order order);
        
        // Pagination support for order history
        Task<PagedResult<Domain.Order.Order>> GetPagedAsync(
            int? userId = null,
            OrderStatus? status = null,
            PagingParams? paging = null);
    }
}
