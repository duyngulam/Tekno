using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Tekno.Application.Cart.DTOs;
using Tekno.Domain.Payment;

namespace Tekno.Application.Payment.DTOs
{
    /// <summary>
    /// Payment request - Create order and initiate payment
    /// </summary>
    public class PaymentRequestDto
    {
        [Required(ErrorMessage = "Shipping address is required")]
        public int ShippingAddressId { get; set; }

        [Required(ErrorMessage = "Payment gateway is required")]
        public PaymentGateway Gateway { get; set; }

        [Required(ErrorMessage = "Payment method is required")]
        public PaymentMethod Method { get; set; }

        public string? CouponCode { get; set; }

        public string? Note { get; set; }

        [Required(ErrorMessage = "Return URL is required")]
        [Url(ErrorMessage = "Invalid return URL format")]
        public string ReturnUrl { get; set; } = string.Empty; // Frontend URL to redirect after payment

        /// <summary>
        /// Selected cart items for checkout
        /// If null or empty, checkout entire cart
        /// </summary>
        public List<SelectedCartItemDto>? SelectedItems { get; set; }
    }

    /// <summary>
    /// Payment response with payment URL
    /// </summary>
    public class PaymentResponseDto
    {
        public int OrderId { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public int PaymentId { get; set; }
        public string TransactionId { get; set; } = string.Empty;
        public string PaymentUrl { get; set; } = string.Empty; // Redirect customer here
        public string? PaymentToken { get; set; }
        public string? QrCodeUrl { get; set; }
        public PaymentStatus Status { get; set; }
        public decimal TotalAmount { get; set; }
        public int ItemsCount { get; set; }
    }

    /// <summary>
    /// Payment callback verification
    /// </summary>
    public class PaymentCallbackDto
    {
        [Required]
        public string TransactionId { get; set; } = string.Empty;

        public string? Status { get; set; }
        public string? Signature { get; set; }
        
        // Additional gateway-specific data as Dictionary or dynamic
        public object? CallbackData { get; set; }
    }

    /// <summary>
    /// Payment status check
    /// </summary>
    public class PaymentStatusDto
    {
        public int PaymentId { get; set; }
        public int OrderId { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public string TransactionId { get; set; } = string.Empty;
        public PaymentGateway Gateway { get; set; }
        public string GatewayName { get; set; } = string.Empty;
        public PaymentMethod Method { get; set; }
        public string MethodName { get; set; } = string.Empty;
        public PaymentStatus Status { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string? ErrorMessage { get; set; }
    }

    /// <summary>
    /// Order summary for payment
    /// </summary>
    public class OrderSummaryDto
    {
        public decimal Subtotal { get; set; }
        public decimal Discount { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal Tax { get; set; }
        public decimal Total { get; set; }
        public int TotalItems { get; set; }
    }
}
