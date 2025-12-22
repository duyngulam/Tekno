using System;
using System.Collections.Generic;

namespace Tekno.Domain.Promotion
{
    /// <summary>
    /// Represents a bulk promotion that automatically applies discounts to products/categories
    /// Unlike Coupons (which require user input), Promotions are automatic and managed by the system
    /// </summary>
    public class Promotion
    {
        public int Id { get; private set; }
        public string Name { get; private set; } = string.Empty; // "Black Friday Sale", "Smartphone Week"
        public string? Description { get; private set; }
        
        public PromotionType Type { get; private set; } = PromotionType.Percentage;
        public decimal Value { get; private set; } // Discount percentage (e.g., 10 = 10%)
        
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public PromotionStatus Status { get; private set; } = PromotionStatus.Scheduled;
        
        public int Priority { get; private set; } = 0; // Higher priority promotions override lower ones
        public bool StackableWithCoupons { get; private set; } = true; // Can use coupons on promoted products
        
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;

        // Relationships
        public ICollection<PromotionCategory> ApplicableCategories { get; private set; } = new List<PromotionCategory>();
        public ICollection<PromotionProduct> ApplicableProducts { get; private set; } = new List<PromotionProduct>();

        // Computed properties
        public bool IsActive => Status == PromotionStatus.Active && 
                                DateTime.UtcNow >= StartDate && 
                                DateTime.UtcNow <= EndDate;
        
        public bool IsScheduled => Status == PromotionStatus.Scheduled && 
                                   DateTime.UtcNow < StartDate;
        
        public bool IsExpired => DateTime.UtcNow > EndDate;

        // Constructors
        public Promotion() { }

        public Promotion(
            string name,
            string? description,
            PromotionType type,
            decimal value,
            DateTime startDate,
            DateTime endDate,
            int priority = 0,
            bool stackableWithCoupons = true)
        {
            Name = name;
            Description = description;
            Type = type;
            Value = value;
            StartDate = startDate;
            EndDate = endDate;
            Priority = priority;
            StackableWithCoupons = stackableWithCoupons;
            Status = DateTime.UtcNow < startDate ? PromotionStatus.Scheduled : PromotionStatus.Active;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        // Methods
        public void Update(
            string name,
            string? description,
            decimal value,
            DateTime startDate,
            DateTime endDate,
            int priority,
            bool stackableWithCoupons)
        {
            Name = name;
            Description = description;
            Value = value;
            StartDate = startDate;
            EndDate = endDate;
            Priority = priority;
            StackableWithCoupons = stackableWithCoupons;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Activate()
        {
            Status = PromotionStatus.Active;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Pause()
        {
            Status = PromotionStatus.Paused;
            UpdatedAt = DateTime.UtcNow;
        }

        public void MarkAsExpired()
        {
            Status = PromotionStatus.Expired;
            UpdatedAt = DateTime.UtcNow;
        }

        public void AddCategory(int categoryId)
        {
            ApplicableCategories.Add(new PromotionCategory { PromotionId = Id, CategoryId = categoryId });
        }

        public void AddProduct(int productId)
        {
            ApplicableProducts.Add(new PromotionProduct { PromotionId = Id, ProductId = productId });
        }

        /// <summary>
        /// Calculate discount for a product price
        /// </summary>
        public decimal CalculateDiscount(decimal productPrice)
        {
            if (Type == PromotionType.Percentage)
            {
                return productPrice * (Value / 100);
            }
            return Value; // Fixed amount
        }

        /// <summary>
        /// Get final price after applying promotion
        /// </summary>
        public decimal GetDiscountedPrice(decimal originalPrice)
        {
            var discount = CalculateDiscount(originalPrice);
            return Math.Max(0, originalPrice - discount);
        }
    }

    public enum PromotionType
    {
        Percentage = 1,  // Percentage discount (most common for bulk promotions)
        FixedAmount = 2  // Fixed amount off each product
    }

    public enum PromotionStatus
    {
        Scheduled = 1,  // Not started yet
        Active = 2,     // Currently running
        Paused = 3,     // Temporarily paused by admin
        Expired = 4     // Past end date
    }

    // Many-to-many: Promotion <-> Category
    public class PromotionCategory
    {
        public int PromotionId { get; set; }
        public int CategoryId { get; set; }
        public Promotion Promotion { get; set; } = null!;
    }

    // Many-to-many: Promotion <-> Product
    public class PromotionProduct
    {
        public int PromotionId { get; set; }
        public int ProductId { get; set; }
        public Promotion Promotion { get; set; } = null!;
    }
}
