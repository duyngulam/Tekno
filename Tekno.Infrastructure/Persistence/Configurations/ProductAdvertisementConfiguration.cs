using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using Tekno.Domain.Catalog;

namespace Tekno.Infrastructure.Persistence.Configurations
{
    public class ProductAdvertisementConfiguration : IEntityTypeConfiguration<ProductAdvertisement>
    {
        private static readonly DateTime SeedTime = new DateTime(2025, 1, 15, 10, 0, 0, DateTimeKind.Utc);
        
        public void Configure(EntityTypeBuilder<ProductAdvertisement> builder)
        {
            builder.ToTable("product_advertisements");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.ProductId)
                .IsRequired();

            builder.Property(a => a.ImageUrl)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(a => a.Position)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValue("HomeTop");

            builder.Property(a => a.Priority)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(a => a.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(a => a.StartDate)
                .HasColumnType("timestamptz");

            builder.Property(a => a.EndDate)
                .HasColumnType("timestamptz");

            builder.Property(a => a.CreatedAt)
                .HasColumnType("timestamptz")
                .HasDefaultValueSql("NOW()");

            builder.HasOne(a => a.Product)
                .WithMany()
                .HasForeignKey(a => a.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(a => a.Position);
            builder.HasIndex(a => a.IsActive);
            builder.HasIndex(a => new { a.IsActive, a.Position, a.Priority });

            // ========== SEED PRODUCT ADVERTISEMENT DATA ==========
            builder.HasData(
                // ===== HomeTop Position (Hero Banners) =====
                new
                {
                    Id = 1,
                    ProductId = 10, // iPhone 15 Pro Max
                    ImageUrl = "https://www.gstatic.com/webp/gallery/1.jpg",
                    Position = "HomeTop",
                    Priority = 100,
                    IsActive = true,
                    StartDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    EndDate = new DateTime(2025, 3, 31, 23, 59, 59, DateTimeKind.Utc),
                    CreatedAt = SeedTime
                },
                new
                {
                    Id = 2,
                    ProductId = 11, // Samsung Galaxy S24 Ultra
                    ImageUrl = "https://www.gstatic.com/webp/gallery/2.jpg",
                    Position = "HomeTop",
                    Priority = 90,
                    IsActive = true,
                    StartDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    EndDate = new DateTime(2025, 3, 31, 23, 59, 59, DateTimeKind.Utc),
                    CreatedAt = SeedTime
                },
                new
                {
                    Id = 3,
                    ProductId = 2, // MacBook Air M2
                    ImageUrl = "https://www.gstatic.com/webp/gallery/3.jpg",
                    Position = "HomeTop",
                    Priority = 85,
                    IsActive = true,
                    StartDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    EndDate = new DateTime(2025, 6, 30, 23, 59, 59, DateTimeKind.Utc),
                    CreatedAt = SeedTime
                },

                // ===== HomeMiddle Position =====
                new
                {
                    Id = 4,
                    ProductId = 20, // iPad Pro M2
                    ImageUrl = "https://www.gstatic.com/webp/gallery/4.jpg",
                    Position = "HomeMiddle",
                    Priority = 80,
                    IsActive = true,
                    StartDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    EndDate = new DateTime(2025, 4, 30, 23, 59, 59, DateTimeKind.Utc),
                    CreatedAt = SeedTime
                },
                new
                {
                    Id = 5,
                    ProductId = 21, // Galaxy Tab S9
                    ImageUrl = "https://www.gstatic.com/webp/gallery/5.jpg",
                    Position = "HomeMiddle",
                    Priority = 75,
                    IsActive = true,
                    StartDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    EndDate = new DateTime(2025, 4, 30, 23, 59, 59, DateTimeKind.Utc),
                    CreatedAt = SeedTime
                },

                // ===== HomeBottom Position =====
                new
                {
                    Id = 6,
                    ProductId = 60, // Sony WH-1000XM5
                    ImageUrl = "https://www.gstatic.com/webp/gallery/1.jpg",
                    Position = "HomeBottom",
                    Priority = 70,
                    IsActive = true,
                    StartDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    EndDate = new DateTime(2025, 12, 31, 23, 59, 59, DateTimeKind.Utc),
                    CreatedAt = SeedTime
                },
                new
                {
                    Id = 7,
                    ProductId = 61, // AirPods Pro 2
                    ImageUrl = "https://www.gstatic.com/webp/gallery/2.jpg",
                    Position = "HomeBottom",
                    Priority = 65,
                    IsActive = true,
                    StartDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    EndDate = new DateTime(2025, 12, 31, 23, 59, 59, DateTimeKind.Utc),
                    CreatedAt = SeedTime
                },

                // ===== CategoryTop Position (Laptop Category) =====
                new
                {
                    Id = 8,
                    ProductId = 1, // Dell XPS 13
                    ImageUrl = "https://www.gstatic.com/webp/gallery/3.jpg",
                    Position = "CategoryTop",
                    Priority = 90,
                    IsActive = true,
                    StartDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    EndDate = new DateTime(2025, 6, 30, 23, 59, 59, DateTimeKind.Utc),
                    CreatedAt = SeedTime
                },
                new
                {
                    Id = 9,
                    ProductId = 3, // Asus ZenBook 14
                    ImageUrl = "https://www.gstatic.com/webp/gallery/4.jpg",
                    Position = "CategoryTop",
                    Priority = 85,
                    IsActive = true,
                    StartDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    EndDate = new DateTime(2025, 6, 30, 23, 59, 59, DateTimeKind.Utc),
                    CreatedAt = SeedTime
                },

                // ===== CategoryMiddle Position =====
                new
                {
                    Id = 10,
                    ProductId = 30, // Dell UltraSharp Monitor
                    ImageUrl = "https://www.gstatic.com/webp/gallery/5.jpg",
                    Position = "CategoryMiddle",
                    Priority = 80,
                    IsActive = true,
                    StartDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    EndDate = new DateTime(2025, 12, 31, 23, 59, 59, DateTimeKind.Utc),
                    CreatedAt = SeedTime
                },
                new
                {
                    Id = 11,
                    ProductId = 31, // LG UltraGear Gaming Monitor
                    ImageUrl = "https://www.gstatic.com/webp/gallery/1.jpg",
                    Position = "CategoryMiddle",
                    Priority = 75,
                    IsActive = true,
                    StartDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    EndDate = new DateTime(2025, 12, 31, 23, 59, 59, DateTimeKind.Utc),
                    CreatedAt = SeedTime
                },

                // ===== ProductSidebar Position =====
                new
                {
                    Id = 12,
                    ProductId = 40, // Logitech MX Keys
                    ImageUrl = "https://www.gstatic.com/webp/gallery/2.jpg",
                    Position = "ProductSidebar",
                    Priority = 70,
                    IsActive = true,
                    StartDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    EndDate = new DateTime(2025, 12, 31, 23, 59, 59, DateTimeKind.Utc),
                    CreatedAt = SeedTime
                },
                new
                {
                    Id = 13,
                    ProductId = 50, // Logitech MX Master 3S
                    ImageUrl = "https://www.gstatic.com/webp/gallery/3.jpg",
                    Position = "ProductSidebar",
                    Priority = 65,
                    IsActive = true,
                    StartDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    EndDate = new DateTime(2025, 12, 31, 23, 59, 59, DateTimeKind.Utc),
                    CreatedAt = SeedTime
                },
                new
                {
                    Id = 14,
                    ProductId = 70, // Anker 747 Charger
                    ImageUrl = "https://www.gstatic.com/webp/gallery/4.jpg",
                    Position = "ProductSidebar",
                    Priority = 60,
                    IsActive = true,
                    StartDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    EndDate = new DateTime(2025, 12, 31, 23, 59, 59, DateTimeKind.Utc),
                    CreatedAt = SeedTime
                },

                // ===== SearchTop Position =====
                new
                {
                    Id = 15,
                    ProductId = 13, // Xiaomi 14
                    ImageUrl = "https://www.gstatic.com/webp/gallery/5.jpg",
                    Position = "SearchTop",
                    Priority = 85,
                    IsActive = true,
                    StartDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    EndDate = new DateTime(2025, 3, 31, 23, 59, 59, DateTimeKind.Utc),
                    CreatedAt = SeedTime
                },
                new
                {
                    Id = 16,
                    ProductId = 14, // OnePlus 12
                    ImageUrl = "https://www.gstatic.com/webp/gallery/1.jpg",
                    Position = "SearchTop",
                    Priority = 80,
                    IsActive = true,
                    StartDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    EndDate = new DateTime(2025, 3, 31, 23, 59, 59, DateTimeKind.Utc),
                    CreatedAt = SeedTime
                },

                // ===== Expired/Inactive Ads (for testing) =====
                new
                {
                    Id = 17,
                    ProductId = 12, // Google Pixel 8 Pro
                    ImageUrl = "https://www.gstatic.com/webp/gallery/2.jpg",
                    Position = "HomeTop",
                    Priority = 95,
                    IsActive = false, // Deactivated
                    StartDate = new DateTime(2024, 12, 1, 0, 0, 0, DateTimeKind.Utc),
                    EndDate = new DateTime(2024, 12, 31, 23, 59, 59, DateTimeKind.Utc),
                    CreatedAt = SeedTime
                },
                new
                {
                    Id = 18,
                    ProductId = 4, // HP Spectre x360
                    ImageUrl = "https://www.gstatic.com/webp/gallery/3.jpg",
                    Position = "CategoryTop",
                    Priority = 88,
                    IsActive = true,
                    StartDate = new DateTime(2024, 11, 1, 0, 0, 0, DateTimeKind.Utc),
                    EndDate = new DateTime(2024, 12, 31, 23, 59, 59, DateTimeKind.Utc), // Expired
                    CreatedAt = SeedTime
                }
            );
        }
    }
}
