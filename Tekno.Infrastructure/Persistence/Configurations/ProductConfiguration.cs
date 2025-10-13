using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tekno.Domain.Catalog;

namespace Tekno.Infrastructure.Persistence.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        private static readonly DateTime SeedTime = new DateTime(2025, 10, 12, 9, 34, 0, DateTimeKind.Utc);
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("product");
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Name).IsRequired().HasMaxLength(150);
            builder.Property(p => p.BasePrice).IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(p => p.Status).IsRequired().HasMaxLength(50).HasDefaultValue("available");
            builder.Property(p => p.Slug).IsRequired().HasMaxLength(200);
            builder.HasIndex(p => p.Slug).IsUnique();

            builder.Property(p => p.CreatedAt).HasColumnType("timestamptz").HasDefaultValueSql("NOW()");
            builder.Property(p => p.UpdatedAt).HasColumnType("timestamptz").HasDefaultValueSql("NOW()");
            builder.Property(p => p.Description).HasColumnType("text");
            builder.Property(p => p.Overview).HasColumnType("text");

            builder.HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId);

            builder.HasOne(p => p.Brand)
                .WithMany(b => b.Products)
                .HasForeignKey(p => p.BrandId);

            //// ========== SEED PRODUCT DATA ==========
            builder.HasData(
                // ===== Laptops =====
                new { Id = 1, Name = "Dell XPS 13", Slug = "dell-xps-13", BrandId = 1, CategoryId = 1, BasePrice = 1699.00M, Status = "available", Overview = "Premium ultrabook with compact design.", Description = "Dell XPS 13 series laptops designed for professionals.", CreatedAt = SeedTime, UpdatedAt = SeedTime },
                new { Id = 2, Name = "MacBook Air", Slug = "macbook-air", BrandId = 2, CategoryId = 1, BasePrice = 1199.00M, Status = "available", Overview = "Ultra-thin and lightweight laptop by Apple.", Description = "MacBook Air powered by Apple M-series chips.", CreatedAt = SeedTime, UpdatedAt = SeedTime },
                new { Id = 3, Name = "Asus ZenBook", Slug = "asus-zenbook", BrandId = 3, CategoryId = 1, BasePrice = 1450.00M, Status = "available", Overview = "Portable productivity ultrabook.", Description = "ZenBook series with Intel and AMD variants.", CreatedAt = SeedTime, UpdatedAt = SeedTime },
                new { Id = 4, Name = "HP Spectre x360", Slug = "hp-spectre-x360", BrandId = 4, CategoryId = 1, BasePrice = 1799.00M, Status = "available", Overview = "Convertible premium laptop.", Description = "2-in-1 design with touch and pen support.", CreatedAt = SeedTime, UpdatedAt = SeedTime },
                new { Id = 5, Name = "Lenovo ThinkPad X1 Carbon", Slug = "thinkpad-x1-carbon", BrandId = 5, CategoryId = 1, BasePrice = 1999.00M, Status = "available", Overview = "Business ultrabook with robust build.", Description = "ThinkPad X1 Carbon for professionals.", CreatedAt = SeedTime, UpdatedAt = SeedTime },

                // ===== Smartphones =====
                new { Id = 10, Name = "iPhone 15 Pro", Slug = "iphone-15-pro", BrandId = 2, CategoryId = 2, BasePrice = 999.00M, Status = "available", Overview = "Apple flagship smartphone.", Description = "iPhone 15 Pro with titanium frame and A17 Pro chip.", CreatedAt = SeedTime, UpdatedAt = SeedTime },
                new { Id = 11, Name = "Samsung Galaxy S24", Slug = "galaxy-s24", BrandId = 6, CategoryId = 2, BasePrice = 899.00M, Status = "available", Overview = "Next-gen Android flagship.", Description = "Galaxy S24 with AMOLED display and powerful camera.", CreatedAt = SeedTime, UpdatedAt = SeedTime },
                new { Id = 12, Name = "Google Pixel 9", Slug = "pixel-9", BrandId = 7, CategoryId = 2, BasePrice = 799.00M, Status = "available", Overview = "Pure Android experience.", Description = "Pixel 9 with Tensor G4 chip and AI photography.", CreatedAt = SeedTime, UpdatedAt = SeedTime },
                new { Id = 13, Name = "Xiaomi 14 Pro", Slug = "xiaomi-14-pro", BrandId = 8, CategoryId = 2, BasePrice = 850.00M, Status = "available", Overview = "High-end performance smartphone.", Description = "Xiaomi 14 Pro with Leica cameras.", CreatedAt = SeedTime, UpdatedAt = SeedTime },
                new { Id = 14, Name = "OnePlus 12", Slug = "oneplus-12", BrandId = 9, CategoryId = 2, BasePrice = 749.00M, Status = "available", Overview = "Performance-focused smartphone.", Description = "OnePlus 12 offers fast charging and high refresh rate.", CreatedAt = SeedTime, UpdatedAt = SeedTime },

                // ===== Tablets =====
                new { Id = 20, Name = "iPad Pro", Slug = "ipad-pro", BrandId = 2, CategoryId = 3, BasePrice = 899.00M, Status = "available", Overview = "Powerful tablet for creators.", Description = "iPad Pro with M2 chip and ProMotion display.", CreatedAt = SeedTime, UpdatedAt = SeedTime },
                new { Id = 21, Name = "Samsung Galaxy Tab S9", Slug = "galaxy-tab-s9", BrandId = 6, CategoryId = 3, BasePrice = 799.00M, Status = "available", Overview = "Android flagship tablet.", Description = "Galaxy Tab S9 series with AMOLED display.", CreatedAt = SeedTime, UpdatedAt = SeedTime },
                new { Id = 22, Name = "Lenovo Tab P12", Slug = "lenovo-tab-p12", BrandId = 5, CategoryId = 3, BasePrice = 419.00M, Status = "available", Overview = "Affordable productivity tablet.", Description = "Tab P12 supports stylus and multi-tasking.", CreatedAt = SeedTime, UpdatedAt = SeedTime },

                // ===== Monitors =====
                new { Id = 30, Name = "Dell UltraSharp 27", Slug = "dell-ultrasharp-27", BrandId = 1, CategoryId = 8, BasePrice = 699.00M, Status = "available", Overview = "Professional 4K monitor.", Description = "Color-accurate UltraSharp series for designers.", CreatedAt = SeedTime, UpdatedAt = SeedTime },
                new { Id = 31, Name = "LG Ultragear 32", Slug = "lg-ultragear-32", BrandId = 10, CategoryId = 8, BasePrice = 450.00M, Status = "available", Overview = "Gaming monitor with 165Hz refresh rate.", Description = "High-performance display for gamers.", CreatedAt = SeedTime, UpdatedAt = SeedTime },

                // ===== Keyboards =====
                new { Id = 40, Name = "Logitech MX Keys", Slug = "logitech-mx-keys", BrandId = 11, CategoryId = 13, BasePrice = 119.99M, Status = "available", Overview = "Wireless keyboard for professionals.", Description = "Backlit keyboard with multi-device support.", CreatedAt = SeedTime, UpdatedAt = SeedTime },
                new { Id = 41, Name = "Razer BlackWidow V4", Slug = "razer-blackwidow-v4", BrandId = 12, CategoryId = 13, BasePrice = 169.99M, Status = "available", Overview = "Mechanical gaming keyboard.", Description = "RGB lighting and tactile feedback.", CreatedAt = SeedTime, UpdatedAt = SeedTime },

                // ===== Mouse =====
                new { Id = 50, Name = "Logitech MX Master 3S", Slug = "logitech-mx-master-3s", BrandId = 11, CategoryId = 14, BasePrice = 99.99M, Status = "available", Overview = "Ergonomic productivity mouse.", Description = "Supports multiple devices with customizable buttons.", CreatedAt = SeedTime, UpdatedAt = SeedTime },
                new { Id = 51, Name = "Razer Viper V2 Pro", Slug = "razer-viper-v2-pro", BrandId = 12, CategoryId = 14, BasePrice = 149.99M, Status = "available", Overview = "Ultra-light gaming mouse.", Description = "Wireless mouse with high precision sensor.", CreatedAt = SeedTime, UpdatedAt = SeedTime },

                // ===== Headphones =====
                new { Id = 60, Name = "Sony WH-1000XM5", Slug = "sony-wh-1000xm5", BrandId = 13, CategoryId = 15, BasePrice = 399.99M, Status = "available", Overview = "Noise-cancelling wireless headphones.", Description = "Industry-leading noise cancellation for audio lovers.", CreatedAt = SeedTime, UpdatedAt = SeedTime },
                new { Id = 61, Name = "Apple AirPods Pro 2", Slug = "airpods-pro-2", BrandId = 2, CategoryId = 15, BasePrice = 249.00M, Status = "available", Overview = "Wireless earbuds with active noise cancellation.", Description = "Compact design and adaptive sound control.", CreatedAt = SeedTime, UpdatedAt = SeedTime },

                // ===== Chargers & Cables =====
                new { Id = 70, Name = "Anker 65W GaN Charger", Slug = "anker-65w-gan", BrandId = 14, CategoryId = 16, BasePrice = 49.99M, Status = "available", Overview = "Fast charger with GaN technology.", Description = "Compact USB-C charger for laptops and phones.", CreatedAt = SeedTime, UpdatedAt = SeedTime },
                new { Id = 71, Name = "Baseus USB-C Cable 1.5m", Slug = "baseus-usb-c-cable", BrandId = 15, CategoryId = 16, BasePrice = 14.99M, Status = "available", Overview = "Durable braided charging cable.", Description = "Supports fast charging and data transfer.", CreatedAt = SeedTime, UpdatedAt = SeedTime },

                // ===== Cases & Covers =====
                new { Id = 80, Name = "Spigen Rugged Armor Case", Slug = "spigen-rugged-armor", BrandId = 16, CategoryId = 17, BasePrice = 29.99M, Status = "available", Overview = "Protective phone case.", Description = "Shock-absorbing TPU material.", CreatedAt = SeedTime, UpdatedAt = SeedTime },
                new { Id = 81, Name = "UAG Plasma Laptop Sleeve", Slug = "uag-laptop-sleeve", BrandId = 17, CategoryId = 17, BasePrice = 49.99M, Status = "available", Overview = "Durable protective sleeve.", Description = "Designed for 13–15 inch laptops.", CreatedAt = SeedTime, UpdatedAt = SeedTime }
            );
        }
    }
}
