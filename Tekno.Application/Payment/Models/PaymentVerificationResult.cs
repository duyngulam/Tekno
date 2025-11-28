namespace Tekno.Application.Payment.Models
{
    /// <summary>
    /// Payment verification result from gateway callback
    /// </summary>
    public class PaymentVerificationResult
    {
        public bool IsValid { get; set; }
        public bool IsSuccessful { get; set; }
        public string TransactionId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string? ErrorMessage { get; set; }
        public object? GatewayResponse { get; set; }
    }
}
