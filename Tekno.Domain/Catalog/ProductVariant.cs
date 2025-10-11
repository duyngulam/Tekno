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

        public Product Product { get; private set; } = null!;
        public ICollection<ProductVariantAttribute> VariantAttributes { get; private set; } = new List<ProductVariantAttribute>();

        public ProductVariant() { }

        public ProductVariant(int productId, string sku, decimal price, int stock)
        {
            ProductId = productId;
            Sku = sku;
            Price = price;
            Stock = stock;
        }
    }
}
