using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Tekno.Api.Common.Responses;
using Tekno.Application.Common.Paging;
using Tekno.Application.Payment.DTOs;
using Tekno.Application.Payment.Services;
using Tekno.Domain.Payment;

namespace Tekno.Api.Controllers.Admin
{
    /// <summary>
    /// Admin payment management endpoints
    /// </summary>
    [ApiController]
    [Route("api/admin/payments")]
    [Authorize(Roles = "Admin")]
    [ValidationFilter]
    public class AdminPaymentController : ControllerBase
    {
        private readonly AdminPaymentService _adminPaymentService;
        private readonly ILogger<AdminPaymentController> _logger;

        public AdminPaymentController(
            AdminPaymentService adminPaymentService,
            ILogger<AdminPaymentController> logger)
        {
            _adminPaymentService = adminPaymentService;
            _logger = logger;
        }

        /// <summary>
        /// Get paged list of payment transactions with filters
        /// </summary>
        /// <remarks>
        /// Filter and paginate through all payment transactions in the system.
        /// 
        /// Query Parameters:
        /// - userId: Filter by specific user
        /// - status: Filter by payment status (1=Pending, 2=Processing, 3=Completed, 4=Failed, 5=Refunded, 6=Cancelled)
        /// - gateway: Filter by payment gateway (0=Mock, 1=Stripe, 2=PayPal, 3=VNPay, 4=MoMo, 5=ZaloPay)
        /// - search: Search in transaction ID or order number
        /// - page: Page number (default: 1)
        /// - pageSize: Items per page (default: 20)
        /// 
        /// Example:
        ///     GET /api/admin/payments?status=3&amp;gateway=1&amp;page=1&amp;pageSize=20
        ///     
        /// Returns transactions with pagination metadata
        /// </remarks>
        [HttpGet]
        public async Task<IActionResult> GetPagedTransactions(
            [FromQuery] int? userId,
            [FromQuery] PaymentStatus? status,
            [FromQuery] PaymentGateway? gateway,
            [FromQuery] string? search,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                var result = await _adminPaymentService.GetPagedTransactionsAsync(
                    userId, status, gateway, search, page, pageSize);

                return Ok(ApiResponse<PagedResult<PaymentStatusDto>>.Ok(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get paged transactions");
                return StatusCode(500, ApiResponse<string>.Fail($"Failed to get transactions: {ex.Message}"));
            }
        }

        /// <summary>
        /// Get payment transaction by ID
        /// </summary>
        /// <remarks>
        /// Get detailed information about a specific payment transaction.
        /// 
        /// Example:
        ///     GET /api/admin/payments/123
        /// </remarks>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetPaymentById(int id)
        {
            try
            {
                var payment = await _adminPaymentService.GetPaymentByIdAsync(id);

                if (payment == null)
                {
                    return NotFound(ApiResponse<string>.Fail("Payment not found"));
                }

                return Ok(ApiResponse<PaymentStatusDto>.Ok(payment));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get payment {PaymentId}", id);
                return StatusCode(500, ApiResponse<string>.Fail($"Failed to get payment: {ex.Message}"));
            }
        }

        /// <summary>
        /// Get payment transaction by transaction ID
        /// </summary>
        /// <remarks>
        /// Get detailed information using the gateway transaction ID.
        /// 
        /// Example:
        ///     GET /api/admin/payments/transaction/MOCK-abc123
        /// </remarks>
        [HttpGet("transaction/{transactionId}")]
        public async Task<IActionResult> GetPaymentByTransactionId(string transactionId)
        {
            try
            {
                var payment = await _adminPaymentService.GetPaymentByTransactionIdAsync(transactionId);

                if (payment == null)
                {
                    return NotFound(ApiResponse<string>.Fail("Payment not found"));
                }

                return Ok(ApiResponse<PaymentStatusDto>.Ok(payment));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get payment by transaction ID {TransactionId}", transactionId);
                return StatusCode(500, ApiResponse<string>.Fail($"Failed to get payment: {ex.Message}"));
            }
        }

        /// <summary>
        /// Get payment statistics summary
        /// </summary>
        /// <remarks>
        /// Get overall payment statistics including totals by status and gateway.
        /// 
        /// Example:
        ///     GET /api/admin/payments/statistics
        /// </remarks>
        [HttpGet("statistics")]
        public IActionResult GetPaymentStatistics()
        {
            // This can be implemented later with actual statistics calculation
            var stats = new
            {
                message = "Payment statistics endpoint - to be implemented",
                availableEndpoints = new[]
                {
                    "GET /api/admin/payments - Get paged transactions",
                    "GET /api/admin/payments/{id} - Get payment by ID",
                    "GET /api/admin/payments/transaction/{transactionId} - Get by transaction ID"
                }
            };

            return Ok(ApiResponse<object>.Ok(stats));
        }
    }
}
