using System.Collections.Generic;
using System.Threading.Tasks;
using Tekno.Domain.Order;

namespace Tekno.Application.Order.Interface
{
    /// <summary>
    /// Simplified order repository for purchase verification
    /// Full order repository can be expanded when implementing order system
    /// </summary>
    public interface IOrderRepository
    {
        Task<Domain.Order.Order?> GetByIdAsync(int id);
        Task<List<Domain.Order.Order>> GetUserOrdersAsync(int userId);
        Task<List<Domain.Order.Order>> GetUserCompletedOrdersAsync(int userId);
        Task<bool> HasUserPurchasedProductAsync(int userId, int productId);
        Task<Domain.Order.Order?> GetUserOrderForProductAsync(int userId, int productId);
        Task<Domain.Order.Order> CreateAsync(Domain.Order.Order order);
    }
}
