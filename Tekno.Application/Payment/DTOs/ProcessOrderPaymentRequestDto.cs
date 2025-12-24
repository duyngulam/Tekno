using System.ComponentModel.DataAnnotations;
using Tekno.Domain.Payment;

namespace Tekno.Application.Payment.DTOs
{
    /// <summary>
    /// Request to process payment for an existing pending order (Step 2 of checkout)
    /// </summary>
    public class ProcessOrderPaymentRequestDto
    {
        /// <summary>
        /// Order ID to process payment for
        /// </summary>
        [Required]
        public int OrderId { get; set; }

        /// <summary>
        /// Shipping address ID (required for order fulfillment)
        /// </summary>
        [Required(ErrorMessage = "Shipping address is required")]
        public int ShippingAddressId { get; set; }

        /// <summary>
        /// Optional coupon code for discount
        /// </summary>
        public string? CouponCode { get; set; }

        /// <summary>
        /// Payment gateway to use
        /// </summary>
        [Required]
        public PaymentGateway Gateway { get; set; }

        /// <summary>
        /// Payment method
        /// </summary>
        [Required]
        public PaymentMethod Method { get; set; }

        /// <summary>
        /// Frontend URL to redirect after payment
        /// </summary>
        [Required]
        [Url(ErrorMessage = "Invalid return URL format")]
        public string ReturnUrl { get; set; } = "http://localhost:3000/payment/result";
    }
}
