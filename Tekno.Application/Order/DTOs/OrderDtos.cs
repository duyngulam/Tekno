using System;
using System.Collections.Generic;
using Tekno.Application.Catalog.DTOs.Products;
using Tekno.Domain.Order;

namespace Tekno.Application.Order.DTOs
{
    /// <summary>
    /// Order history - Full order details with nested objects
    /// Used for: Order history, Order details page, Order tracking
    /// </summary>
    public class OrderHistoryDto
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public OrderStatus Status { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }

        // Payment Details (essential info only)
        public OrderPaymentDto? Payment { get; set; }

        // Order Items with Product/Variant details
        public List<OrderItemDto> Items { get; set; } = new();

        // Delivery information
        public OrderDeliveryDto? Delivery { get; set; }
    }

    /// <summary>
    /// Payment details within order (simplified)
    /// </summary>
    public class OrderPaymentDto
    {
        public int PaymentId { get; set; }
        public string TransactionId { get; set; } = string.Empty;
        public string Gateway { get; set; } = string.Empty; // VNPay, Stripe, etc.
        public string Method { get; set; } = string.Empty; // Credit Card, Bank Transfer, etc.
        public string Status { get; set; } = string.Empty; // Completed, Failed, etc.
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "VND";
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string? ErrorMessage { get; set; }
    }

    /// <summary>
    /// Order item with nested Product and Variant (reusing catalog DTOs)
    /// </summary>
    public class OrderItemDto
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal TotalPrice { get; set; }

        // Reuse existing Product DTO
        public ProductSummaryDto Product { get; set; } = new();

        // Reuse existing Variant DTO
        public ProductVariantDto Variant { get; set; } = new();
    }

    /// <summary>
    /// Delivery/Shipping information
    /// </summary>
    public class OrderDeliveryDto
    {
        public string Status { get; set; } = string.Empty; // Pending, Shipped, Delivered
        public string? TrackingNumber { get; set; }
        public string? Carrier { get; set; }
        public DateTime? ShippedAt { get; set; }
        public DateTime? DeliveredAt { get; set; }
        public DateTime? EstimatedDeliveryDate { get; set; }
        
        // Shipping address
        public OrderShippingAddressDto? ShippingAddress { get; set; }
    }

    /// <summary>
    /// Shipping address in order (Vietnamese location format)
    /// </summary>
    public class OrderShippingAddressDto
    {
        public string RecipientName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string AddressLine { get; set; } = string.Empty;
        public int ProvinceCode { get; set; }
        public string ProvinceName { get; set; } = string.Empty;
        public int DistrictCode { get; set; }
        public string DistrictName { get; set; } = string.Empty;
        public int WardCode { get; set; }
        public string WardName { get; set; } = string.Empty;
    }

    /// <summary>
    /// Simplified order list item (for listing pages)
    /// </summary>
    public class OrderListItemDto
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public OrderStatus Status { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public int ItemsCount { get; set; }
        public DateTime CreatedAt { get; set; }
        
        // Payment summary
        public string PaymentGateway { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = string.Empty;
        
        // First item thumbnail for preview
        public string? FirstItemThumbnail { get; set; }
    }
}
