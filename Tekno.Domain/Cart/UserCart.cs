using System;
using System.Collections.Generic;
using System.Linq;

namespace Tekno.Domain.Cart
{
    public class UserCart
    {
        public int Id { get; private set; }
        public int UserId { get; private set; }
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;

        public ICollection<CartItem> Items { get; private set; } = new List<CartItem>();

        // Computed properties
        public decimal Subtotal => Items.Sum(i => i.TotalPrice);
        public int TotalItems => Items.Sum(i => i.Quantity);

        public UserCart() { }

        public UserCart(int userId)
        {
            UserId = userId;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public void AddItem(int variantId, int quantity, decimal price)
        {
            var existingItem = Items.FirstOrDefault(i => i.VariantId == variantId);
            
            if (existingItem != null)
            {
                existingItem.UpdateQuantity(existingItem.Quantity + quantity);
            }
            else
            {
                var newItem = new CartItem(Id, variantId, quantity, price);
                Items.Add(newItem);
            }

            UpdatedAt = DateTime.UtcNow;
        }

        public void RemoveItem(int variantId)
        {
            var item = Items.FirstOrDefault(i => i.VariantId == variantId);
            if (item != null)
            {
                Items.Remove(item);
                UpdatedAt = DateTime.UtcNow;
            }
        }

        public void UpdateItemQuantity(int variantId, int quantity)
        {
            var item = Items.FirstOrDefault(i => i.VariantId == variantId);
            if (item != null)
            {
                item.UpdateQuantity(quantity);
                UpdatedAt = DateTime.UtcNow;
            }
        }

        public void Clear()
        {
            Items.Clear();
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public class CartItem
    {
        public int Id { get; private set; }
        public int CartId { get; private set; }
        public int VariantId { get; private set; }
        public int Quantity { get; private set; }
        public decimal Price { get; private set; } // Price at time of adding to cart
        public DateTime AddedAt { get; private set; } = DateTime.UtcNow;

        // Navigation properties
        public UserCart Cart { get; private set; } = null!;
        public Tekno.Domain.Catalog.ProductVariant? Variant { get; private set; }

        public decimal TotalPrice => Price * Quantity;

        public CartItem() { }

        public CartItem(int cartId, int variantId, int quantity, decimal price)
        {
            CartId = cartId;
            VariantId = variantId;
            Quantity = quantity;
            Price = price;
            AddedAt = DateTime.UtcNow;
        }

        public void UpdateQuantity(int quantity)
        {
            if (quantity <= 0)
                throw new InvalidOperationException("Quantity must be greater than 0");
            
            Quantity = quantity;
        }

        public void UpdatePrice(decimal price)
        {
            if (price < 0)
                throw new InvalidOperationException("Price cannot be negative");
            
            Price = price;
        }
    }
}
