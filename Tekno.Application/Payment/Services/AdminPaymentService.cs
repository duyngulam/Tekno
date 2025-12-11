using AutoMapper;
using System.Threading.Tasks;
using Tekno.Application.Common.Paging;
using Tekno.Application.Payment.DTOs;
using Tekno.Application.Payment.Interfaces;
using Tekno.Domain.Payment;

namespace Tekno.Application.Payment.Services
{
    /// <summary>
    /// Admin payment service for managing and viewing transactions
    /// </summary>
    public class AdminPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IMapper _mapper;

        public AdminPaymentService(IPaymentRepository paymentRepository, IMapper mapper)
        {
            _paymentRepository = paymentRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Get paged payment transactions with filters
        /// </summary>
        public async Task<PagedResult<PaymentStatusDto>> GetPagedTransactionsAsync(
            int? userId = null,
            PaymentStatus? status = null,
            PaymentGateway? gateway = null,
            string? search = null,
            int page = 1,
            int pageSize = 20)
        {
            var paging = new PagingParams(page, pageSize);
            var result = await _paymentRepository.GetPagedAsync(userId, status, gateway, search, paging);

            // Map to DTOs
            var dtos = _mapper.Map<List<PaymentStatusDto>>(result.Data);

            return new PagedResult<PaymentStatusDto>(dtos, result.TotalRecords, result.Page, result.PageSize);
        }

        /// <summary>
        /// Get payment details by ID
        /// </summary>
        public async Task<PaymentStatusDto?> GetPaymentByIdAsync(int id)
        {
            var payment = await _paymentRepository.GetByIdAsync(id);
            return payment == null ? null : _mapper.Map<PaymentStatusDto>(payment);
        }

        /// <summary>
        /// Get payment details by transaction ID
        /// </summary>
        public async Task<PaymentStatusDto?> GetPaymentByTransactionIdAsync(string transactionId)
        {
            var payment = await _paymentRepository.GetByTransactionIdAsync(transactionId);
            return payment == null ? null : _mapper.Map<PaymentStatusDto>(payment);
        }
    }
}
