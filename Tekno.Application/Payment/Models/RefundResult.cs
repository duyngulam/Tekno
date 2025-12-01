namespace Tekno.Application.Payment.Models
{
    /// <summary>
    /// Refund operation result
    /// </summary>
    public class RefundResult
    {
        public bool Success { get; set; }
        public string RefundId { get; set; } = string.Empty;
        public decimal RefundedAmount { get; set; }
        public string? ErrorMessage { get; set; }
        public object? GatewayResponse { get; set; }
    }
}
