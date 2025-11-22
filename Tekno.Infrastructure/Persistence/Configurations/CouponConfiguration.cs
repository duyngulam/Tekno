using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using Tekno.Domain.Promotion;

namespace Tekno.Infrastructure.Persistence.Configurations
{
    public class CouponConfiguration : IEntityTypeConfiguration<Coupon>
    {
        public void Configure(EntityTypeBuilder<Coupon> builder)
        {
            builder.ToTable("coupons");
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Code)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(c => c.Code).IsUnique();

            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(c => c.Type)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.Property(c => c.Value)
                .HasPrecision(12, 2)
                .IsRequired();

            builder.Property(c => c.Quantity)
                .IsRequired();

            builder.Property(c => c.UsedCount)
                .HasDefaultValue(0);

            builder.Property(c => c.MaxUsagePerUser)
                .IsRequired(false);

            builder.Property(c => c.MinPurchaseAmount)
                .HasPrecision(12, 2)
                .IsRequired(false);

            builder.Property(c => c.MaxDiscountAmount)
                .HasPrecision(12, 2)
                .IsRequired(false);

            builder.Property(c => c.StartDate)
                .HasColumnType("timestamptz")
                .IsRequired();

            builder.Property(c => c.EndDate)
                .HasColumnType("timestamptz")
                .IsRequired();

            builder.Property(c => c.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20)
                .HasDefaultValue(CouponStatus.Active);

            builder.Property(c => c.Note)
                .HasMaxLength(500);

            builder.Property(c => c.CreatedAt)
                .HasColumnType("timestamptz")
                .HasDefaultValueSql("NOW()");

            builder.Property(c => c.UpdatedAt)
                .HasColumnType("timestamptz")
                .HasDefaultValueSql("NOW()");

            // Relationships
            builder.HasMany(c => c.ApplicableCategories)
                .WithOne(cc => cc.Coupon)
                .HasForeignKey(cc => cc.CouponId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(c => c.ApplicableProducts)
                .WithOne(cp => cp.Coupon)
                .HasForeignKey(cp => cp.CouponId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(c => c.Usages)
                .WithOne(u => u.Coupon)
                .HasForeignKey(u => u.CouponId)
                .OnDelete(DeleteBehavior.Cascade);

            // Seed data (matching your UI example)
            var seedTime = new DateTime(2025, 1, 15, 0, 0, 0, DateTimeKind.Utc);
            
            builder.HasData(
                new
                {
                    Id = 1,
                    Code = "PHVC000001",
                    Name = "Holiday",
                    Type = CouponType.FixedAmount,
                    Value = 300000m,
                    Quantity = 10,
                    UsedCount = 0,
                    StartDate = new DateTime(2023, 8, 23, 0, 0, 0, DateTimeKind.Utc),
                    EndDate = new DateTime(2022, 2, 23, 0, 0, 0, DateTimeKind.Utc),
                    Status = CouponStatus.Active,
                    CreatedAt = seedTime,
                    UpdatedAt = seedTime
                },
                new
                {
                    Id = 2,
                    Code = "PHVC000002",
                    Name = "Summer",
                    Type = CouponType.FixedAmount,
                    Value = 300000m,
                    Quantity = 10,
                    UsedCount = 0,
                    StartDate = new DateTime(2025, 8, 23, 0, 0, 0, DateTimeKind.Utc),
                    EndDate = new DateTime(2026, 2, 23, 0, 0, 0, DateTimeKind.Utc),
                    Status = CouponStatus.Active,
                    CreatedAt = seedTime,
                    UpdatedAt = seedTime
                },
                new
                {
                    Id = 3,
                    Code = "PHVC000003",
                    Name = "Return",
                    Type = CouponType.FixedAmount,
                    Value = 300000m,
                    Quantity = 10,
                    UsedCount = 0,
                    StartDate = new DateTime(2025, 8, 23, 0, 0, 0, DateTimeKind.Utc),
                    EndDate = new DateTime(2026, 2, 23, 0, 0, 0, DateTimeKind.Utc),
                    Status = CouponStatus.Active,
                    CreatedAt = seedTime,
                    UpdatedAt = seedTime
                }
            );
        }
    }

    public class CouponCategoryConfiguration : IEntityTypeConfiguration<CouponCategory>
    {
        public void Configure(EntityTypeBuilder<CouponCategory> builder)
        {
            builder.ToTable("coupon_categories");
            builder.HasKey(cc => new { cc.CouponId, cc.CategoryId });

            builder.HasOne(cc => cc.Coupon)
                .WithMany(c => c.ApplicableCategories)
                .HasForeignKey(cc => cc.CouponId);
        }
    }

    public class CouponProductConfiguration : IEntityTypeConfiguration<CouponProduct>
    {
        public void Configure(EntityTypeBuilder<CouponProduct> builder)
        {
            builder.ToTable("coupon_products");
            builder.HasKey(cp => new { cp.CouponId, cp.ProductId });

            builder.HasOne(cp => cp.Coupon)
                .WithMany(c => c.ApplicableProducts)
                .HasForeignKey(cp => cp.CouponId);
        }
    }

    public class CouponUsageConfiguration : IEntityTypeConfiguration<CouponUsage>
    {
        public void Configure(EntityTypeBuilder<CouponUsage> builder)
        {
            builder.ToTable("coupon_usages");
            builder.HasKey(u => u.Id);

            builder.Property(u => u.DiscountAmount)
                .HasPrecision(12, 2)
                .IsRequired();

            builder.Property(u => u.UsedAt)
                .HasColumnType("timestamptz")
                .HasDefaultValueSql("NOW()");

            builder.HasOne(u => u.Coupon)
                .WithMany(c => c.Usages)
                .HasForeignKey(u => u.CouponId);

            builder.HasIndex(u => new { u.CouponId, u.UserId });
        }
    }
}
