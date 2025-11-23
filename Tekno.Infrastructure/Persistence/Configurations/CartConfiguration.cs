using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tekno.Domain.Cart;

namespace Tekno.Infrastructure.Persistence.Configurations
{
    public class CartConfiguration : IEntityTypeConfiguration<UserCart>
    {
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

            builder.Property(w => w.VariantId)
                .IsRequired();

            builder.Property(w => w.AddedAt)
                .HasColumnType("timestamptz")
                .HasDefaultValueSql("NOW()");

            // Unique constraint: one variant per user wishlist
            builder.HasIndex(w => new { w.UserId, w.VariantId }).IsUnique();
        }
    }
}
