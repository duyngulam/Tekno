using Tekno.Domain.Payment;

namespace Tekno.Application.Payment.Models
{
    /// <summary>
    /// Payment request data for gateway operations
    /// </summary>
    public class PaymentRequest
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "VND";
        public PaymentMethod Method { get; set; }
        public string ReturnUrl { get; set; } = string.Empty; // URL to redirect after payment
        public string CallbackUrl { get; set; } = string.Empty; // Webhook URL
        public string CustomerEmail { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
    }
}
