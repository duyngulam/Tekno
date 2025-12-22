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
