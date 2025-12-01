using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;

namespace Tekno.Application.Payment.Interfaces
{
    public interface IPaymentRepository
    {
        Task<Domain.Payment.Payment?> GetByIdAsync(int id);
        Task<Domain.Payment.Payment?> GetByTransactionIdAsync(string transactionId);
        Task<List<Domain.Payment.Payment>> GetByOrderIdAsync(int orderId);
        Task<List<Domain.Payment.Payment>> GetByUserIdAsync(int userId);
        Task<Domain.Payment.Payment> CreateAsync(Domain.Payment.Payment payment);
        Task<Domain.Payment.Payment> UpdateAsync(Domain.Payment.Payment payment);
        Task<IDbContextTransaction> BeginTransactionAsync();
    }
}
