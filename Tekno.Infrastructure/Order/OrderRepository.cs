using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tekno.Application.Common.Paging;
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
                .Include(o => o.Payment)
                .Include(o => o.ShippingAddress)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<Domain.Order.Order?> GetByOrderNumberAsync(string orderNumber)
        {
            return await _context.Set<Domain.Order.Order>()
                .Include(o => o.Items)
                .Include(o => o.Payment)
                .Include(o => o.ShippingAddress)
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);
        }

        public async Task<List<Domain.Order.Order>> GetByUserIdAsync(int userId)
        {
            return await _context.Set<Domain.Order.Order>()
                .Include(o => o.Items)
                .Include(o => o.Payment)
                .Include(o => o.ShippingAddress)
                .AsNoTracking()
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Domain.Order.Order>> GetUserOrdersAsync(int userId)
        {
            return await GetByUserIdAsync(userId);
        }

        public async Task<List<Domain.Order.Order>> GetUserCompletedOrdersAsync(int userId)
        {
            return await _context.Set<Domain.Order.Order>()
                .Include(o => o.Items)
                .Include(o => o.Payment)
                .Include(o => o.ShippingAddress)
                .AsNoTracking()
                .Where(o => o.UserId == userId && o.Status == OrderStatus.Completed)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> HasUserPurchasedProductAsync(int userId, int productId)
        {
            return await _context.Set<Domain.Order.Order>()
                .AsNoTracking()
                .Where(o => o.UserId == userId &&( o.Status == OrderStatus.Completed ||o.Status ==OrderStatus.Processing))
                .SelectMany(o => o.Items)
                .AnyAsync(item => item.ProductId == productId);
        }

        public async Task<Domain.Order.Order?> GetUserOrderForProductAsync(int userId, int productId)
        {
            return await _context.Set<Domain.Order.Order>()
                .Include(o => o.Items)
                .Include(o => o.ShippingAddress)
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

        public async Task<Domain.Order.Order> UpdateAsync(Domain.Order.Order order)
        {
            _context.Set<Domain.Order.Order>().Update(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<PagedResult<Domain.Order.Order>> GetPagedAsync(
            int? userId = null,
            OrderStatus? status = null,
            PagingParams? paging = null)
        {
            var query = _context.Set<Domain.Order.Order>()
                .Include(o => o.Items)
                .Include(o => o.Payment)
                .Include(o => o.ShippingAddress)
                .AsNoTracking()
                .AsQueryable();

            // Filter by user
            if (userId.HasValue)
            {
                query = query.Where(o => o.UserId == userId.Value);
            }

            // Filter by status
            if (status.HasValue)
            {
                query = query.Where(o => o.Status == status.Value);
            }

            // Order by most recent first
            query = query.OrderByDescending(o => o.CreatedAt);

            // Get total count
            var totalRecords = await query.CountAsync();

            // Apply pagination
            var pagingParams = paging ?? new PagingParams(1, 20);
            var data = await query
                .Skip((pagingParams.Page - 1) * pagingParams.PageSize)
                .Take(pagingParams.PageSize)
                .ToListAsync();

            return new PagedResult<Domain.Order.Order>(
                data,
                totalRecords,
                pagingParams.Page,
                pagingParams.PageSize);
        }
    }
}
