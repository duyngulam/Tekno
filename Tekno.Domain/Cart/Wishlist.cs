using System;

namespace Tekno.Domain.Cart
{
    public class Wishlist
    {
        public int Id { get; private set; }
        public int UserId { get; private set; }
        public int VariantId { get; private set; }
        public DateTime AddedAt { get; private set; } = DateTime.UtcNow;

        // Navigation property
        public Tekno.Domain.Catalog.ProductVariant? Variant { get; private set; }

        public Wishlist() { }

        public Wishlist(int userId, int variantId)
        {
            UserId = userId;
            VariantId = variantId;
            AddedAt = DateTime.UtcNow;
        }
    }
}
