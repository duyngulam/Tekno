using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tekno.Domain.Catalog
{
    public class ProductVariant
    {
        public int Id { get; private set; }
        public int ProductId { get; private set; }
        public string Sku { get; private set; } = string.Empty;
        public decimal Price { get; private set; }
        public int Stock { get; private set; }
        public string Status { get; private set; } = "available";
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        public string? VariantSpecsJson { get; set; } = null;// specs riêng (ghi đè nếu cần)

        public Product Product { get; private set; } = null!;
        public ICollection<ProductVariantAttribute> VariantAttributes { get; private set; } = new List<ProductVariantAttribute>();

        public ProductVariant() { }

        public ProductVariant(int productId, string sku, decimal price, int stock, string status = "available")
        {
            ProductId = productId;
            Sku = sku;
            Price = price;
            Stock = stock;
            Status = status;
            CreatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Add attribute to variant
        /// </summary>
        public void AddAttribute(int attributeId, int valueId)
        {
            var attribute = new ProductVariantAttribute(Id, attributeId, valueId);
            VariantAttributes.Add(attribute);
        }

        /// <summary>
        /// Update variant stock
        /// </summary>
        public void UpdateStock(int newStock)
        {
            if (newStock < 0)
                throw new ArgumentException("Stock cannot be negative", nameof(newStock));
            
            Stock = newStock;
        }

        /// <summary>
        /// Reduce stock by quantity (for order fulfillment)
        /// </summary>
        public void ReduceStock(int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be positive", nameof(quantity));

            if (Stock < quantity)
                throw new InvalidOperationException($"Insufficient stock. Available: {Stock}, Requested: {quantity}");

            Stock -= quantity;
        }

        /// <summary>
        /// Increase stock by quantity (for order cancellation/refund)
        /// </summary>
        public void IncreaseStock(int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be positive", nameof(quantity));

            Stock += quantity;
        }

        /// <summary>
        /// Update variant price
        /// </summary>
        public void UpdatePrice(decimal newPrice)
        {
            if (newPrice < 0)
                throw new ArgumentException("Price cannot be negative", nameof(newPrice));
            
            Price = newPrice;
        }

        /// <summary>
        /// Update variant status
        /// </summary>
        public void UpdateStatus(string status)
        {
            if (string.IsNullOrWhiteSpace(status))
                throw new ArgumentException("Status cannot be empty", nameof(status));
            
            Status = status;
        }

        /// <summary>
        /// Update variant SKU
        /// </summary>
        public void UpdateSku(string sku)
        {
            if (string.IsNullOrWhiteSpace(sku))
                throw new ArgumentException("SKU cannot be empty", nameof(sku));

            Sku = sku;
        }
    }
}
