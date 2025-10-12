using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tekno.Domain.Catalog
{
    public class Product
    {
        public int Id { get; private set; }
        public int CategoryId { get; private set; }
        public int BrandId { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string Slug { get; private set; } = string.Empty;
        public string Status { get; private set; } = "available";
        public decimal BasePrice { get; private set; }
        public string? Description { get; private set; }
        public string? Overview { get; private set; }
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;

        public Category Category { get; private set; } = null!;
        public Brand Brand { get; private set; } = null!;
        public ProductDetail? Detail { get; private set; }
        public ICollection<ProductVariant> Variants { get; private set; } = new List<ProductVariant>();
        public ICollection<ProductImage> Images { get; private set; } = new List<ProductImage>();

        public Product() { }

        public Product(string name, string slug, int categoryId, int brandId, decimal basePrice)
        {
            Name = name;
            Slug = slug;
            CategoryId = categoryId;
            BrandId = brandId;
            BasePrice = basePrice;
        }
    }
}
