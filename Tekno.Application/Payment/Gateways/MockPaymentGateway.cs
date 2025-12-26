using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Tekno.Application.Common.Interfaces;
using Tekno.Application.Payment.Interfaces;
using Tekno.Application.Payment.Models;
using Tekno.Domain.Payment;

namespace Tekno.Application.Payment.Gateways
{
    /// <summary>
    /// Mock payment gateway for testing - Vietnam (VND) version
    /// Simulates real gateway behavior including automatic callback processing
    /// </summary>
    public class MockPaymentGateway : IPaymentGateway
    {
        private readonly IAppLogger<MockPaymentGateway> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public PaymentGateway Gateway => PaymentGateway.Mock;

        public MockPaymentGateway(IAppLogger<MockPaymentGateway> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<PaymentInitResult> InitiatePaymentAsync(PaymentRequest request)
        {
            _logger.LogInformation("Mock payment initiated for order {OrderNumber}, amount {Amount:N0} {Currency}",
                request.OrderNumber, request.Amount, request.Currency);

            // Simulate transaction ID
            var guidPart = Guid.NewGuid().ToString("N")[..8].ToUpper();
            var transactionId = $"MOCK-{DateTime.UtcNow:yyyyMMddHHmmss}-{guidPart}";

            // Generate mock signature
            var signature = $"MOCK-SIG-{Guid.NewGuid().ToString("N")[..16].ToUpper()}";

            // Mock payment URL with Vietnamese parameters
            //var paymentUrl = $"{request.ReturnUrl}?transactionId={transactionId}&status=success&amount={request.Amount}&currency={request.Currency}";
            //mock will redirect to returnUrl with orderId only ( frontend)
            var paymentUrl = $"{request.ReturnUrl}";
            // Log Vietnamese pricing for testing
            var amountFormatted = request.Amount.ToString("N0"); // Format: 1,500,000
            _logger.LogInformation("Mock payment URL generated: Amount = {Amount} VND", amountFormatted);

            // Automatically trigger the payment callback (simulating gateway webhook)
            // This runs in the background and doesn't block the response
            _ = Task.Run(async () =>
            {
                try
                {
                    // Wait a bit to simulate payment processing
                    await Task.Delay(2000); // 2 seconds delay

                    _logger.LogInformation("Mock gateway: Triggering automatic callback for transaction {TransactionId}", transactionId);

                    // Call the payment callback endpoint
                    var httpClient = _httpClientFactory.CreateClient();
                    
                    // Important: Set a reasonable timeout to prevent hanging
                    httpClient.Timeout = TimeSpan.FromSeconds(10);
                    
                    var callbackDto = new
                    {
                        TransactionId = transactionId,
                        Status = "success",
                        Signature = signature,
                        CallbackData = new
                        {
                            Amount = request.Amount,
                            Currency = request.Currency,
                            Status = "success",
                            Signature = signature
                        }
                    };

                    // Try to post callback - use absolute URL from request.CallbackUrl
                    var response = await httpClient.PostAsJsonAsync(request.CallbackUrl, callbackDto);

                    if (response.IsSuccessStatusCode)
                    {
                        _logger.LogInformation("Mock gateway: Callback successful for transaction {TransactionId}", transactionId);
                    }
                    else
                    {
                        _logger.LogWarning("Mock gateway: Callback failed for transaction {TransactionId}, Status: {StatusCode}",
                            transactionId, response.StatusCode);
                    }
                }
                catch (HttpRequestException ex)
                {
                    _logger.LogWarning(ex, "Mock gateway: HTTP error triggering callback for transaction {TransactionId}. " +
                        "This is expected if running in development without the API running. " +
                        "Callback URL: {CallbackUrl}", 
                        transactionId, request.CallbackUrl);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Mock gateway: Error triggering callback for transaction {TransactionId}", transactionId);
                }
            });

            return await Task.FromResult(new PaymentInitResult
            {
                Success = true,
                TransactionId = transactionId,
                PaymentUrl = paymentUrl,
                GatewayResponse = new
                {
                    message = "Mock payment - auto callback will be triggered in 2 seconds (VND)",
                    transactionId,
                    amount = request.Amount,
                    currency = request.Currency,
                    amountFormatted = $"{amountFormatted} VND",
                    orderNumber = request.OrderNumber,
                    method = request.Method.ToString(),
                    timestamp = DateTime.UtcNow,
                    description = $"Payment for order {request.OrderNumber} - {amountFormatted} VND",
                    callbackUrl = request.CallbackUrl,
                    signature = signature,
                    note = "? Automatic callback enabled - payment will be completed automatically in 2 seconds. " +
                           "If callback fails, you can manually call POST /api/payment/callback"
                }
            });
        }

        public Task<PaymentVerificationResult> VerifyPaymentAsync(string transactionId, object callbackData)
        {
            _logger.LogInformation("Mock payment verification for transaction {TransactionId}", transactionId);

            // Parse callback data - In real implementation, this would verify signature and extract data
            var result = ParseCallbackData(transactionId, callbackData);

            _logger.LogInformation("Mock verification result: IsValid={IsValid}, IsSuccessful={IsSuccessful}, Amount={Amount} {Currency}",
                result.IsValid, result.IsSuccessful, result.Amount, result.Currency);

            return Task.FromResult(result);
        }

        public Task<RefundResult> RefundPaymentAsync(string transactionId, decimal amount, string reason)
        {
            _logger.LogInformation("Mock refund for transaction {TransactionId}, amount {Amount:N0} VND, reason: {Reason}",
                transactionId, amount, reason);

            var guidPart = Guid.NewGuid().ToString("N")[..8].ToUpper();
            var refundId = $"REFUND-{DateTime.UtcNow:yyyyMMddHHmmss}-{guidPart}";

            return Task.FromResult(new RefundResult
            {
                Success = true,
                RefundId = refundId,
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

        /// <summary>
        /// Parse and verify callback data from payment gateway
        /// This simulates what real gateways do: signature verification and data extraction
        /// </summary>
        private PaymentVerificationResult ParseCallbackData(string transactionId, object callbackData)
        {
            try
            {
                // In real implementation, this would:
                // 1. Extract signature from callback data
                // 2. Reconstruct signature using secret key and payload
                // 3. Compare signatures (HMAC-SHA256)
                // 4. Verify transaction exists in gateway database
                // 5. Extract payment details (amount, status, etc.)

                // For mock, we'll parse the callback data if it's provided
                if (callbackData != null)
                {
                    var json = JsonSerializer.Serialize(callbackData);
                    var data = JsonSerializer.Deserialize<MockCallbackData>(json);

                    if (data != null && !string.IsNullOrWhiteSpace(data.Status))
                    {
                        // Only process if Status is actually provided
                        // Simulate signature verification
                        var isValidSignature = VerifyMockSignature(transactionId, data.Status, data.Signature);

                        if (!isValidSignature)
                        {
                            _logger.LogWarning("Mock: Invalid signature for transaction {TransactionId}", transactionId);
                            return new PaymentVerificationResult
                            {
                                IsValid = false,
                                IsSuccessful = false,
                                TransactionId = transactionId,
                                ErrorMessage = "Invalid signature - possible tampering detected",
                                GatewayResponse = callbackData
                            };
                        }

                        // Parse status
                        var isSuccess = data.Status?.ToLower() == "success" || data.Status?.ToLower() == "completed";

                        return new PaymentVerificationResult
                        {
                            IsValid = true,
                            IsSuccessful = isSuccess,
                            TransactionId = transactionId,
                            Amount = data.Amount ?? 0,
                            Currency = data.Currency ?? "VND",
                            ErrorMessage = isSuccess ? null : (data.ErrorMessage ?? "Payment failed"),
                            GatewayResponse = new
                            {
                                message = "Mock verification successful",
                                transactionId,
                                status = data.Status,
                                amount = data.Amount,
                                currency = data.Currency,
                                verified = true,
                                timestamp = DateTime.UtcNow,
                                signatureValid = true
                            }
                        };
                    }
                }

                // Default: auto-success for testing if no valid callback data provided
                _logger.LogInformation("Mock: No valid callback data provided, defaulting to success");
                return new PaymentVerificationResult
                {
                    IsValid = true,
                    IsSuccessful = true,
                    TransactionId = transactionId,
                    Amount = 0,
                    Currency = "VND",
                    GatewayResponse = new
                    {
                        message = "Mock verification successful (default)",
                        transactionId,
                        verified = true,
                        currency = "VND",
                        timestamp = DateTime.UtcNow,
                        note = "No callback data provided - auto-success for testing"
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Mock: Error parsing callback data for transaction {TransactionId}", transactionId);
                return new PaymentVerificationResult
                {
                    IsValid = false,
                    IsSuccessful = false,
                    TransactionId = transactionId,
                    ErrorMessage = $"Failed to parse callback data: {ex.Message}",
                    GatewayResponse = callbackData
                };
            }
        }

        /// <summary>
        /// Simulate signature verification
        /// In real implementation, this would use HMAC-SHA256 with secret key
        /// </summary>
        private bool VerifyMockSignature(string transactionId, string? status, string? signature)
        {
            // For mock, we'll accept any signature or no signature
            // In real implementation:
            // 1. Reconstruct payload string: transactionId + status + timestamp + amount
            // 2. Generate HMAC-SHA256 hash using secret key
            // 3. Compare with provided signature

            if (string.IsNullOrEmpty(signature))
            {
                // No signature provided - accept for testing
                return true;
            }

            // Simple mock validation: signature should start with "MOCK-SIG-"
            return signature.StartsWith("MOCK-SIG-");
        }

        /// <summary>
        /// Mock callback data structure
        /// Represents what a real payment gateway would send
        /// </summary>
        private class MockCallbackData
        {
            public string? Status { get; set; }
            public string? Signature { get; set; }
            public decimal? Amount { get; set; }
            public string? Currency { get; set; }
            public string? ErrorMessage { get; set; }
            public string? ErrorCode { get; set; }
        }
    }
}
