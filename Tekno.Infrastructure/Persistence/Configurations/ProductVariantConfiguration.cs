using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tekno.Domain.Catalog;

namespace Tekno.Infrastructure.Persistence.Configurations
{
    public class ProductVariantConfiguration : IEntityTypeConfiguration<ProductVariant>
    {
        private static readonly DateTime SeedTime = new DateTime(2025, 1, 15, 10, 0, 0, DateTimeKind.Utc);
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
                
            // ================= SEED PRODUCT VARIANTS (VND) =================
            builder.HasData(
                // ===== Dell XPS 13 Variants =====
                new { Id = 1, ProductId = 1, Sku = "XPS13-I5-8-512", Price = 25990000m, Stock = 15, Status = "available", CreatedAt = SeedTime },
                new { Id = 2, ProductId = 1, Sku = "XPS13-I7-16-1TB", Price = 32990000m, Stock = 8, Status = "available", CreatedAt = SeedTime },

                // ===== MacBook Air M2 Variants =====
                new { Id = 3, ProductId = 2, Sku = "MBA-M2-8-256", Price = 28990000m, Stock = 12, Status = "available", CreatedAt = SeedTime },
                new { Id = 4, ProductId = 2, Sku = "MBA-M2-16-512", Price = 36990000m, Stock = 7, Status = "available", CreatedAt = SeedTime },

                // ===== Asus ZenBook Variants =====
                new { Id = 5, ProductId = 3, Sku = "ZEN14-I5-8-512", Price = 22490000m, Stock = 20, Status = "available", CreatedAt = SeedTime },
                new { Id = 6, ProductId = 3, Sku = "ZEN14-I7-16-1TB", Price = 28990000m, Stock = 10, Status = "available", CreatedAt = SeedTime },

                // ===== HP Spectre x360 Variants =====
                new { Id = 7, ProductId = 4, Sku = "HPX360-I5-8-512", Price = 42990000m, Stock = 12, Status = "available", CreatedAt = SeedTime },
                new { Id = 8, ProductId = 4, Sku = "HPX360-I7-16-1TB", Price = 52990000m, Stock = 6, Status = "available", CreatedAt = SeedTime },

                // ===== ThinkPad X1 Carbon Variants =====
                new { Id = 9, ProductId = 5, Sku = "X1C11-I5-16-512", Price = 48990000m, Stock = 10, Status = "available", CreatedAt = SeedTime },
                new { Id = 10, ProductId = 5, Sku = "X1C11-I7-16-1TB", Price = 58990000m, Stock = 5, Status = "available", CreatedAt = SeedTime },

                // ===== iPhone 15 Pro Max Variants =====
                new { Id = 11, ProductId = 10, Sku = "IP15PM-256-NAT", Price = 33990000m, Stock = 25, Status = "available", CreatedAt = SeedTime },
                new { Id = 12, ProductId = 10, Sku = "IP15PM-512-NAT", Price = 39990000m, Stock = 15, Status = "available", CreatedAt = SeedTime },
                new { Id = 13, ProductId = 10, Sku = "IP15PM-256-BLK", Price = 33990000m, Stock = 20, Status = "available", CreatedAt = SeedTime },

                // ===== Samsung Galaxy S24 Ultra Variants =====
                new { Id = 14, ProductId = 11, Sku = "S24U-256-GRAY", Price = 29990000m, Stock = 30, Status = "available", CreatedAt = SeedTime },
                new { Id = 15, ProductId = 11, Sku = "S24U-512-GRAY", Price = 34990000m, Stock = 18, Status = "available", CreatedAt = SeedTime },
                new { Id = 16, ProductId = 11, Sku = "S24U-256-VIOL", Price = 29990000m, Stock = 25, Status = "available", CreatedAt = SeedTime },

                // ===== Google Pixel 8 Pro Variants =====
                new { Id = 17, ProductId = 12, Sku = "PIX8P-128-OBSI", Price = 24990000m, Stock = 20, Status = "available", CreatedAt = SeedTime },
                new { Id = 18, ProductId = 12, Sku = "PIX8P-256-OBSI", Price = 27990000m, Stock = 15, Status = "available", CreatedAt = SeedTime },
                new { Id = 19, ProductId = 12, Sku = "PIX8P-256-BAY", Price = 27990000m, Stock = 12, Status = "available", CreatedAt = SeedTime },

                // ===== Xiaomi 14 Variants =====
                new { Id = 20, ProductId = 13, Sku = "MI14-12-256-BLK", Price = 18990000m, Stock = 35, Status = "available", CreatedAt = SeedTime },
                new { Id = 21, ProductId = 13, Sku = "MI14-12-512-BLK", Price = 21990000m, Stock = 22, Status = "available", CreatedAt = SeedTime },

                // ===== OnePlus 12 Variants =====
                new { Id = 22, ProductId = 14, Sku = "OP12-12-256-BLK", Price = 19990000m, Stock = 28, Status = "available", CreatedAt = SeedTime },
                new { Id = 23, ProductId = 14, Sku = "OP12-16-512-GRN", Price = 23990000m, Stock = 18, Status = "available", CreatedAt = SeedTime },

                // ===== iPad Pro M2 Variants =====
                new { Id = 24, ProductId = 20, Sku = "IPADPRO11-128-SIL", Price = 24990000m, Stock = 18, Status = "available", CreatedAt = SeedTime },
                new { Id = 25, ProductId = 20, Sku = "IPADPRO11-256-SIL", Price = 28990000m, Stock = 12, Status = "available", CreatedAt = SeedTime },
                new { Id = 26, ProductId = 20, Sku = "IPADPRO11-256-SPC", Price = 28990000m, Stock = 10, Status = "available", CreatedAt = SeedTime },

                // ===== Galaxy Tab S9 Variants =====
                new { Id = 27, ProductId = 21, Sku = "TABS9-8-128-GRAY", Price = 19990000m, Stock = 25, Status = "available", CreatedAt = SeedTime },
                new { Id = 28, ProductId = 21, Sku = "TABS9-12-256-GRAY", Price = 24990000m, Stock = 15, Status = "available", CreatedAt = SeedTime },

                // ===== Xiaomi Pad 6 Variants =====
                new { Id = 29, ProductId = 22, Sku = "MIPAD6-6-128-GRAY", Price = 8990000m, Stock = 40, Status = "available", CreatedAt = SeedTime },
                new { Id = 30, ProductId = 22, Sku = "MIPAD6-8-256-BLUE", Price = 10990000m, Stock = 30, Status = "available", CreatedAt = SeedTime },

                // ===== Dell UltraSharp Monitor =====
                new { Id = 31, ProductId = 30, Sku = "U2723DE-27QHD", Price = 17990000m, Stock = 15, Status = "available", CreatedAt = SeedTime },

                // ===== LG UltraGear Monitor =====
                new { Id = 32, ProductId = 31, Sku = "27GN800-QHD144", Price = 8490000m, Stock = 22, Status = "available", CreatedAt = SeedTime },

                // ===== Logitech MX Keys Variants =====
                new { Id = 33, ProductId = 40, Sku = "MXKEYS-GRAY", Price = 2990000m, Stock = 35, Status = "available", CreatedAt = SeedTime },
                new { Id = 34, ProductId = 40, Sku = "MXKEYS-WHT", Price = 2990000m, Stock = 28, Status = "available", CreatedAt = SeedTime },

                // ===== Razer BlackWidow V4 Pro =====
                new { Id = 35, ProductId = 41, Sku = "BW-V4PRO-GRN", Price = 5990000m, Stock = 20, Status = "available", CreatedAt = SeedTime },

                // ===== Logitech MX Master 3S Variants =====
                new { Id = 36, ProductId = 50, Sku = "MXM3S-GRAPH", Price = 2490000m, Stock = 40, Status = "available", CreatedAt = SeedTime },
                new { Id = 37, ProductId = 50, Sku = "MXM3S-PALE", Price = 2490000m, Stock = 32, Status = "available", CreatedAt = SeedTime },

                // ===== Razer Viper V2 Pro Variants =====
                new { Id = 38, ProductId = 51, Sku = "VIPER-V2PRO-BLK", Price = 3990000m, Stock = 25, Status = "available", CreatedAt = SeedTime },
                new { Id = 39, ProductId = 51, Sku = "VIPER-V2PRO-WHT", Price = 3990000m, Stock = 20, Status = "available", CreatedAt = SeedTime },

                // ===== Sony WH-1000XM5 Variants =====
                new { Id = 40, ProductId = 60, Sku = "XM5-BLK", Price = 8990000m, Stock = 22, Status = "available", CreatedAt = SeedTime },
                new { Id = 41, ProductId = 60, Sku = "XM5-SLV", Price = 8990000m, Stock = 18, Status = "available", CreatedAt = SeedTime },

                // ===== AirPods Pro 2 =====
                new { Id = 42, ProductId = 61, Sku = "AIRPODSPRO2-USBC", Price = 6490000m, Stock = 35, Status = "available", CreatedAt = SeedTime },

                // ===== Anker 747 Charger =====
                new { Id = 43, ProductId = 70, Sku = "ANKER747-150W", Price = 2490000m, Stock = 45, Status = "available", CreatedAt = SeedTime },

                // ===== Baseus Cable Variants =====
                new { Id = 44, ProductId = 71, Sku = "BASEUS-C2C-2M-BLK", Price = 290000m, Stock = 120, Status = "available", CreatedAt = SeedTime },
                new { Id = 45, ProductId = 71, Sku = "BASEUS-C2C-2M-WHT", Price = 290000m, Stock = 100, Status = "available", CreatedAt = SeedTime },

                // ===== Spigen Cases Variants =====
                new { Id = 46, ProductId = 80, Sku = "SPIGEN-IP15P-BLK", Price = 490000m, Stock = 80, Status = "available", CreatedAt = SeedTime },
                new { Id = 47, ProductId = 80, Sku = "SPIGEN-IP15P-CLEAR", Price = 490000m, Stock = 70, Status = "available", CreatedAt = SeedTime },

                // ===== Tomtoc Sleeve Variants =====
                new { Id = 48, ProductId = 81, Sku = "TOMTOC-13-BLK", Price = 790000m, Stock = 50, Status = "available", CreatedAt = SeedTime },
                new { Id = 49, ProductId = 81, Sku = "TOMTOC-14-BLK", Price = 790000m, Stock = 45, Status = "available", CreatedAt = SeedTime }
            );
        }
    }
}
