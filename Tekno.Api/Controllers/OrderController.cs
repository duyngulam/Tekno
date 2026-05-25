using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Tekno.Api.Commons.Responses;
using Tekno.Application.Common.Paging;
using Tekno.Application.Order.DTOs;
using Tekno.Application.Order.Services;
using Tekno.Domain.Order;

namespace Tekno.Api.Controllers
{
    /// <summary>
    /// Order history and tracking endpoints
    /// </summary>
    [ApiController]
    [Route("api/orders")]
    [ValidationFilter]
    public class OrderController : ControllerBase
    {
        private readonly OrderService _orderService;
        private readonly ILogger<OrderController> _logger;

        public OrderController(OrderService orderService, ILogger<OrderController> _logger)
        {
            _orderService = orderService;
            this._logger = _logger;
        }

        /// <summary>
        /// Create pending order from cart (Step 1 of two-step checkout)
        /// </summary>
        /// <remarks>
        /// Creates an order with Pending status. Cart items remain in cart.
        /// Shipping address and coupon will be provided in Step 2 (payment).
        /// 
        /// **Two-Step Checkout Flow:**
        /// 1. POST /api/orders/create - Creates pending order (this endpoint)
        /// 2. POST /api/payment/process-order - Add shipping, coupon & process payment
        /// 3. Payment callback - Marks order as Processing/Cancelled, clears/restores cart
        /// 
        /// **If payment fails or times out:**
        /// - Order status ? Cancelled
        /// - Cart items ? Restored automatically
        /// - User can retry payment or modify cart
        /// 
        /// Example request (full cart):
        /// 
        ///     POST /api/orders/create
        ///     {
        ///       "note": "Please deliver after 5pm"
        ///     }
        /// 
        /// Example request (partial cart):
        /// 
        ///     POST /api/orders/create
        ///     {
        ///       "note": "Selected items only",
        ///       "selectedItems": [
        ///         { "variantId": 11, "quantity": 2 }
        ///       ]
        ///     }
        /// 
        /// For full cart checkout, omit selectedItems or pass empty array.
        /// </remarks>
        [HttpPost("create")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<CreateOrderResponseDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<string>), 500)]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequestDto request)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var result = await _orderService.CreateOrderFromCartAsync(userId, request);

                return Ok(ApiResponse<CreateOrderResponseDto>.Ok(result, 
                    "Order created successfully. Proceed to payment with the orderId."));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create order for user");
                return StatusCode(500, ApiResponse<string>.Fail($"Failed to create order: {ex.Message}"));
            }
        }

        /// <summary>
        /// Get current user's order history
        /// </summary>
        /// <remarks>
        /// Returns all orders placed by the authenticated user with full product details.
        /// 
        /// **Purpose:** Let users see what they bought, track delivery, and contact support
        /// 
        /// **Includes:**
        /// - Order status (Processing, Shipping, Delivered, etc.)
        /// - Product details with images
        /// - Variant attributes (Color, Size, etc.)
        /// - Delivery tracking information
        /// - Payment information
        /// - Order number for support
        /// 
        /// **Order Status Flow:**
        /// 1. Pending (1) - Order created, awaiting payment
        /// 2. Processing (2) - Payment received, preparing order
        /// 3. Shipping (4) - Order shipped, on the way
        /// 4. Delivered (5) - Order delivered to customer
        /// 5. Cancelled (6) - Order cancelled (payment failed/timeout)
        /// 
        /// **Filter by status:**
        /// - null: All orders
        /// - 1: Pending (awaiting payment)
        /// - 2: Processing (payment received, preparing order)
        /// - 3: Completed (legacy status - use Shipping/Delivered instead)
        /// - 4: Shipping (in transit)
        /// - 5: Delivered (completed delivery)
        /// - 6: Cancelled (order cancelled)
        /// - 7: Refund Requested (customer requested refund)
        /// - 8: Refunded (refund completed)
        /// 
        /// Examples:
        ///     GET /api/orders/history                    // All orders
        ///     GET /api/orders/history?status=2           // Only processing orders
        ///     GET /api/orders/history?status=4           // Only shipping orders
        ///     GET /api/orders/history?page=2&pageSize=10 // Pagination
        /// 
        /// Response includes order ID for customer support inquiries.
        /// 
        /// **Status Names:**
        /// - All status names are in English (e.g., "Processing", "Shipping", "Delivered")
        /// - Payment status also in English (e.g., "Completed", "Failed", "Pending")
        /// </remarks>
        [HttpGet("history")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<OrderHistoryDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<string>), 500)]
        public async Task<IActionResult> GetOrderHistory(
            [FromQuery] OrderStatus? status = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var orders = await _orderService.GetUserOrderHistoryAsync(userId, status, page, pageSize);

                return Ok(ApiResponse<PagedResult<OrderHistoryDto>>.Ok(
                    orders, 
                    "Order history retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get order history for user");
                return StatusCode(500, ApiResponse<string>.Fail($"Failed to get order history: {ex.Message}"));
            }
        }

        /// <summary>
        /// Get order details by order number
        /// </summary>
        /// <remarks>
        /// Returns complete order information including products, delivery status, and tracking.
        /// 
        /// **Use for:**
        /// - Order details page
        /// - Delivery tracking
        /// - Customer support (provide order number to support team)
        /// - Reorder functionality
        /// 
        /// Example:
        ///     GET /api/orders/ORD-20241220-ABC123
        /// </remarks>
        [HttpGet("{orderNumber}")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<OrderHistoryDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<string>), 404)]
        [ProducesResponseType(typeof(ApiResponse<string>), 500)]
        public async Task<IActionResult> GetOrderDetails(string orderNumber)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var order = await _orderService.GetOrderByNumberAsync(userId, orderNumber);

                if (order == null)
                {
                    return NotFound(ApiResponse<string>.Fail("Order not found"));
                }

                return Ok(ApiResponse<OrderHistoryDto>.Ok(order));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get order details for {OrderNumber}", orderNumber);
                return StatusCode(500, ApiResponse<string>.Fail($"Failed to get order details: {ex.Message}"));
            }
        }

        /// <summary>
        /// Get order details by order ID
        /// </summary>
        /// <remarks>
        /// Alternative endpoint to get order by numeric ID instead of order number.
        /// 
        /// Example:
        ///     GET /api/orders/by-id/123
        /// </remarks>
        [HttpGet("by-id/{orderId:int}")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<OrderHistoryDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<string>), 404)]
        [ProducesResponseType(typeof(ApiResponse<string>), 500)]
        public async Task<IActionResult> GetOrderById(int orderId)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var order = await _orderService.GetOrderDetailsByIdAsync(userId, orderId);

                if (order == null)
                {
                    return NotFound(ApiResponse<string>.Fail("Order not found"));
                }

                return Ok(ApiResponse<OrderHistoryDto>.Ok(order));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get order details for ID {OrderId}", orderId);
                return StatusCode(500, ApiResponse<string>.Fail($"Failed to get order details: {ex.Message}"));
            }
        }
    }
}
