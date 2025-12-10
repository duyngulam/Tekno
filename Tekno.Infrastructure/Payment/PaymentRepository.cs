using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tekno.Application.Common.Paging;
using Tekno.Application.Payment.Interfaces;
using Tekno.Infrastructure.Persistence;

namespace Tekno.Infrastructure.Payment
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly AppDbContext _context;

        public PaymentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Domain.Payment.Payment?> GetByIdAsync(int id)
        {
            return await _context.Set<Domain.Payment.Payment>()
                .Include(p => p.Order)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Domain.Payment.Payment?> GetByTransactionIdAsync(string transactionId)
        {
            return await _context.Set<Domain.Payment.Payment>()
                .Include(p => p.Order)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.TransactionId == transactionId);
        }

        public async Task<List<Domain.Payment.Payment>> GetByOrderIdAsync(int orderId)
        {
            return await _context.Set<Domain.Payment.Payment>()
                .Where(p => p.OrderId == orderId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<Domain.Payment.Payment>> GetByUserIdAsync(int userId)
        {
            return await _context.Set<Domain.Payment.Payment>()
                .Include(p => p.Order)
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.CreatedAt)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<PagedResult<Domain.Payment.Payment>> GetPagedAsync(
            int? userId = null,
            Domain.Payment.PaymentStatus? status = null,
            Domain.Payment.PaymentGateway? gateway = null,
            string? search = null,
            PagingParams paging = null)
        {
            paging ??= new PagingParams(1, 20);

            var query = _context.Set<Domain.Payment.Payment>()
                .Include(p => p.Order)
                .AsNoTracking();

            // Apply filters
            if (userId.HasValue)
            {
                query = query.Where(p => p.UserId == userId.Value);
            }

            if (status.HasValue)
            {
                query = query.Where(p => p.Status == status.Value);
            }

            if (gateway.HasValue)
            {
                query = query.Where(p => p.Gateway == gateway.Value);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(p =>
                    p.TransactionId.Contains(search) ||
                    p.Order.OrderNumber.Contains(search));
            }

            // Order by created date descending (newest first)
            query = query.OrderByDescending(p => p.CreatedAt);

            // Get total count
            var totalRecords = await query.CountAsync();

            // Apply pagination
            var data = await query
                .Skip((paging.Page - 1) * paging.PageSize)
                .Take(paging.PageSize)
                .ToListAsync();

            return new PagedResult<Domain.Payment.Payment>(data, totalRecords, paging.Page, paging.PageSize);
        }

        public async Task<Domain.Payment.Payment> CreateAsync(Domain.Payment.Payment payment)
        {
            await _context.Set<Domain.Payment.Payment>().AddAsync(payment);
            await _context.SaveChangesAsync();
            return payment;
        }

        public async Task<Domain.Payment.Payment> UpdateAsync(Domain.Payment.Payment payment)
        {
            _context.Set<Domain.Payment.Payment>().Update(payment);
            await _context.SaveChangesAsync();
            return payment;
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }
    }
}
