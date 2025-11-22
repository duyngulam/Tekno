using System;
using System.Collections.Generic;

namespace Tekno.Domain.Promotion
{
    public class Coupon
    {
        public int Id { get; private set; }
        public string Code { get; private set; } = string.Empty; // PHVC000003
        public string Name { get; private set; } = string.Empty; // Return, Summer, Holiday
        public CouponType Type { get; private set; } = CouponType.FixedAmount;
        public decimal Value { get; private set; } // 300,000 VND or percentage
        public int Quantity { get; private set; } // Total available
        public int UsedCount { get; private set; } = 0; // How many times used
        public int? MaxUsagePerUser { get; private set; } // Limit per user (null = unlimited)
        
        public decimal? MinPurchaseAmount { get; private set; } // Minimum order value
        public decimal? MaxDiscountAmount { get; private set; } // Cap for percentage discounts
        
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public CouponStatus Status { get; private set; } = CouponStatus.Active;
        
        public string? Note { get; private set; }
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;

        // Relationships
        public ICollection<CouponCategory> ApplicableCategories { get; private set; } = new List<CouponCategory>();
        public ICollection<CouponProduct> ApplicableProducts { get; private set; } = new List<CouponProduct>();
        public ICollection<CouponUsage> Usages { get; private set; } = new List<CouponUsage>();

        // Computed properties
        public int RemainingQuantity => Quantity - UsedCount;
        public bool IsExpired => DateTime.UtcNow > EndDate || DateTime.UtcNow < StartDate;
        public bool IsAvailable => Status == CouponStatus.Active && !IsExpired && RemainingQuantity > 0;

        // Constructors
        public Coupon() { }

        public Coupon(
            string code,
            string name,
            CouponType type,
            decimal value,
            int quantity,
            DateTime startDate,
            DateTime endDate,
            decimal? minPurchaseAmount = null,
            decimal? maxDiscountAmount = null,
            int? maxUsagePerUser = null,
            string? note = null)
        {
            Code = code;
            Name = name;
            Type = type;
            Value = value;
            Quantity = quantity;
            StartDate = startDate;
            EndDate = endDate;
            MinPurchaseAmount = minPurchaseAmount;
            MaxDiscountAmount = maxDiscountAmount;
            MaxUsagePerUser = maxUsagePerUser;
            Note = note;
            Status = CouponStatus.Active;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        // Methods
        public void Update(
            string name,
            decimal value,
            int quantity,
            DateTime startDate,
            DateTime endDate,
            decimal? minPurchaseAmount,
            decimal? maxDiscountAmount,
            int? maxUsagePerUser,
            string? note)
        {
            Name = name;
            Value = value;
            Quantity = quantity;
            StartDate = startDate;
            EndDate = endDate;
            MinPurchaseAmount = minPurchaseAmount;
            MaxDiscountAmount = maxDiscountAmount;
            MaxUsagePerUser = maxUsagePerUser;
            Note = note;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Activate() => Status = CouponStatus.Active;
        public void Deactivate() => Status = CouponStatus.Inactive;
        public void MarkAsExpired() => Status = CouponStatus.Expired;

        public void IncrementUsage()
        {
            if (UsedCount >= Quantity)
                throw new InvalidOperationException("Coupon quantity exceeded");
            
            UsedCount++;
            UpdatedAt = DateTime.UtcNow;
        }

        public decimal CalculateDiscount(decimal orderAmount)
        {
            if (Type == CouponType.Percentage)
            {
                var discount = orderAmount * (Value / 100);
                return MaxDiscountAmount.HasValue 
                    ? Math.Min(discount, MaxDiscountAmount.Value) 
                    : discount;
            }
            
            return Value; // Fixed amount
        }

        public void AddApplicableCategory(int categoryId)
        {
            ApplicableCategories.Add(new CouponCategory { CouponId = Id, CategoryId = categoryId });
        }

        public void AddApplicableProduct(int productId)
        {
            ApplicableProducts.Add(new CouponProduct { CouponId = Id, ProductId = productId });
        }
    }

    public enum CouponType
    {
        FixedAmount = 1,    // Fixed discount in currency
        Percentage = 2,     // Percentage discount
        FreeShipping = 3    // Free shipping
    }

    public enum CouponStatus
    {
        Active = 1,
        Inactive = 2,
        Expired = 3
    }
}
