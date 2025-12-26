using System;
using System.Collections.Generic;
using System.Linq;

namespace Tekno.Domain.Order
{
    /// <summary>
    /// Order entity with delivery tracking
    /// </summary>
    public class Order
    {
        public int Id { get; private set; }
        public int UserId { get; private set; }
        public string OrderNumber { get; private set; } = string.Empty;
        public OrderStatus Status { get; private set; }
        public decimal TotalAmount { get; private set; }
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; private set; }
        
        // Shipping address
        public int? ShippingAddressId { get; private set; }
        public Auth.UserAddress? ShippingAddress { get; private set; }
        
        // Delivery tracking
        public DateTime? ShippedAt { get; private set; }
        public DateTime? DeliveredAt { get; private set; }
        public string? TrackingNumber { get; private set; }
        public string? ShippingCarrier { get; private set; }
        
        // Customer note
        public string? CustomerNote { get; private set; }
        
        // Coupon/Discount
        public string? CouponCode { get; private set; }
        public decimal DiscountAmount { get; private set; }

        private readonly List<OrderItem> _items = new();
        public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();
        
        // Navigation property to Payment
        public Payment.Payment? Payment { get; private set; }

        public Order() { }

        public Order(int userId, string orderNumber, decimal totalAmount, string? customerNote = null)
        {
            UserId = userId;
            OrderNumber = orderNumber;
            TotalAmount = totalAmount;
            CustomerNote = customerNote;
            Status = OrderStatus.Pending;
            CreatedAt = DateTime.UtcNow;
        }

        public void AddItem(int productId, int variantId, int quantity, decimal price)
        {
            var item = new OrderItem(Id, productId, variantId, quantity, price);
            _items.Add(item);
        }

        public void SetShippingAddress(int shippingAddressId)
        {
            ShippingAddressId = shippingAddressId;
        }

        public void ApplyCoupon(string couponCode, decimal discountAmount)
        {
            CouponCode = couponCode;
            DiscountAmount = discountAmount;
            
            // Update total amount with discount
            TotalAmount = Math.Max(0, TotalAmount - discountAmount);
        }

        public void Complete()
        {
            Status = OrderStatus.Completed;
            CompletedAt = DateTime.UtcNow;
        }

        public void MarkAsProcessing()
        {
            Status = OrderStatus.Processing;
        }

        public void Ship(string? trackingNumber = null, string? carrier = null)
        {
            Status = OrderStatus.Shipping;
            ShippedAt = DateTime.UtcNow;
            TrackingNumber = trackingNumber;
            ShippingCarrier = carrier;
        }

        public void Deliver()
        {
            Status = OrderStatus.Delivered;
            DeliveredAt = DateTime.UtcNow;
        }

        public void Cancel(string? reason = null)
        {
            Status = OrderStatus.Cancelled;
            // You can add CancelReason property if needed
        }

        public void RequestRefund()
        {
            Status = OrderStatus.RefundRequested;
        }

        public void Refund()
        {
            Status = OrderStatus.Refunded;
        }

        /// <summary>
        /// Reactivate a cancelled order back to Pending state for payment retry
        /// Only allowed if order was cancelled due to payment timeout/failure
        /// </summary>
        public void ReactivateForPaymentRetry()
        {
            if (Status != OrderStatus.Cancelled)
            {
                throw new InvalidOperationException(
                    $"Cannot reactivate order in {Status} status. Only Cancelled orders can be reactivated.");
            }

            // Additional validation: Order shouldn't be too old (e.g., within 24 hours)
            var orderAge = DateTime.UtcNow - CreatedAt;
            if (orderAge.TotalHours > 24)
            {
                throw new InvalidOperationException(
                    $"Order is too old to reactivate ({orderAge.TotalHours:F1} hours). Maximum age is 24 hours.");
            }

            Status = OrderStatus.Pending;
        }

        /// <summary>
        /// Check if order can be retried for payment
        /// Order must be Pending or Cancelled (due to payment failure) and not too old
        /// </summary>
        public bool CanRetryPayment()
        {
            // Allow retry if Pending (original state) or Cancelled (after payment failure)
            if (Status != OrderStatus.Pending && Status != OrderStatus.Cancelled)
            {
                return false;
            }

            // Check order age - don't allow retry for orders older than 24 hours
            var orderAge = DateTime.UtcNow - CreatedAt;
            return orderAge.TotalHours <= 24;
        }

        public bool HasPurchasedProduct(int productId)
        {
            return Status == OrderStatus.Completed && 
                   Items.Any(i => i.ProductId == productId);
        }

        public bool HasPurchasedVariant(int variantId)
        {
            return Status == OrderStatus.Completed && 
                   Items.Any(i => i.VariantId == variantId);
        }
    }

    public class OrderItem
    {
        public int Id { get; private set; }
        public int OrderId { get; private set; }
        public int ProductId { get; private set; }
        public int VariantId { get; private set; }
        public int Quantity { get; private set; }
        public decimal Price { get; private set; }
        public decimal TotalPrice => Price * Quantity;

        public Order Order { get; private set; } = null!;

        public OrderItem() { }

        public OrderItem(int orderId, int productId, int variantId, int quantity, decimal price)
        {
            OrderId = orderId;
            ProductId = productId;
            VariantId = variantId;
            Quantity = quantity;
            Price = price;
        }
    }

    public enum OrderStatus
    {
        Pending = 1,           // Order created, awaiting payment
        Processing = 2,        // Payment received, preparing order
        Completed = 3,         // Legacy - use Shipping/Delivered instead
        Shipping = 4,          // Order shipped, on the way
        Delivered = 5,         // Order delivered to customer
        Cancelled = 6,         // Order cancelled
        RefundRequested = 7,   // Customer requested refund
        Refunded = 8          // Order refunded
    }
}
