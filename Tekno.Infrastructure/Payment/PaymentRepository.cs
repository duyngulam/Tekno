using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
