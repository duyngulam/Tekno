using System;
using System.Threading.Tasks;
using Tekno.Application.Common.Interfaces;
using Tekno.Application.Payment.Interfaces;
using Tekno.Application.Payment.Models;
using Tekno.Domain.Payment;

namespace Tekno.Application.Payment.Gateways
{
    /// <summary>
    /// Mock payment gateway for testing - Vietnam (VND) version
    /// Always succeeds immediately with Vietnamese pricing
    /// </summary>
    public class MockPaymentGateway : IPaymentGateway
    {
        private readonly IAppLogger<MockPaymentGateway> _logger;

        public PaymentGateway Gateway => PaymentGateway.Mock;

        public MockPaymentGateway(IAppLogger<MockPaymentGateway> logger)
        {
            _logger = logger;
        }

        public Task<PaymentInitResult> InitiatePaymentAsync(PaymentRequest request)
        {
            _logger.LogInformation("Mock payment initiated for order {OrderNumber}, amount {Amount:N0} {Currency}",
                request.OrderNumber, request.Amount, request.Currency);

            // Simulate transaction ID
            var transactionId = $"MOCK-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid():N[..8].ToUpper()}";

            // Mock payment URL with Vietnamese parameters
            var paymentUrl = $"{request.ReturnUrl}?transactionId={transactionId}&status=success&amount={request.Amount}&currency={request.Currency}";

            // Log Vietnamese pricing for testing
            var amountFormatted = request.Amount.ToString("N0"); // Format: 1,500,000
            _logger.LogInformation("Mock payment URL generated: Amount = {Amount} VND", amountFormatted);

            return Task.FromResult(new PaymentInitResult
            {
                Success = true,
                TransactionId = transactionId,
                PaymentUrl = paymentUrl,
                GatewayResponse = new
                {
                    message = "Mock payment - auto success (VND)",
                    transactionId,
                    amount = request.Amount,
                    currency = request.Currency,
                    amountFormatted = $"{amountFormatted} VND",
                    orderNumber = request.OrderNumber,
                    method = request.Method.ToString(),
                    timestamp = DateTime.UtcNow,
                    description = $"Payment for order {request.OrderNumber} - {amountFormatted} VND"
                }
            });
        }

        public Task<PaymentVerificationResult> VerifyPaymentAsync(string transactionId, object callbackData)
        {
            _logger.LogInformation("Mock payment verification for transaction {TransactionId}", transactionId);

            // Mock gateway always succeeds with VND
            // In real implementation, you'd parse the callback data
            return Task.FromResult(new PaymentVerificationResult
            {
                IsValid = true,
                IsSuccessful = true,
                TransactionId = transactionId,
                Amount = 0, // Would be populated from callback data
                Currency = "VND",
                GatewayResponse = new
                {
                    message = "Mock verification successful",
                    transactionId,
                    verified = true,
                    currency = "VND",
                    timestamp = DateTime.UtcNow
                }
            });
        }

        public Task<RefundResult> RefundPaymentAsync(string transactionId, decimal amount, string reason)
        {
            _logger.LogInformation("Mock refund for transaction {TransactionId}, amount {Amount:N0} VND, reason: {Reason}",
                transactionId, amount, reason);

            return Task.FromResult(new RefundResult
            {
                Success = true,
                RefundId = $"REFUND-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid():N[..8].ToUpper()}",
                RefundedAmount = amount,
                GatewayResponse = new
                {
                    message = "Mock refund successful (VND)",
                    transactionId,
                    refundAmount = amount,
                    refundAmountFormatted = $"{amount:N0} VND",
                    currency = "VND",
                    reason,
                    timestamp = DateTime.UtcNow,
                    status = "refunded"
                }
            });
        }
    }
}
