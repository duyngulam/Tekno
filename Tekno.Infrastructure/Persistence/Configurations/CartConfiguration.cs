using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using Tekno.Domain.Cart;

namespace Tekno.Infrastructure.Persistence.Configurations
{
    public class CartConfiguration : IEntityTypeConfiguration<UserCart>
    {
        private static readonly DateTime SeedTime = new DateTime(2025, 1, 12, 14, 0, 0, DateTimeKind.Utc);
        
        public void Configure(EntityTypeBuilder<UserCart> builder)
        {
            builder.ToTable("user_carts");
            builder.HasKey(c => c.Id);

            builder.Property(c => c.UserId)
                .IsRequired();

            builder.HasIndex(c => c.UserId).IsUnique();

            builder.Property(c => c.CreatedAt)
                .HasColumnType("timestamptz")
                .HasDefaultValueSql("NOW()");

            builder.Property(c => c.UpdatedAt)
                .HasColumnType("timestamptz")
                .HasDefaultValueSql("NOW()");

            builder.HasMany(c => c.Items)
                .WithOne(i => i.Cart)
                .HasForeignKey(i => i.CartId)
                .OnDelete(DeleteBehavior.Cascade);

            // ========== SEED CART DATA (Customer User Only) ==========
            builder.HasData(
                // Cart for Customer User (ID: 2)
                new
                {
                    Id = 1,
                    UserId = 2, // Customer user
                    CreatedAt = new DateTime(2025, 1, 12, 9, 30, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2025, 1, 14, 15, 20, 0, DateTimeKind.Utc)
                }
            );
        }
    }

    public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
    {
        public void Configure(EntityTypeBuilder<CartItem> builder)
        {
            builder.ToTable("cart_items");
            builder.HasKey(i => i.Id);

            builder.Property(i => i.CartId)
                .IsRequired();

            builder.Property(i => i.VariantId)
                .IsRequired();

            builder.Property(i => i.Quantity)
                .IsRequired()
                .HasDefaultValue(1);

            builder.Property(i => i.Price)
                .HasPrecision(12, 2)
                .IsRequired();

            builder.Property(i => i.AddedAt)
                .HasColumnType("timestamptz")
                .HasDefaultValueSql("NOW()");

            // Unique constraint: one variant per cart
            builder.HasIndex(i => new { i.CartId, i.VariantId }).IsUnique();

            builder.HasOne(i => i.Cart)
                .WithMany(c => c.Items)
                .HasForeignKey(i => i.CartId);

            // ========== SEED CART ITEMS DATA (Customer User Only) ==========
            builder.HasData(
                // Cart 1 items (Customer User): iPhone + AirPods + Case
                new
                {
                    Id = 1,
                    CartId = 1,
                    VariantId = 11, // iPhone 15 Pro Max 256GB Natural
                    Quantity = 1,
                    Price = 33990000m,
                    AddedAt = new DateTime(2025, 1, 12, 9, 30, 0, DateTimeKind.Utc)
                },
                new
                {
                    Id = 2,
                    CartId = 1,
                    VariantId = 42, // AirPods Pro 2
                    Quantity = 1,
                    Price = 6490000m,
                    AddedAt = new DateTime(2025, 1, 13, 14, 15, 0, DateTimeKind.Utc)
                },
                new
                {
                    Id = 3,
                    CartId = 1,
                    VariantId = 46, // Spigen Case Black
                    Quantity = 1,
                    Price = 490000m,
                    AddedAt = new DateTime(2025, 1, 14, 15, 20, 0, DateTimeKind.Utc)
                }
            );
        }
    }

    public class WishlistConfiguration : IEntityTypeConfiguration<Wishlist>
    {
        public void Configure(EntityTypeBuilder<Wishlist> builder)
        {
            builder.ToTable("wishlists");
            builder.HasKey(w => w.Id);

            builder.Property(w => w.UserId)
                .IsRequired();

            builder.Property(w => w.ProductId)
                .IsRequired();

            builder.Property(w => w.AddedAt)
                .HasColumnType("timestamptz")
                .HasDefaultValueSql("NOW()");

            // Unique constraint: one product per user wishlist
            builder.HasIndex(w => new { w.UserId, w.ProductId }).IsUnique();

            // Relationship with Product
            builder.HasOne(w => w.Product)
                .WithMany()
                .HasForeignKey(w => w.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // ========== SEED WISHLIST DATA (Customer User Only) ==========
            builder.HasData(
                // Customer user wishlist - Saving for future purchases
                new
                {
                    Id = 1,
                    UserId = 2, // Customer user
                    ProductId = 2, // MacBook Air M2
                    AddedAt = new DateTime(2025, 1, 10, 9, 0, 0, DateTimeKind.Utc)
                },
                new
                {
                    Id = 2,
                    UserId = 2, // Customer user
                    ProductId = 21, // Samsung Galaxy Tab S9
                    AddedAt = new DateTime(2025, 1, 11, 14, 30, 0, DateTimeKind.Utc)
                },
                new
                {
                    Id = 3,
                    UserId = 2, // Customer user
                    ProductId = 30, // Dell UltraSharp Monitor
                    AddedAt = new DateTime(2025, 1, 12, 16, 45, 0, DateTimeKind.Utc)
                }
            );
        }
    }
}
