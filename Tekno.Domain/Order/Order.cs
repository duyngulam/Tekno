using System;
using System.Collections.Generic;
using System.Linq;

namespace Tekno.Domain.Order
{
    /// <summary>
    /// Simplified Order entity for purchase verification
    /// Full order system can be expanded later
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

        public ICollection<OrderItem> Items { get; private set; } = new List<OrderItem>();

        public Order() { }

        public Order(int userId, string orderNumber, decimal totalAmount)
        {
            UserId = userId;
            OrderNumber = orderNumber;
            TotalAmount = totalAmount;
            Status = OrderStatus.Pending;
            CreatedAt = DateTime.UtcNow;
        }

        public void Complete()
        {
            Status = OrderStatus.Completed;
            CompletedAt = DateTime.UtcNow;
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
        Pending = 1,
        Processing = 2,
        Completed = 3,
        Cancelled = 4
    }
}
