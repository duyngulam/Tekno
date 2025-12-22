using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Tekno.Domain.Order;

namespace Tekno.Application.Order.DTOs
{
    /// <summary>
    /// Admin order list item with customer info
    /// </summary>
    public class AdminOrderListDto
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public int UserId { get; set; }
        public string UserEmail { get; set; } = string.Empty;
        public OrderStatus Status { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public int ItemsCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ShippedAt { get; set; }
        public DateTime? DeliveredAt { get; set; }
        public string? TrackingNumber { get; set; }
        public string? ShippingCarrier { get; set; }
        
        // Payment info
        public string? PaymentGateway { get; set; }
        public string? PaymentStatus { get; set; }
        public string? PaymentMethod { get; set; }
    }

    /// <summary>
    /// Admin order details with full information
    /// </summary>
    public class AdminOrderDetailDto
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public int UserId { get; set; }
        public string UserEmail { get; set; } = string.Empty;
        public string? UserPhone { get; set; }
        
        public OrderStatus Status { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime? ShippedAt { get; set; }
        public DateTime? DeliveredAt { get; set; }
        
        public string? TrackingNumber { get; set; }
        public string? ShippingCarrier { get; set; }
        public string? CustomerNote { get; set; }
        
        // Customer shipping address
        public OrderAddressDto? ShippingAddress { get; set; }
        
        // Order items
        public List<OrderItemDto> Items { get; set; } = new();
        
        // Payment details
        public OrderPaymentDto? Payment { get; set; }
    }

    /// <summary>
    /// Order address for admin view
    /// </summary>
    public class OrderAddressDto
    {
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string AddressLine { get; set; } = string.Empty;
        public string Ward { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string Province { get; set; } = string.Empty;
    }

    /// <summary>
    /// Update order status DTO
    /// </summary>
    public class UpdateOrderStatusDto
    {
        [Required]
        [Range(1, 8, ErrorMessage = "Status must be between 1 and 8")]
        public OrderStatus Status { get; set; }
        
        [StringLength(500, ErrorMessage = "Note cannot exceed 500 characters")]
        public string? Note { get; set; }
    }

    /// <summary>
    /// Ship order DTO
    /// </summary>
    public class ShipOrderDto
    {
        [Required]
        [StringLength(100, ErrorMessage = "Tracking number cannot exceed 100 characters")]
        public string TrackingNumber { get; set; } = string.Empty;
        
        [StringLength(100, ErrorMessage = "Carrier name cannot exceed 100 characters")]
        public string? Carrier { get; set; }
        
        [StringLength(500, ErrorMessage = "Note cannot exceed 500 characters")]
        public string? Note { get; set; }
    }

    /// <summary>
    /// Cancel order DTO
    /// </summary>
    public class CancelOrderDto
    {
        [Required]
        [StringLength(500, MinimumLength = 5, ErrorMessage = "Reason must be between 5 and 500 characters")]
        public string Reason { get; set; } = string.Empty;
    }

    /// <summary>
    /// Order statistics for admin dashboard
    /// </summary>
    public class OrderStatisticsDto
    {
        public int TotalOrders { get; set; }
        public int PendingOrders { get; set; }
        public int ProcessingOrders { get; set; }
        public int ShippingOrders { get; set; }
        public int DeliveredOrders { get; set; }
        public int CancelledOrders { get; set; }
        public int RefundRequestedOrders { get; set; }
        
        public decimal TotalRevenue { get; set; }
        public decimal TodayRevenue { get; set; }
        public decimal ThisMonthRevenue { get; set; }
        
        public int TodayOrders { get; set; }
        public int ThisWeekOrders { get; set; }
        public int ThisMonthOrders { get; set; }
    }
}
