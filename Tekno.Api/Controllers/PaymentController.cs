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
        /// **DEPRECATED - Use Order History Instead**
        /// 
        /// **Recommended endpoints for users:**
        /// - GET /api/orders/history - Complete order history with products, delivery, and payment info
        /// - GET /api/orders/{orderNumber} - Single order details
        /// 
        /// This endpoint returns lightweight payment records without order details.
        /// Only use for:
        /// - Payment verification/debugging
        /// - Financial records
        /// - Support inquiries about payment transactions
        /// 
        /// Example:
        ///     GET /api/payment/my-payments?page=1&amp;pageSize=20
        /// 
        /// Returns only transaction info. For order details, use /api/orders/history
        /// </remarks>
        //[HttpGet("my-payments")]
        //[Authorize]
        //public async Task<IActionResult> GetMyPayments(
        //    [FromQuery] int page = 1,
        //    [FromQuery] int pageSize = 20)
        //{
        //    try
        //    {
        //        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        //        
        //        // Only show completed payments (for payment verification/support)
        //        var payments = await _paymentService.GetUserCompletedPaymentsAsync(userId, page, pageSize);
        //
        //        return Ok(ApiResponse<PagedResult<PaymentStatusDto>>.Ok(
        //            payments, 
        //            "Payment history retrieved. For order details, use /api/orders/history"));
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Failed to get payment history for user");
        //        return StatusCode(500, ApiResponse<string>.Fail($"Failed to get payment history: {ex.Message}"));
        //    }
        //}

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
        
        //[HttpPost("process")]
        //[Authorize]
        //[Obsolete("Use two-step checkout: POST /api/orders/create then POST /api/payment/process-order")]
        //public async Task<IActionResult> ProcessPayment([FromBody] PaymentRequestDto request)
        //{
        //    try
        //    {
        //        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        //        var result = await _paymentService.ProcessPaymentAsync(userId, request);

        //        return Ok(ApiResponse<PaymentResponseDto>.Ok(result, "Payment initiated successfully. Redirect to payment URL."));
        //    }
        //    catch (NotSupportedException ex)
        //    {
        //        // Gateway not available - return user-friendly error
        //        _logger.LogWarning(ex, "Payment gateway not available: {Gateway}", request.Gateway);
        //        return BadRequest(ApiResponse<string>.Fail(ex.Message));
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Payment processing failed");
        //        return StatusCode(500, ApiResponse<string>.Fail($"Payment processing failed: {ex.Message}"));
        //    }
        //}

        /// <summary>
        /// Process payment for existing order - Two-step checkout (Step 2)
        /// </summary>
        /// <remarks>
        /// Applies shipping address and optional coupon, then initiates payment for a Pending order.
        /// **Supports payment retry** - customers can retry if they closed the payment window or experienced failures.
        /// 
        /// **Two-Step Checkout Flow:**
        /// 1. POST /api/orders/create - Creates pending order with cart items
        /// 2. POST /api/payment/process - Add shipping, coupon &amp; initiate payment (this endpoint)
        /// 3. Customer redirected to payment gateway
        /// 4. Payment callback - Updates order and cart based on result
        /// 
        /// **Payment Retry Scenarios:**
        /// 
        /// **Scenario A - Active Payment (Within Timeout):**
        /// - Customer closes VNPay window before completing payment
        /// - Payment still in Processing status (timeout not reached)
        /// - Customer calls this endpoint again with same orderId
        /// - Returns existing payment URL (continues same payment session)
        /// - Response includes `isRetry: true`
        /// 
        /// **Scenario B - After Failed Payment:**
        /// - Payment fails or times out (Status: Failed, Order: Cancelled)
        /// - Cart items automatically restored
        /// - Customer must create new order from cart
        /// - Cannot retry payment for cancelled orders
        /// 
        /// **On Payment Success:**
        /// - Order status → Processing
        /// - Cart items → Cleared
        /// - Stock → Reduced
        /// - Sold count → Incremented
        /// 
        /// **On Payment Failure/Timeout:**
        /// - Order status → Cancelled
        /// - Cart items → Restored
        /// - User can create new order and retry
        /// 
        /// Example request (initial payment):
        /// 
        ///     POST /api/payment/process
        ///     {
        ///       "orderId": 123,
        ///       "shippingAddressId": 1,
        ///       "couponCode": "SUMMER2024",
        ///       "gateway": 3,
        ///       "method": 3,
        ///       "returnUrl": "http://localhost:3000/payment/result"
        ///     }
        /// 
        /// Response (initial):
        ///     {
        ///       "paymentUrl": "https://sandbox.vnpayment.vn/...",
        ///       "isRetry": false
        ///     }
        /// 
        /// Example request (retry - same orderId):
        /// 
        ///     POST /api/payment/process
        ///     {
        ///       "orderId": 123,
        ///       "shippingAddressId": 1,
        ///       "gateway": 3,
        ///       "method": 3,
        ///       "returnUrl": "http://localhost:3000/payment/result"
        ///     }
        /// 
        /// Response (retry):
        ///     {
        ///       "paymentUrl": "https://sandbox.vnpayment.vn/...",
        ///       "isRetry": true
        ///     }
        /// 
        /// Payment Gateways:
        /// - 0 = Mock (for testing) ✓ Available
        /// - 3 = VNPay (Vietnam) ✓ Available
        /// 
        /// Payment Methods:
        /// - 1 = CreditCard
        /// - 2 = DebitCard
        /// - 3 = BankTransfer
        /// - 4 = EWallet
        /// - 5 = Cash (COD)
        /// 
        /// Error Responses:
        /// - 400: Order not in Pending status (cancelled/completed)
        /// - 400: Order already has completed payment
        /// - 404: Order not found
        /// - 401: User not authenticated or order doesn't belong to user
        /// 
        /// Response includes paymentUrl - redirect customer to complete payment.
        /// isRetry flag indicates whether this is a retry attempt (true) or new payment (false).
        /// </remarks>
        /// <param name="request">Payment processing request with order ID, shipping address, coupon, gateway, and method</param>
        /// <returns>Payment response with payment URL to redirect customer</returns>
        /// <response code="200">Payment initiated successfully</response>
        /// <response code="400">Invalid request or gateway not available</response>
        /// <response code="401">User not authenticated</response>
        /// <response code="404">Order not found</response>
        /// <response code="500">Internal server error</response>
        [HttpPost("process")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<PaymentResponseDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<string>), 400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(typeof(ApiResponse<string>), 404)]
        [ProducesResponseType(typeof(ApiResponse<string>), 500)]
        public async Task<IActionResult> ProcessOrderPayment([FromBody] ProcessOrderPaymentRequestDto request)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var result = await _paymentService.ProcessOrderPaymentAsync(userId, request);

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
        [HttpGet("vnpay/ipn")]
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

                // Process the callback
                try
                {
                    var callback = new PaymentCallbackDto
                    {
                        TransactionId = transactionId,
                        CallbackData = queryDict
                    };

                    var result = await _paymentService.HandlePaymentCallbackAsync(callback);

                    _logger.LogInformation(
                        "VNPay IPN processed successfully for {TransactionId}. Status: {Status}",
                        transactionId, result.Status);

                    // Respond to VNPay according to their specification
                    return Ok(new { RspCode = "00", Message = "Confirm Success" });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "VNPay IPN callback processing failed for {TransactionId}", transactionId);
                    
                    // Tell VNPay to retry (RspCode 99 = Unknown error, VNPay will retry)
                    return Ok(new { RspCode = "99", Message = "System error - please retry" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "VNPay IPN processing failed. Query: {Query}", Request.QueryString.Value);
                
                // Tell VNPay to retry
                return Ok(new { RspCode = "99", Message = "System error" });
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
        [HttpGet("status/{transactionId}")]
        public async Task<IActionResult> GetPaymentStatus(string transactionId)
        {
            var status = await _paymentService.GetPaymentStatusAsync(transactionId);
            
            if (status == null)
            {
                return NotFound(ApiResponse<PaymentStatusDto>.Fail($"Payment not found: {transactionId}"));
            }

            return Ok(ApiResponse<PaymentStatusDto>.Ok(status));
        }

        /// <summary>
        /// Get order payment status with retry information
        /// Shows payment history, retry capability, and sync status between order and payment
        /// </summary>
        /// <remarks>
        /// Returns comprehensive information about order payment status:
        /// - Current order status (Pending, Cancelled, Processing, etc.)
        /// - All payment attempts for this order
        /// - Whether order can be retried (CanRetryPayment)
        /// - Why retry is/isn't available (RetryReason)
        /// - Order age and timeout information
        /// 
        /// **Use Cases:**
        /// 
        /// 1. **Check if order can be retried after timeout:**
        ///    - Order in Cancelled status (payment timed out)
        ///    - CanRetryPayment = true if within 24 hours
        ///    - Frontend shows "Retry Payment" button
        /// 
        /// 2. **Check payment progress:**
        ///    - HasActivePayment = true: payment still processing
        ///    - HasActivePayment = false: payment timed out or failed
        /// 
        /// 3. **Debug payment issues:**
        ///    - View full payment history
        ///    - See error messages from gateway
        ///    - Check timestamps and timeout status
        /// 
        /// Example response (order can be retried):
        /// 
        ///     {
        ///       "orderId": 123,
        ///       "orderNumber": "ORD-20241220-ABC123",
        ///       "orderStatus": "Cancelled",
        ///       "orderCreatedAt": "2024-12-20T10:00:00Z",
        ///       "orderAgeHours": 2.5,
        ///       "totalAmount": 1500000,
        ///       "canRetryPayment": true,
        ///       "retryReason": "You can retry payment for this order.",
        ///       "paymentAttempts": 2,
        ///       "latestPaymentStatus": "Failed",
        ///       "latestPaymentError": "Payment timed out after 15 minutes",
        ///       "hasActivePayment": false,
        ///       "paymentHistory": [
        ///         {
        ///           "transactionId": "VNPAY-20241220-001",
        ///           "status": "Failed",
        ///           "gateway": "VNPay",
        ///           "amount": 1500000,
        ///           "createdAt": "2024-12-20T10:00:00Z",
        ///           "failedAt": "2024-12-20T10:15:00Z",
        ///           "errorMessage": "Payment timed out after 15 minutes"
        ///         }
        ///       ]
        ///     }
        /// 
        /// Example response (order too old):
        /// 
        ///     {
        ///       "orderId": 456,
        ///       "orderStatus": "Cancelled",
        ///       "orderAgeHours": 30.5,
        ///       "canRetryPayment": false,
        ///       "retryReason": "Order is too old (30.5 hours). Please create a new order.",
        ///       ...
        ///     }
        /// </remarks>
        [HttpGet("order/{orderId}/status")]
        [Authorize]
        public async Task<IActionResult> GetOrderPaymentStatus(int orderId)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var status = await _paymentService.GetOrderPaymentStatusAsync(orderId, userId);
                
                return Ok(ApiResponse<OrderPaymentStatusDto>.Ok(status));
            }
            catch (NotFoundException ex)
            {
                return NotFound(ApiResponse<OrderPaymentStatusDto>.Fail(ex.Message));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid();
            }
        }

        /// <summary>
        /// Get payment details by transaction ID
        /// </summary>
        /// <remarks>
        /// **DEPRECATED - Use Order Details Instead**
        /// 
        /// **Recommended endpoints:**
        /// - GET /api/orders/{orderNumber} - Full order with products, variants, delivery tracking
        /// - GET /api/orders/by-id/{orderId} - Order by ID
        /// 
        /// This endpoint returns basic payment info. You can get the orderNumber from the response
        /// and use it with /api/orders/{orderNumber} for complete order details.
        /// 
        /// Example:
        ///     GET /api/payment/details/MOCK-abc123
        ///     Response: { "orderNumber": "ORD-20241220-ABC123", ... }
        ///     
        ///     Then use:
        ///     GET /api/orders/ORD-20241220-ABC123
        /// </remarks>
        //[HttpGet("details/{transactionId}")]
        //[Authorize]
        //public async Task<IActionResult> GetPaymentDetails(string transactionId)
        //{
        //    try
        //    {
        //        var result = await _paymentService.GetPaymentStatusAsync(transactionId);

        //        if (result == null)
        //        {
        //            return NotFound(ApiResponse<string>.Fail("Payment not found"));
        //        }

        //        // Return lightweight status with orderNumber to use with Order endpoints
        //        return Ok(ApiResponse<PaymentStatusDto>.Ok(result, 
        //            $"For complete order details, use: /api/orders/{result.OrderNumber}"));
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Failed to get payment details for {TransactionId}", transactionId);
        //        return StatusCode(500, ApiResponse<string>.Fail($"Failed to get payment details: {ex.Message}"));
        //    }
        //}

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
            //var separator = baseUrl.Contains("?") ? "&" : "?";

            //return $"{baseUrl}{separator}" +
            //       $"transactionId={Uri.EscapeDataString(result.TransactionId)}&" +
            //       $"orderId={result.OrderId}&" +
            //       $"orderNumber={Uri.EscapeDataString(result.OrderNumber)}&" +
            //       $"status={result.Status}&" +
            //       $"statusName={Uri.EscapeDataString(result.StatusName ?? "")}&" +
            //       $"amount={result.Amount}&" +
            //       $"currency={result.Currency}&" +
            //       $"responseCode={responseCode}&" +
            //       $"gateway={result.Gateway}";
            return baseUrl + $"/?OrderId={result.OrderId}";
            ;
            ;
        }

        #endregion
    }
}
