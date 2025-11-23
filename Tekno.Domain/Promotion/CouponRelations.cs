using System;

namespace Tekno.Domain.Promotion
{
    // Many-to-many relationship: Coupon <-> Category
    public class CouponCategory
    {
        public int CouponId { get; set; }
        public int CategoryId { get; set; }
        
        public Coupon Coupon { get; set; } = null!;
        // Navigation to Category can be added if needed
    }

    // Many-to-many relationship: Coupon <-> Product
    public class CouponProduct
    {
        public int CouponId { get; set; }
        public int ProductId { get; set; }
        
        public Coupon Coupon { get; set; } = null!;
        // Navigation to Product can be added if needed
    }

    // Track coupon usage by users
    public class CouponUsage
    {
        public int Id { get; set; }
        public int CouponId { get; set; }
        public int UserId { get; set; }
        public int OrderId { get; set; }
        public decimal DiscountAmount { get; set; }
        public DateTime UsedAt { get; set; } = DateTime.UtcNow;

        public Coupon Coupon { get; set; } = null!;
        // Navigation to User and Order can be added
    }
}
