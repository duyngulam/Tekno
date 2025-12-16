using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using Tekno.Api.Common.Responses;
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
        /// Get current user's payment history
        /// </summary>
        /// <remarks>
        /// Returns all payments made by the authenticated user, ordered by most recent first.
        /// Supports pagination for better performance with large payment histories.
        /// 
        /// Example:
        ///     GET /api/payment/my-payments?page=1&amp;pageSize=20
        /// 
        /// Returns paginated list of payments with order details, gateway names, and status
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
                var payments = await _paymentService.GetUserPaymentsAsync(userId, page, pageSize);

                return Ok(ApiResponse<PagedResult<PaymentStatusDto>>.Ok(payments, "Payment history retrieved successfully"));
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
        /// - 0 = Mock (for testing)
        /// - 1 = Stripe (credit/debit cards)
        /// - 2 = PayPal
        /// - 3 = VNPay (Vietnam)
        /// - 4 = MoMo (Vietnam e-wallet)
        /// - 5 = ZaloPay (Vietnam e-wallet)
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Payment processing failed");
                return StatusCode(500, ApiResponse<string>.Fail($"Payment processing failed: {ex.Message}"));
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
        /// Get list of available payment gateways
        /// </summary>
        [HttpGet("gateways")]
        [AllowAnonymous]
        public IActionResult GetAvailableGateways()
        {
            var gateways = new[]
            {
                new { id = 0, name = "Mock", description = "Test payment gateway" },
                new { id = 1, name = "Stripe", description = "Credit/Debit card payments" },
                new { id = 2, name = "PayPal", description = "PayPal payments" },
                new { id = 3, name = "VNPay", description = "Vietnam payment gateway" },
                new { id = 4, name = "MoMo", description = "MoMo e-wallet" },
                new { id = 5, name = "ZaloPay", description = "ZaloPay e-wallet" }
            };

            return Ok(ApiResponse<object>.Ok(gateways));
        }
    }
}
