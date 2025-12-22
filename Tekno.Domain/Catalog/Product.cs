using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tekno.Domain.Catalog
{
    public class Product
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public int BrandId { get; set; }
        public string Name { get;   set; } = string.Empty;
        public string Slug { get;   set; } = string.Empty;
        public decimal? DiscountPercent { get; set; } // Managed by Promotion system
        public string Status { get; set; } = "available";
        public decimal BasePrice { get; set; }
        public string? Description { get; set; }
        public string? Overview { get; set; }
        public string? Specs { get; set; } = "{}"; // JSONB
        public int TotalSold { get; set; } = 0; // Track total units sold
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public Category Category { get; set; } = null!;
        public Brand Brand { get; set; } = null!;
        public ICollection<ProductVariant> Variants { get; set; } = new List<ProductVariant>();
        public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
        
        public void AddImage(string url, bool isPrimary = false, int sortOrder = 0)
        {
            Images.Add(new ProductImage(Id,url, isPrimary, sortOrder));
        }

        public void IncrementSoldCount(int quantity)
        {
            TotalSold += quantity;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Update product specifications JSON
        /// </summary>
        public void UpdateSpecs(string specsJson)
        {
            Specs = specsJson;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Check if product has an active discount
        /// </summary>
        public bool HasActiveDiscount()
        {
            return DiscountPercent.HasValue && DiscountPercent.Value > 0;
        }

        /// <summary>
        /// Get effective price after applying discount
        /// </summary>
        public decimal GetEffectivePrice()
        {
            if (HasActiveDiscount() && DiscountPercent.HasValue)
            {
                return BasePrice * (1 - DiscountPercent.Value / 100);
            }
            return BasePrice;
        }

        /// <summary>
        /// Set product discount (managed by Promotion system)
        /// </summary>
        public void SetDiscount(decimal? discountPercent)
        {
            DiscountPercent = discountPercent;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Remove product discount
        /// </summary>
        public void RemoveDiscount()
        {
            DiscountPercent = null;
            UpdatedAt = DateTime.UtcNow;
        }

        public Product() { }

        public Product(string name, string slug, int categoryId, int brandId, decimal basePrice)
        {
            Name = name;
            Slug = slug;
            CategoryId = categoryId;
            BrandId = brandId;
            BasePrice = basePrice;
            TotalSold = 0;
        }
    }
}
