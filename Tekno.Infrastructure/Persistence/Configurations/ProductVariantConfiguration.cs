using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tekno.Domain.Catalog;

namespace Tekno.Infrastructure.Persistence.Configurations
{
    public class ProductVariantConfiguration : IEntityTypeConfiguration<ProductVariant>
    {
        public void Configure(EntityTypeBuilder<ProductVariant> builder)
        {
            builder.ToTable("product_variant");
            builder.HasKey(v => v.Id);

            builder.Property(v => v.Sku)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(v => v.Sku).IsUnique();

            builder.Property(v => v.Price)
                .HasPrecision(12, 2)
                .IsRequired();

            builder.Property(v => v.Stock).HasDefaultValue(0);
            builder.Property(v => v.Status).HasDefaultValue("available");
            builder.Property(v => v.VariantSpecsJson).HasColumnType("jsonb");

            builder.Property(v => v.CreatedAt)
                .HasColumnType("timestamptz")
                .HasDefaultValueSql("NOW()");


            builder.HasOne(v => v.Product)
               .WithMany(p => p.Variants)
               .HasForeignKey(v => v.ProductId)
               .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(v => v.VariantAttributes)
                .WithOne(va => va.Variant)
                .HasForeignKey(va => va.VariantId);
            // ================= SEED DỮ LIỆU VARIANT =================
            builder.HasData(
                // ===== Laptops =====
                new { Id = 1, ProductId = 1, Sku = "XPS13-I5-8-512", Price = 1099.00m, Stock = 20, Status = "available" },
                new { Id = 2, ProductId = 1, Sku = "XPS13-I7-16-1TB", Price = 1499.00m, Stock = 10, Status = "available" },

                new { Id = 3, ProductId = 2, Sku = "MBA-M2-8-256", Price = 999.00m, Stock = 15, Status = "available" },
                new { Id = 4, ProductId = 2, Sku = "MBA-M2-16-512", Price = 1299.00m, Stock = 8, Status = "available" },

                new { Id = 5, ProductId = 3, Sku = "ZEN-I5-8-512", Price = 899.00m, Stock = 25, Status = "available" },
                new { Id = 6, ProductId = 3, Sku = "ZEN-I7-16-1TB", Price = 1199.00m, Stock = 12, Status = "available" },

                new { Id = 7, ProductId = 4, Sku = "HPX360-I5-8-512", Price = 1099.00m, Stock = 18, Status = "available" },
                new { Id = 8, ProductId = 4, Sku = "HPX360-I7-16-1TB", Price = 1399.00m, Stock = 10, Status = "available" },

                new { Id = 9, ProductId = 5, Sku = "X1-I5-8-512", Price = 1199.00m, Stock = 14, Status = "available" },
                new { Id = 10, ProductId = 5, Sku = "X1-I7-16-1TB", Price = 1499.00m, Stock = 8, Status = "available" },

                // ===== Smartphones =====
                new { Id = 11, ProductId = 10, Sku = "IP15P-128", Price = 1199.00m, Stock = 20, Status = "available" },
                new { Id = 12, ProductId = 10, Sku = "IP15P-256", Price = 1299.00m, Stock = 15, Status = "available" },

                new { Id = 13, ProductId = 11, Sku = "S24-128", Price = 999.00m, Stock = 25, Status = "available" },
                new { Id = 14, ProductId = 11, Sku = "S24-256", Price = 1099.00m, Stock = 20, Status = "available" },

                new { Id = 15, ProductId = 12, Sku = "PIX9-128", Price = 899.00m, Stock = 18, Status = "available" },
                new { Id = 16, ProductId = 12, Sku = "PIX9-256", Price = 999.00m, Stock = 12, Status = "available" },

                // ===== Tablets =====
                new { Id = 17, ProductId = 20, Sku = "IPADPRO-128", Price = 1099.00m, Stock = 20, Status = "available" },
                new { Id = 18, ProductId = 20, Sku = "IPADPRO-256", Price = 1299.00m, Stock = 15, Status = "available" },

                new { Id = 19, ProductId = 21, Sku = "TABS9-128", Price = 899.00m, Stock = 25, Status = "available" },
                new { Id = 20, ProductId = 21, Sku = "TABS9-256", Price = 999.00m, Stock = 15, Status = "available" },

                // ===== Monitors =====
                new { Id = 21, ProductId = 30, Sku = "DELL-U2720Q", Price = 499.00m, Stock = 12, Status = "available" },
                new { Id = 22, ProductId = 31, Sku = "LG-UG32", Price = 599.00m, Stock = 10, Status = "available" },

                // ===== Keyboards =====
                new { Id = 23, ProductId = 40, Sku = "MX-KEYS-GRAY", Price = 119.00m, Stock = 30, Status = "available" },
                new { Id = 24, ProductId = 41, Sku = "RZR-BW-V4", Price = 149.00m, Stock = 25, Status = "available" },

                // ===== Mice =====
                new { Id = 25, ProductId = 50, Sku = "MXM3S-GRAPHITE", Price = 99.00m, Stock = 35, Status = "available" },
                new { Id = 26, ProductId = 51, Sku = "RZR-V2PRO", Price = 129.00m, Stock = 28, Status = "available" },

                // ===== Headphones =====
                new { Id = 27, ProductId = 60, Sku = "SONY-XM5-BLK", Price = 349.00m, Stock = 18, Status = "available" },
                new { Id = 28, ProductId = 61, Sku = "AIRPODS-PRO2", Price = 249.00m, Stock = 25, Status = "available" },

                // ===== Chargers & Cables =====
                new { Id = 29, ProductId = 70, Sku = "ANKER-65W", Price = 59.00m, Stock = 40, Status = "available" },
                new { Id = 30, ProductId = 71, Sku = "BASEUS-CABLE-15", Price = 19.00m, Stock = 100, Status = "available" },

                // ===== Cases =====
                new { Id = 31, ProductId = 80, Sku = "SPIGEN-RUGGED", Price = 29.00m, Stock = 50, Status = "available" },
                new { Id = 32, ProductId = 81, Sku = "UAG-SLEEVE", Price = 49.00m, Stock = 35, Status = "available" }
            );
        }
    }
}
