using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tekno.Application.Order.Interface;
using Tekno.Domain.Order;
using Tekno.Infrastructure.Persistence;

namespace Tekno.Infrastructure.Order
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;

        public OrderRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Domain.Order.Order?> GetByIdAsync(int id)
        {
            return await _context.Set<Domain.Order.Order>()
                .Include(o => o.Items)
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<List<Domain.Order.Order>> GetUserOrdersAsync(int userId)
        {
            return await _context.Set<Domain.Order.Order>()
                .Include(o => o.Items)
                .AsNoTracking()
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Domain.Order.Order>> GetUserCompletedOrdersAsync(int userId)
        {
            return await _context.Set<Domain.Order.Order>()
                .Include(o => o.Items)
                .AsNoTracking()
                .Where(o => o.UserId == userId && o.Status == OrderStatus.Completed)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> HasUserPurchasedProductAsync(int userId, int productId)
        {
            return await _context.Set<Domain.Order.Order>()
                .AsNoTracking()
                .Where(o => o.UserId == userId && o.Status == OrderStatus.Completed)
                .SelectMany(o => o.Items)
                .AnyAsync(item => item.ProductId == productId);
        }

        public async Task<Domain.Order.Order?> GetUserOrderForProductAsync(int userId, int productId)
        {
            return await _context.Set<Domain.Order.Order>()
                .Include(o => o.Items)
                .AsNoTracking()
                .Where(o => o.UserId == userId && o.Status == OrderStatus.Completed)
                .Where(o => o.Items.Any(item => item.ProductId == productId))
                .OrderByDescending(o => o.CompletedAt)
                .FirstOrDefaultAsync();
        }

        public async Task<Domain.Order.Order> CreateAsync(Domain.Order.Order order)
        {
            _context.Set<Domain.Order.Order>().Add(order);
            await _context.SaveChangesAsync();
            return order;
        }
    }
}
