using System;

namespace Tekno.Domain.Payment
{
    /// <summary>
    /// Payment transaction record
    /// </summary>
    public class Payment
    {
        public int Id { get; private set; }
        public int OrderId { get; private set; }
        public int UserId { get; private set; }
        
        public string TransactionId { get; private set; } = string.Empty; // Gateway transaction ID
        public PaymentGateway Gateway { get; private set; }
        public PaymentMethod Method { get; private set; }
        public PaymentStatus Status { get; private set; }
        
        public decimal Amount { get; private set; }
        public string Currency { get; private set; } = "VND";
        
        public string? GatewayResponse { get; private set; } // JSON response from gateway
        public string? ErrorMessage { get; private set; }
        
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; private set; }
        public DateTime? FailedAt { get; private set; }

        // Navigation
        public Order.Order Order { get; private set; } = null!;

        private Payment() { }

        public Payment(
            int orderId, 
            int userId, 
            PaymentGateway gateway, 
            PaymentMethod method, 
            decimal amount, 
            string currency = "VND")
        {
            OrderId = orderId;
            UserId = userId;
            Gateway = gateway;
            Method = method;
            Amount = amount;
            Currency = currency;
            Status = PaymentStatus.Pending;
            CreatedAt = DateTime.UtcNow;
        }

        public void MarkAsProcessing(string transactionId)
        {
            TransactionId = transactionId;
            Status = PaymentStatus.Processing;
        }

        public void MarkAsCompleted(string? gatewayResponse = null)
        {
            Status = PaymentStatus.Completed;
            CompletedAt = DateTime.UtcNow;
            GatewayResponse = gatewayResponse;
        }

        public void MarkAsFailed(string errorMessage, string? gatewayResponse = null)
        {
            Status = PaymentStatus.Failed;
            FailedAt = DateTime.UtcNow;
            ErrorMessage = errorMessage;
            GatewayResponse = gatewayResponse;
        }

        public void MarkAsRefunded()
        {
            Status = PaymentStatus.Refunded;
        }
    }

    public enum PaymentGateway
    {
        Mock = 0,        // For testing
        Stripe = 1,
        PayPal = 2,
        VNPay = 3,       // Vietnam payment gateway
        MoMo = 4,        // Vietnam mobile wallet
        ZaloPay = 5      // Vietnam payment
    }

    public enum PaymentMethod
    {
        CreditCard = 1,
        DebitCard = 2,
        BankTransfer = 3,
        EWallet = 4,
        Cash = 5
    }

    public enum PaymentStatus
    {
        Pending = 1,     // Initial state
        Processing = 2,  // Payment initiated
        Completed = 3,   // Payment successful
        Failed = 4,      // Payment failed
        Refunded = 5,    // Payment refunded
        Cancelled = 6    // Payment cancelled
    }
}
