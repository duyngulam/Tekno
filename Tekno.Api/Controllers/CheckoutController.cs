using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using Tekno.Api.Common.Responses;
using Tekno.Application.Payment.DTOs;
using Tekno.Application.Payment.Services;

namespace Tekno.Api.Controllers
{
    /// <summary>
    /// Checkout and payment endpoints
    /// </summary>
    [ApiController]
    [Route("api/checkout")]
    [ValidationFilter]
    public class CheckoutController : ControllerBase
    {
        private readonly CheckoutService _checkoutService;
        private readonly ILogger<CheckoutController> _logger;

        public CheckoutController(CheckoutService checkoutService, ILogger<CheckoutController> logger)
        {
            _checkoutService = checkoutService;
            _logger = logger;
        }

        /// <summary>
        /// Get current user's payment history
        /// </summary>
        /// <remarks>
        /// Returns all payments made by the authenticated user, ordered by most recent first.
        /// 
        /// Example:
        ///     GET /api/checkout/my-payments
        /// 
        /// Returns list of payments with order details and status
        /// </remarks>
        [HttpGet("my-payments")]
        [Authorize]
        public async Task<IActionResult> GetMyPayments()
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var payments = await _checkoutService.GetUserPaymentsAsync(userId);

                return Ok(ApiResponse<List<PaymentStatusDto>>.Ok(payments, "Payment history retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get payment history for user");
                return StatusCode(500, ApiResponse<string>.Fail($"Failed to get payment history: {ex.Message}"));
            }
        }

        /// <summary>
        /// Process checkout - Create order and initiate payment
        /// </summary>
        /// <remarks>
        /// Workflow:
        /// 1. Validates user's cart
        /// 2. Creates order from cart items
        /// 3. Initiates payment with selected gateway
        /// 4. Returns payment URL to redirect customer
        /// 5. Clears cart after successful order creation
        /// 
        /// Example request:
        /// 
        ///     POST /api/checkout
        ///     {
        ///       "shippingAddressId": 1,
        ///       "gateway": 0,  // 0=Mock, 1=Stripe, 2=PayPal, 3=VNPay, 4=MoMo, 5=ZaloPay
        ///       "method": 1,   // 1=CreditCard, 2=DebitCard, 3=BankTransfer, 4=EWallet, 5=Cash
        ///       "couponCode": "SAVE10",
        ///       "note": "Please deliver after 5pm",
        ///       "returnUrl": "https://yoursite.com/payment/result"
        ///     }
        /// 
        /// Response includes paymentUrl - redirect customer there to complete payment
        /// </remarks>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Checkout([FromBody] CheckoutRequestDto request)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var result = await _checkoutService.CheckoutAsync(userId, request);

                return Ok(ApiResponse<CheckoutResponseDto>.Ok(result, "Checkout successful. Redirect to payment URL."));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Checkout failed");
                return StatusCode(500, ApiResponse<string>.Fail($"Checkout failed: {ex.Message}"));
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
        ///     POST /api/checkout/payment-callback
        ///     {
        ///       "transactionId": "MOCK-abc123",
        ///       "status": "success",
        ///       "signature": "xyz789..."
        ///     }
        /// </remarks>
        [HttpPost("payment-callback")]
        [AllowAnonymous] // Webhook needs to be accessible
        public async Task<IActionResult> PaymentCallback([FromBody] PaymentCallbackDto callback)
        {
            try
            {
                var result = await _checkoutService.HandlePaymentCallbackAsync(callback);

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
        ///     GET /api/checkout/payment-status/MOCK-abc123
        /// </remarks>
        [HttpGet("payment-status/{transactionId}")]
        [Authorize]
        public async Task<IActionResult> GetPaymentStatus(string transactionId)
        {
            try
            {
                var result = await _checkoutService.GetPaymentStatusAsync(transactionId);

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
