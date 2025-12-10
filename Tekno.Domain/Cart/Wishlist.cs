using System;

namespace Tekno.Domain.Cart
{
    public class Wishlist
    {
        public int Id { get; private set; }
        public int UserId { get; private set; }
        public int ProductId { get; private set; }
        public DateTime AddedAt { get; private set; } = DateTime.UtcNow;

        // Navigation property
        public Tekno.Domain.Catalog.Product? Product { get; private set; }

        public Wishlist() { }

        public Wishlist(int userId, int productId)
        {
            UserId = userId;
            ProductId = productId;
            AddedAt = DateTime.UtcNow;
        }
    }
}
