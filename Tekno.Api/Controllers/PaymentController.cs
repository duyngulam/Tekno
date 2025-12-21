using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using Tekno.Api.Commons.Responses;
using Tekno.Application.Common.Exceptions;
using Tekno.Application.Common.Paging;
using Tekno.Application.Payment.DTOs;
using Tekno.Application.Payment.Services;

namespace Tekno.Api.Controllers
{
    /// <summary>
    /// Payment processing endpoints
    /// </summary>
    [ApiController]
    [Route("api/payment")]
    [ValidationFilter]
    public class PaymentController : ControllerBase
    {
        private readonly PaymentService _paymentService;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(PaymentService paymentService, ILogger<PaymentController> logger)
        {
            _paymentService = paymentService;
            _logger = logger;
        }

        /// <summary>
        /// Get current user's payment history (completed payments only)
        /// </summary>
        /// <remarks>
        /// **For Support/Debugging Purpose** - Shows completed payment transactions
        /// 
        /// **Note:** For order tracking and delivery status, use `/api/orders/history` instead.
        /// 
        /// This endpoint returns:
        /// - Only completed payment transactions
        /// - Payment gateway used (VNPay, Stripe, etc.)
        /// - Transaction IDs for support
        /// - Payment timestamps
        /// 
        /// **Use this for:**
        /// - Payment verification
        /// - Financial records
        /// - Support inquiries about payments
        /// 
        /// **For user-facing order tracking, use:**
        /// - GET /api/orders/history (recommended)
        /// 
        /// Example:
        ///     GET /api/payment/my-payments?page=1&amp;pageSize=20
        /// 
        /// Returns only successful payment transactions.
        /// </remarks>
        [HttpGet("my-payments")]
        [Authorize]
        public async Task<IActionResult> GetMyPayments(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                
                // Only show completed payments (for payment verification/support)
                var payments = await _paymentService.GetUserCompletedPaymentsAsync(userId, page, pageSize);

                return Ok(ApiResponse<PagedResult<PaymentStatusDto>>.Ok(
                    payments, 
                    "Payment history retrieved successfully. For order tracking, use /api/orders/history"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get payment history for user");
                return StatusCode(500, ApiResponse<string>.Fail($"Failed to get payment history: {ex.Message}"));
            }
        }

        /// <summary>
        /// Process payment - Create order and initiate payment
        /// </summary>
        /// <remarks>
        /// Workflow:
        /// 1. Validates user's cart
        /// 2. Creates order from selected cart items (or all items if not specified)
        /// 3. Initiates payment with selected gateway
        /// 4. Returns payment URL to redirect customer
        /// 5. Removes checked out items from cart (partial or full)
        /// 
        /// VNPay Flow Details:
        /// - Frontend sends returnUrl (e.g., "http://localhost:3000/payment/result")
        /// - Backend sends vnp_ReturnUrl to VNPay (e.g., "https://api.com/api/v1/payments/vnpay-return?frontendUrl=...")
        /// - After payment: Customer Browser → VNPay → Backend ReturnUrl → Frontend returnUrl
        /// - Simultaneously: VNPay Server → Backend IPN (reliable callback)
        /// 
        /// Two Callbacks:
        /// 1. IPN (Server-to-Server): Reliable, updates database, configured in VNPay portal
        /// 2. ReturnUrl (Browser): Quick UX, shows result to customer, sent in payment request
        /// 
        /// Example request (Full cart payment):
        /// 
        ///     POST /api/payment/process
        ///     {
        ///       "shippingAddressId": 1,
        ///       "gateway": 0,
        ///       "method": 1,
        ///       "note": "Please deliver after 5pm",
        ///       "returnUrl": "http://localhost:3000/payment/result"
        ///     }
        /// 
        /// Example request (Partial cart payment with selected items):
        /// 
        ///     POST /api/payment/process
        ///     {
        ///       "shippingAddressId": 1,
        ///       "gateway": 0,
        ///       "method": 1,
        ///       "note": "Buy 2 iPhone 15 Pro Max",
        ///       "returnUrl": "http://localhost:3000/payment/result",
        ///       "selectedItems": [
        ///         {
        ///           "variantId": 11,
        ///           "quantity": 2
        ///         }
        ///       ]
        ///     }
        /// 
        /// Payment Gateways:
        /// - 0 = Mock (for testing) ? Available
        /// - 1 = Stripe (credit/debit cards) - Not implemented yet
        /// - 2 = PayPal - Not implemented yet
        /// - 3 = VNPay (Vietnam) ? Available
        /// - 4 = MoMo (Vietnam e-wallet) - Not implemented yet
        /// - 5 = ZaloPay (Vietnam e-wallet) - Not implemented yet
        /// 
        /// Payment Methods:
        /// - 1 = CreditCard
        /// - 2 = DebitCard
        /// - 3 = BankTransfer
        /// - 4 = EWallet
        /// - 5 = Cash (COD)
        /// 
        /// Response includes paymentUrl - redirect customer to complete payment.
        /// For Mock gateway, payment auto-succeeds and triggers callback automatically.
        /// </remarks>
        [HttpPost("process")]
        [Authorize]
        public async Task<IActionResult> ProcessPayment([FromBody] PaymentRequestDto request)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var result = await _paymentService.ProcessPaymentAsync(userId, request);

                return Ok(ApiResponse<PaymentResponseDto>.Ok(result, "Payment initiated successfully. Redirect to payment URL."));
            }
            catch (NotSupportedException ex)
            {
                // Gateway not available - return user-friendly error
                _logger.LogWarning(ex, "Payment gateway not available: {Gateway}", request.Gateway);
                return BadRequest(ApiResponse<string>.Fail(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Payment processing failed");
                return StatusCode(500, ApiResponse<string>.Fail($"Payment processing failed: {ex.Message}"));
            }
        }

        /// <summary>
        /// VNPay IPN (Instant Payment Notification) - Server-to-server callback
        /// 
        /// IMPORTANT: This is PRIMARY callback for payment status updates
        /// - Called by VNPay SERVER (not customer browser)
        /// - Configured in VNPay Merchant Portal (separate from vnp_ReturnUrl)
        /// - Reliable with retry mechanism (up to 10 times, every 5 minutes)
        /// - MUST update database here - this is source of truth
        /// - MUST respond with JSON: {"RspCode":"00","Message":"Confirm Success"}
        /// 
        /// Retry Logic:
        /// - RspCode "00" or "02" → VNPay stops retrying (success)
        /// - RspCode "01", "04", "97", "99" or timeout → VNPay retries
        /// 
        /// vs ReturnUrl: IPN is reliable server callback, ReturnUrl is browser redirect for UX
        /// </summary>
        [HttpGet("vnpay/IPN")]
        [HttpGet("/api/v1/payments/vnpay-ipn")]
        [AllowAnonymous]
        public async Task<IActionResult> VNPayIPN()
        {
            try
            {
                _logger.LogInformation("VNPay IPN received: {QueryString}", Request.QueryString.Value);

                // Read query string parameters into dictionary
                var queryDict = Request.Query.ToDictionary(k => k.Key, v => v.Value.ToString());

                // Extract transaction id
                string transactionId = queryDict.TryGetValue("vnp_TxnRef", out var txn) ? txn : string.Empty;

                if (string.IsNullOrEmpty(transactionId))
                {
                    _logger.LogWarning("VNPay IPN called without transaction id");
                    return Ok(new { RspCode = "99", Message = "Missing transaction id" });
                }

                var callback = new PaymentCallbackDto
                {
                    TransactionId = transactionId,
                    CallbackData = queryDict
                };

                // Process payment callback (service handles idempotency)
                var result = await _paymentService.HandlePaymentCallbackAsync(callback);

                _logger.LogInformation("VNPay IPN processed successfully for {TransactionId}. Status: {Status}", 
                    transactionId, result.Status);


                // Respond to VNPay according to their specification
                return Ok(new { RspCode = "00", Message = "Confirm Success" });
            }
            catch (NotFoundException)
            {
                _logger.LogWarning("VNPay IPN: Payment not found. Query: {Query}", Request.QueryString.Value);
                return Ok(new { RspCode = "02", Message = "Order not found" });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "VNPay IPN: Invalid signature or operation. Query: {Query}", Request.QueryString.Value);
                return Ok(new { RspCode = "97", Message = "Invalid signature" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "VNPay IPN processing failed. Query: {Query}", Request.QueryString.Value);
                
                // Tell VNPay to retry
                return Ok(new { RspCode = "99", Message = $"System error" });
            }
        }

        /// <summary>
        /// VNPay Return URL - Browser redirect after payment (Customer-facing)
        /// 
        /// IMPORTANT: This is SECONDARY callback for user experience
        /// - Customer browser is redirected here by VNPay after payment
        /// - Triggered by vnp_ReturnUrl parameter sent in payment request
        /// - Not reliable (customer might close browser)
        /// - Should verify signature but get status from database (IPN already updated it)
        /// - Redirects customer to frontend with payment result
        /// 
        /// Flow:
        /// 1. Customer completes payment at VNPay
        /// 2. VNPay redirects browser to this endpoint with payment params
        /// 3. This endpoint verifies (optional) and gets status from DB
        /// 4. Redirects customer to frontend URL with result
        /// 
        /// vs IPN: ReturnUrl is browser redirect for UX, IPN is reliable server callback
        /// 
        /// Query Parameters:
        /// - frontendUrl: Custom parameter to redirect customer after processing
        /// - vnp_*: VNPay payment result parameters (same as IPN)
        /// </summary>
        [HttpGet("/api/v1/payments/vnpay-return")]
        [AllowAnonymous]
        public async Task<IActionResult> VNPayReturn([FromQuery] string? frontendUrl)
        {
            try
            {
                _logger.LogInformation("VNPay Return received: {QueryString}", Request.QueryString.Value);

                // Read query string parameters into dictionary
                var queryDict = Request.Query
                    .Where(q => q.Key != "frontendUrl") // Exclude our custom param
                    .ToDictionary(k => k.Key, v => v.Value.ToString());

                // Extract transaction id
                string transactionId = queryDict.TryGetValue("vnp_TxnRef", out var txn) ? txn : string.Empty;
                string responseCode = queryDict.TryGetValue("vnp_ResponseCode", out var code) ? code : "";

                if (string.IsNullOrEmpty(transactionId))
                {
                    _logger.LogWarning("VNPay Return called without transaction id");
                    
                    // Redirect to frontend error page
                    var errorUrl = !string.IsNullOrEmpty(frontendUrl) 
                        ? $"{frontendUrl}?status=error&message=Missing+transaction+id"
                        : "/payment/error?message=Missing+transaction+id";
                    
                    return Redirect(errorUrl);
                }

                // Note: IPN should have already processed this payment, but we verify again for safety
                // The service handles idempotency - if already processed, it returns existing status
                PaymentStatusDto result;
                try
                {
                    var callback = new PaymentCallbackDto
                    {
                        TransactionId = transactionId,
                        CallbackData = queryDict
                    };

                    result = await _paymentService.HandlePaymentCallbackAsync(callback);
                    
                    _logger.LogInformation("VNPay Return processed for {TransactionId}. Status: {Status}, ResponseCode: {Code}", 
                        transactionId, result.Status, responseCode);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "VNPay Return: Callback processing failed, fetching status from DB");
                    
                    // If callback fails (e.g., signature mismatch on tampered URL), get from DB
                    var status = await _paymentService.GetPaymentStatusAsync(transactionId);
                    if (status == null)
                    {
                        throw new NotFoundException("Payment", transactionId);
                    }
                    result = status;
                }

                // Redirect to frontend with result
                var redirectUrl = !string.IsNullOrEmpty(frontendUrl) 
                    ? BuildFrontendRedirectUrl(frontendUrl, result, responseCode)
                    : $"/payment/result?transactionId={Uri.EscapeDataString(transactionId)}&status={result.Status}&responseCode={responseCode}";

                _logger.LogInformation("Redirecting to frontend: {Url}", redirectUrl);
                return Redirect(redirectUrl);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "VNPay Return processing failed. Query: {Query}", Request.QueryString.Value);
                
                // Redirect to frontend error page
                var errorUrl = !string.IsNullOrEmpty(frontendUrl) 
                    ? $"{frontendUrl}?status=error&message={Uri.EscapeDataString(ex.Message)}"
                    : $"/payment/error?message={Uri.EscapeDataString(ex.Message)}";
                
                return Redirect(errorUrl);
            }
        }

        /// <summary>
        /// Payment callback/webhook handler
        /// </summary>
        /// <remarks>
        /// This endpoint is called by the payment gateway after payment completion.
        /// It verifies the payment and updates order status.
        /// 
        /// Can be called by:
        /// 1. Payment gateway webhook (server-to-server)
        /// 2. Return redirect from payment page (customer returns)
        /// 
        /// Example:
        ///     POST /api/payment/callback
        ///     {
        ///       "transactionId": "MOCK-abc123",
        ///       "status": "success",
        ///       "signature": "xyz789..."
        ///     }
        /// </remarks>
        [HttpPost("callback")]
        [AllowAnonymous] // Webhook needs to be accessible
        public async Task<IActionResult> PaymentCallback([FromBody] PaymentCallbackDto callback)
        {
            try
            {
                var result = await _paymentService.HandlePaymentCallbackAsync(callback);

                return Ok(ApiResponse<PaymentStatusDto>.Ok(result, "Payment callback processed"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Payment callback processing failed for transaction {TransactionId}",
                    callback.TransactionId);
                return BadRequest(ApiResponse<string>.Fail($"Payment callback failed: {ex.Message}"));
            }
        }

        /// <summary>
        /// Get payment status by transaction ID
        /// </summary>
        /// <remarks>
        /// Use this to check payment status after customer returns from payment gateway.
        /// 
        /// Example:
        ///     GET /api/payment/status/MOCK-abc123
        /// </remarks>
        [HttpGet("status/{transactionId}")]
        [Authorize]
        public async Task<IActionResult> GetPaymentStatus(string transactionId)
        {
            try
            {
                var result = await _paymentService.GetPaymentStatusAsync(transactionId);

                if (result == null)
                {
                    return NotFound(ApiResponse<string>.Fail("Payment not found"));
                }

                return Ok(ApiResponse<PaymentStatusDto>.Ok(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get payment status for {TransactionId}", transactionId);
                return StatusCode(500, ApiResponse<string>.Fail($"Failed to get payment status: {ex.Message}"));
            }
        }

        /// <summary>
        /// Get payment details with full order information (products, variants)
        /// </summary>
        /// <remarks>
        /// Returns payment with complete order details including:
        /// - All order items
        /// - Product information (name, thumbnail, brand, category)
        /// - Variant details (SKU, attributes like Color, RAM, Storage)
        /// 
        /// Example response structure:
        /// {
        ///   "transactionId": "ORD-20241220-ABC123",
        ///   "status": "Completed",
        ///   "amount": 67980000,
        ///   "order": {
        ///     "orderNumber": "ORD-20241220-ABC123",
        ///     "items": [
        ///       {
        ///         "quantity": 2,
        ///         "price": 33990000,
        ///         "product": {
        ///           "name": "iPhone 15 Pro Max",
        ///           "thumbnailUrl": "...",
        ///           "brandName": "Apple",
        ///           "categoryName": "Smartphones"
        ///         },
        ///         "variant": {
        ///           "sku": "IP15PM-BL-256",
        ///           "attributes": [
        ///             { "name": "Color", "value": "Black" },
        ///             { "name": "Storage", "value": "256GB" }
        ///           ]
        ///         }
        ///       }
        ///     ]
        ///   }
        /// }
        /// 
        /// Use this for:
        /// - Payment confirmation page
        /// - Order history details
        /// - Receipt display
        /// 
        /// Example:
        ///     GET /api/payment/details/MOCK-abc123
        /// </remarks>
        [HttpGet("details/{transactionId}")]
        [Authorize]
        public async Task<IActionResult> GetPaymentDetails(string transactionId)
        {
            try
            {
                var result = await _paymentService.GetPaymentStatusWithDetailsAsync(transactionId);

                if (result == null)
                {
                    return NotFound(ApiResponse<string>.Fail("Payment not found"));
                }

                return Ok(ApiResponse<PaymentStatusDto>.Ok(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get payment details for {TransactionId}", transactionId);
                return StatusCode(500, ApiResponse<string>.Fail($"Failed to get payment details: {ex.Message}"));
            }
        }

        /// <summary>
        /// Get list of available payment gateways
        /// </summary>
        /// <remarks>
        /// Returns list of payment gateways with their availability status.
        /// Only gateways marked as "available: true" are configured and can be used.
        /// 
        /// Example response:
        ///     {
        ///       "success": true,
        ///       "data": [
        ///         { "id": 0, "name": "Mock", "description": "Test gateway", "available": true },
        ///         { "id": 3, "name": "VNPay", "description": "Vietnam payment", "available": true }
        ///       ]
        ///     }
        /// </remarks>
        [HttpGet("gateways")]
        [AllowAnonymous]
        public IActionResult GetAvailableGateways([FromServices] PaymentGatewayFactory gatewayFactory)
        {
            var allGateways = new[]
            {
                new { id = 0, name = "Mock", description = "Test payment gateway" },
                new { id = 1, name = "Stripe", description = "Credit/Debit card payments" },
                new { id = 2, name = "PayPal", description = "PayPal payments" },
                new { id = 3, name = "VNPay", description = "Vietnam payment gateway" },
                new { id = 4, name = "MoMo", description = "MoMo e-wallet" },
                new { id = 5, name = "ZaloPay", description = "ZaloPay e-wallet" }
            };

            var availableGatewayIds = gatewayFactory.GetAvailableGateways().Select(g => (int)g).ToHashSet();

            var result = allGateways.Select(g => new
            {
                g.id,
                g.name,
                g.description,
                available = availableGatewayIds.Contains(g.id)
            });

            return Ok(ApiResponse<object>.Ok(result));
        }

        #region Helper Methods

        /// <summary>
        /// Build frontend redirect URL with payment result
        /// </summary>
        private string BuildFrontendRedirectUrl(string baseUrl, PaymentStatusDto result, string responseCode)
        {
            var separator = baseUrl.Contains("?") ? "&" : "?";
            
            return $"{baseUrl}{separator}" +
                   $"transactionId={Uri.EscapeDataString(result.TransactionId)}&" +
                   $"orderId={result.OrderId}&" +
                   $"orderNumber={Uri.EscapeDataString(result.OrderNumber)}&" +
                   $"status={result.Status}&" +
                   $"statusName={Uri.EscapeDataString(result.StatusName ?? "")}&" +
                   $"amount={result.Amount}&" +
                   $"currency={result.Currency}&" +
                   $"responseCode={responseCode}&" +
                   $"gateway={result.Gateway}";
        }

        #endregion
    }
}
