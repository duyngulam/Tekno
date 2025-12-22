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

namespace Tekno.Api.Controllers.Admin
{
    /// <summary>
    /// Admin order management endpoints
    /// </summary>
    [ApiController]
    [Route("api/admin/orders")]
    [Authorize(Roles = "Admin")]
    [ValidationFilter]
    public class AdminOrderController : ControllerBase
    {
        private readonly AdminOrderService _adminOrderService;
        private readonly ILogger<AdminOrderController> _logger;

        public AdminOrderController(
            AdminOrderService adminOrderService,
            ILogger<AdminOrderController> logger)
        {
            _adminOrderService = adminOrderService;
            _logger = logger;
        }

        /// <summary>
        /// Get all orders (admin view)
        /// </summary>
        /// <remarks>
        /// Returns paginated list of all orders with customer information.
        /// 
        /// **Features:**
        /// - Filter by order status
        /// - Search by order number or user ID
        /// - Filter by date range
        /// - Pagination support
        /// 
        /// **Filter by status:**
        /// - null: All orders
        /// - 1: Pending (awaiting payment)
        /// - 2: Processing (payment received, preparing order)
        /// - 4: Shipping (in transit)
        /// - 5: Delivered (completed)
        /// - 6: Cancelled
        /// - 7: Refund Requested
        /// - 8: Refunded
        /// 
        /// **Search:**
        /// - Order number (e.g., "ORD-20241222")
        /// - User ID (e.g., "123")
        /// 
        /// Examples:
        ///     GET /api/admin/orders                                    // All orders
        ///     GET /api/admin/orders?status=2                           // Processing orders
        ///     GET /api/admin/orders?search=ORD-20241222                // Search by order number
        ///     GET /api/admin/orders?startDate=2024-01-01               // Orders from date
        ///     GET /api/admin/orders?status=4&amp;page=2&amp;pageSize=50  // Shipping orders, page 2
        /// </remarks>
        [HttpGet]
        public async Task<IActionResult> GetAllOrders(
            [FromQuery] OrderStatus? status = null,
            [FromQuery] string? search = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                var orders = await _adminOrderService.GetAllOrdersAsync(
                    status, search, startDate, endDate, page, pageSize);

                return Ok(ApiResponse<PagedResult<AdminOrderListDto>>.Ok(
                    orders,
                    "Orders retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get orders for admin");
                return StatusCode(500, ApiResponse<string>.Fail(
                    $"Failed to get orders: {ex.Message}"));
            }
        }

        /// <summary>
        /// Get order details by ID (admin view)
        /// </summary>
        /// <remarks>
        /// Returns complete order information including:
        /// - Customer details
        /// - Order items with product info
        /// - Payment details
        /// - Delivery tracking
        /// - Order history
        /// 
        /// Example:
        ///     GET /api/admin/orders/123
        /// </remarks>
        [HttpGet("{orderId:int}")]
        public async Task<IActionResult> GetOrderDetails(int orderId)
        {
            try
            {
                var order = await _adminOrderService.GetOrderDetailsByIdAsync(orderId);

                if (order == null)
                {
                    return NotFound(ApiResponse<string>.Fail("Order not found"));
                }

                return Ok(ApiResponse<AdminOrderDetailDto>.Ok(order));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get order details for order {OrderId}", orderId);
                return StatusCode(500, ApiResponse<string>.Fail(
                    $"Failed to get order details: {ex.Message}"));
            }
        }

        /// <summary>
        /// Update order status
        /// </summary>
        /// <remarks>
        /// Update order to a specific status.
        /// 
        /// **Valid status transitions:**
        /// - Pending ? Processing (after payment)
        /// - Processing ? Cancelled (cancel before shipping)
        /// - Any ? Refund Requested ? Refunded
        /// 
        /// **Note:** For shipping and delivery, use dedicated endpoints
        /// 
        /// Example request:
        ///     PUT /api/admin/orders/123/status
        ///     {
        ///       "status": 2,
        ///       "note": "Payment verified manually"
        ///     }
        /// </remarks>
        [HttpPut("{orderId:int}/status")]
        public async Task<IActionResult> UpdateOrderStatus(
            int orderId,
            [FromBody] UpdateOrderStatusDto dto)
        {
            try
            {
                var adminId = GetCurrentUserId();
                var success = await _adminOrderService.UpdateOrderStatusAsync(orderId, dto, adminId);

                if (!success)
                {
                    return BadRequest(ApiResponse<string>.Fail(
                        "Failed to update order status. Order not found or invalid status transition."));
                }

                return Ok(ApiResponse<bool>.Ok(true, "Order status updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update status for order {OrderId}", orderId);
                return StatusCode(500, ApiResponse<string>.Fail(
                    $"Failed to update order status: {ex.Message}"));
            }
        }

        /// <summary>
        /// Ship an order
        /// </summary>
        /// <remarks>
        /// Mark order as shipped and add tracking information.
        /// 
        /// **Requirements:**
        /// - Order must be in Processing status
        /// - Tracking number is required
        /// - Carrier name is optional
        /// 
        /// **After shipping:**
        /// - Order status ? Shipping
        /// - Customer can track delivery
        /// 
        /// Example request:
        ///     POST /api/admin/orders/123/ship
        ///     {
        ///       "trackingNumber": "VNP123456789",
        ///       "carrier": "Viettel Post",
        ///       "note": "Shipped via express delivery"
        ///     }
        /// </remarks>
        [HttpPost("{orderId:int}/ship")]
        public async Task<IActionResult> ShipOrder(
            int orderId,
            [FromBody] ShipOrderDto dto)
        {
            try
            {
                var adminId = GetCurrentUserId();
                var success = await _adminOrderService.ShipOrderAsync(orderId, dto, adminId);

                if (!success)
                {
                    return BadRequest(ApiResponse<string>.Fail(
                        "Failed to ship order. Order not found or not in Processing status."));
                }

                return Ok(ApiResponse<bool>.Ok(true, "Order marked as shipped successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to ship order {OrderId}", orderId);
                return StatusCode(500, ApiResponse<string>.Fail(
                    $"Failed to ship order: {ex.Message}"));
            }
        }

        /// <summary>
        /// Mark order as delivered
        /// </summary>
        /// <remarks>
        /// Mark order as delivered to customer.
        /// 
        /// **Requirements:**
        /// - Order must be in Shipping status
        /// 
        /// **After delivery:**
        /// - Order status ? Delivered
        /// - Product sold counts updated
        /// - Customer can review products
        /// 
        /// Example:
        ///     POST /api/admin/orders/123/deliver
        /// </remarks>
        [HttpPost("{orderId:int}/deliver")]
        public async Task<IActionResult> DeliverOrder(int orderId)
        {
            try
            {
                var adminId = GetCurrentUserId();
                var success = await _adminOrderService.DeliverOrderAsync(orderId, adminId);

                if (!success)
                {
                    return BadRequest(ApiResponse<string>.Fail(
                        "Failed to deliver order. Order not found or not in Shipping status."));
                }

                return Ok(ApiResponse<bool>.Ok(true, "Order marked as delivered successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to deliver order {OrderId}", orderId);
                return StatusCode(500, ApiResponse<string>.Fail(
                    $"Failed to deliver order: {ex.Message}"));
            }
        }

        /// <summary>
        /// Cancel an order
        /// </summary>
        /// <remarks>
        /// Cancel an order before delivery.
        /// 
        /// **Requirements:**
        /// - Order cannot be Delivered or already Cancelled
        /// - Cancellation reason is required
        /// 
        /// **After cancellation:**
        /// - Order status ? Cancelled
        /// - Payment may need to be refunded
        /// 
        /// Example request:
        ///     POST /api/admin/orders/123/cancel
        ///     {
        ///       "reason": "Product out of stock"
        ///     }
        /// </remarks>
        [HttpPost("{orderId:int}/cancel")]
        public async Task<IActionResult> CancelOrder(
            int orderId,
            [FromBody] CancelOrderDto dto)
        {
            try
            {
                var adminId = GetCurrentUserId();
                var success = await _adminOrderService.CancelOrderAsync(orderId, dto, adminId);

                if (!success)
                {
                    return BadRequest(ApiResponse<string>.Fail(
                        "Failed to cancel order. Order not found or cannot be cancelled."));
                }

                return Ok(ApiResponse<bool>.Ok(true, "Order cancelled successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to cancel order {OrderId}", orderId);
                return StatusCode(500, ApiResponse<string>.Fail(
                    $"Failed to cancel order: {ex.Message}"));
            }
        }

        /// <summary>
        /// Get order statistics
        /// </summary>
        /// <remarks>
        /// Returns order statistics for admin dashboard:
        /// - Order counts by status
        /// - Revenue statistics (total, today, this month)
        /// - Order trends (today, this week, this month)
        /// 
        /// **Use for:**
        /// - Dashboard overview
        /// - Business analytics
        /// - Performance monitoring
        /// 
        /// Example:
        ///     GET /api/admin/orders/statistics
        /// </remarks>
        [HttpGet("statistics")]
        public async Task<IActionResult> GetStatistics()
        {
            try
            {
                var statistics = await _adminOrderService.GetOrderStatisticsAsync();

                return Ok(ApiResponse<OrderStatisticsDto>.Ok(
                    statistics,
                    "Order statistics retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get order statistics");
                return StatusCode(500, ApiResponse<string>.Fail(
                    $"Failed to get order statistics: {ex.Message}"));
            }
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                throw new UnauthorizedAccessException("User not authenticated");
            }
            return userId;
        }
    }
}
