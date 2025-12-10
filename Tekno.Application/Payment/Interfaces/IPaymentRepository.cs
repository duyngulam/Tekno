using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using Tekno.Application.Common.Paging;

namespace Tekno.Application.Payment.Interfaces
{
    public interface IPaymentRepository
    {
        Task<Domain.Payment.Payment?> GetByIdAsync(int id);
        Task<Domain.Payment.Payment?> GetByTransactionIdAsync(string transactionId);
        Task<List<Domain.Payment.Payment>> GetByOrderIdAsync(int orderId);
        Task<List<Domain.Payment.Payment>> GetByUserIdAsync(int userId);
        Task<PagedResult<Domain.Payment.Payment>> GetPagedAsync(
            int? userId = null,
            Domain.Payment.PaymentStatus? status = null,
            Domain.Payment.PaymentGateway? gateway = null,
            string? search = null,
            PagingParams paging = null);
        Task<Domain.Payment.Payment> CreateAsync(Domain.Payment.Payment payment);
        Task<Domain.Payment.Payment> UpdateAsync(Domain.Payment.Payment payment);
        Task<IDbContextTransaction> BeginTransactionAsync();
    }
}
