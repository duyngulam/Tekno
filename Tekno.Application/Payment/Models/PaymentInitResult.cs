namespace Tekno.Application.Payment.Models
{
    /// <summary>
    /// Payment initiation result from gateway
    /// </summary>
    public class PaymentInitResult
    {
        public bool Success { get; set; }
        public string TransactionId { get; set; } = string.Empty;
        public string PaymentUrl { get; set; } = string.Empty; // URL to redirect customer
        public string? PaymentToken { get; set; } // Token for embedded payment
        public string? QrCodeUrl { get; set; } // For QR payment methods
        public string? ErrorMessage { get; set; }
        public object? GatewayResponse { get; set; } // Original response from gateway
    }
}
