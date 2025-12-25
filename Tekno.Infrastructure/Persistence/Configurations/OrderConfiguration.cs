using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderEntity = Tekno.Domain.Order.Order;
using OrderItemEntity = Tekno.Domain.Order.OrderItem;
using Tekno.Domain.Order;

namespace Tekno.Infrastructure.Persistence.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<OrderEntity>
    {
        public void Configure(EntityTypeBuilder<OrderEntity> builder)
        {
            builder.ToTable("orders");
            builder.HasKey(o => o.Id);

            builder.Property(o => o.UserId)
                .IsRequired();

            builder.Property(o => o.OrderNumber)
                .HasMaxLength(50)
                .IsRequired();

            builder.HasIndex(o => o.OrderNumber).IsUnique();

            builder.Property(o => o.Status)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired()
                .HasDefaultValue(OrderStatus.Pending);

            builder.Property(o => o.TotalAmount)
                .HasPrecision(12, 2)
                .IsRequired();

            // Shipping Address ID (nullable - set during payment)
            builder.Property(o => o.ShippingAddressId)
                .IsRequired(false);

            // Shipping Address Navigation Property
            builder.HasOne(o => o.ShippingAddress)
                .WithMany()
                .HasForeignKey(o => o.ShippingAddressId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            // Coupon/Discount
            builder.Property(o => o.CouponCode)
                .HasMaxLength(50)
                .IsRequired(false);

            builder.Property(o => o.DiscountAmount)
                .HasPrecision(12, 2)
                .HasDefaultValue(0);

            // Customer Note
            builder.Property(o => o.CustomerNote)
                .HasMaxLength(500)
                .IsRequired(false);

            builder.Property(o => o.CreatedAt)
                .HasColumnType("timestamptz")
                .HasDefaultValueSql("NOW()");

            builder.Property(o => o.CompletedAt)
                .HasColumnType("timestamptz")
                .IsRequired(false);

            // Delivery tracking
            builder.Property(o => o.ShippedAt)
                .HasColumnType("timestamptz")
                .IsRequired(false);

            builder.Property(o => o.DeliveredAt)
                .HasColumnType("timestamptz")
                .IsRequired(false);

            builder.Property(o => o.TrackingNumber)
                .HasMaxLength(100)
                .IsRequired(false);

            builder.Property(o => o.ShippingCarrier)
                .HasMaxLength(100)
                .IsRequired(false);

            builder.HasMany(o => o.Items)
                .WithOne(i => i.Order)
                .HasForeignKey(i => i.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(o => o.UserId);
            builder.HasIndex(o => o.Status);
            builder.HasIndex(o => o.ShippingAddressId);
        }
    }

    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItemEntity>
    {
        public void Configure(EntityTypeBuilder<OrderItemEntity> builder)
        {
            builder.ToTable("order_items");
            builder.HasKey(i => i.Id);

            builder.Property(i => i.OrderId)
                .IsRequired();

            builder.Property(i => i.ProductId)
                .IsRequired();

            builder.Property(i => i.VariantId)
                .IsRequired();

            builder.Property(i => i.Quantity)
                .IsRequired();

            builder.Property(i => i.Price)
                .HasPrecision(12, 2)
                .IsRequired();

            builder.HasOne(i => i.Order)
                .WithMany(o => o.Items)
                .HasForeignKey(i => i.OrderId);

            builder.HasIndex(i => i.OrderId);
            builder.HasIndex(i => i.ProductId);
        }
    }
}
