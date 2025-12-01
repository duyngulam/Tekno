using System.Threading.Tasks;
using Tekno.Application.Payment.Models;
using Tekno.Domain.Payment;

namespace Tekno.Application.Payment.Interfaces
{
    /// <summary>
    /// Payment gateway interface - Strategy Pattern
    /// Implement this interface to add new payment gateways
    /// </summary>
    public interface IPaymentGateway
    {
        /// <summary>
        /// Gateway identifier
        /// </summary>
        PaymentGateway Gateway { get; }

        /// <summary>
        /// Initialize payment and get payment URL/token
        /// </summary>
        Task<PaymentInitResult> InitiatePaymentAsync(PaymentRequest request);

        /// <summary>
        /// Verify payment callback/webhook
        /// </summary>
        Task<PaymentVerificationResult> VerifyPaymentAsync(string transactionId, object callbackData);

        /// <summary>
        /// Refund a completed payment
        /// </summary>
        Task<RefundResult> RefundPaymentAsync(string transactionId, decimal amount, string reason);
    }
}
