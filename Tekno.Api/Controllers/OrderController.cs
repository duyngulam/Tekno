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
        /// - Order number for support
        /// 
        /// **Filter by status:**
        /// - null: All orders
        /// - 2: Processing (?ang x? lý)
        /// - 4: Shipping (?ang giao hàng)
        /// - 5: Delivered (?ã giao hàng)
        /// - 7: Refund Requested (yêu c?u hoàn ti?n)
        /// - 8: Refunded (?ã hoàn ti?n)
        /// 
        /// Examples:
        ///     GET /api/orders/history                    // All orders
        ///     GET /api/orders/history?status=4           // Only shipping orders
        ///     GET /api/orders/history?page=2&pageSize=10 // Pagination
        /// 
        /// Response includes order ID for customer support inquiries.
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
