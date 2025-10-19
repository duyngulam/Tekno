using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tekno.Domain.Catalog;

namespace Tekno.Infrastructure.Persistence.Configurations
{
    public class BrandConfiguration : IEntityTypeConfiguration<Brand>
    {
        // Sử dụng thời gian tĩnh đã thống nhất để tránh lỗi Non-Deterministic Model
        private static readonly DateTime SeedTime = new DateTime(2025, 10, 12, 9, 34, 0, DateTimeKind.Utc);

        public void Configure(EntityTypeBuilder<Brand> builder)
        {
            builder.ToTable("brand");
            builder.HasKey(b => b.Id);

            builder.Property(b => b.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(b => b.Slug)
                .IsRequired()
                .HasMaxLength(120);

            builder.HasIndex(b => b.Slug)
                .IsUnique();

            builder.Property(b => b.Country)
                .HasMaxLength(50);

            builder.Property(b => b.LogoPath)
                .HasMaxLength(255);

            builder.Property(b => b.CreatedAt)
                .HasColumnType("timestamptz").HasDefaultValueSql("NOW()");

            builder.Property(b => b.UpdatedAt)
                .HasColumnType("timestamptz").HasDefaultValueSql("NOW()");

            builder.HasMany(b => b.Products)
                .WithOne(p => p.Brand)
                .HasForeignKey(p => p.BrandId);

            builder.HasData(
                // BRANDS ĐÃ CÓ (1-8)
                new { Id = 1, Name = "Dell", Slug = "dell", Country = "USA", LogoPath = "https://cdn.worldvectorlogo.com/logos/dell-2.svg", CreatedAt = SeedTime, UpdatedAt = SeedTime },
                new { Id = 2, Name = "Apple", Slug = "apple", Country = "USA", LogoPath = "https://cdn.worldvectorlogo.com/logos/apple-13.svg", CreatedAt = SeedTime, UpdatedAt = SeedTime },
                new { Id = 3, Name = "Asus", Slug = "asus", Country = "Taiwan", LogoPath = "https://cdn.worldvectorlogo.com/logos/asus-4.svg", CreatedAt = SeedTime, UpdatedAt = SeedTime },
                new { Id = 4, Name = "HP", Slug = "hp", Country = "USA", LogoPath = "https://cdn.worldvectorlogo.com/logos/HP-5.svg", CreatedAt = SeedTime, UpdatedAt = SeedTime },
                new { Id = 5, Name = "Lenovo", Slug = "lenovo", Country = "China", LogoPath = "https://cdn.worldvectorlogo.com/logos/lenovo-2.svg", CreatedAt = SeedTime, UpdatedAt = SeedTime },
                new { Id = 6, Name = "Samsung", Slug = "samsung", Country = "Korea", LogoPath = "https://cdn.worldvectorlogo.com/logos/samsung-8.svg", CreatedAt = SeedTime, UpdatedAt = SeedTime },
                new { Id = 7, Name = "Google", Slug = "google", Country = "USA", LogoPath = "https://cdn.worldvectorlogo.com/logos/google-1.svg", CreatedAt = SeedTime, UpdatedAt = SeedTime },
                new { Id = 8, Name = "Xiaomi", Slug = "xiaomi", Country = "China", LogoPath = "https://cdn.worldvectorlogo.com/logos/xiaomi-1.svg", CreatedAt = SeedTime, UpdatedAt = SeedTime },

                // BRANDS ĐÃ BỔ SUNG (9-17)
                new { Id = 9, Name = "OnePlus", Slug = "oneplus", Country = "China", LogoPath = "https://cdn.worldvectorlogo.com/logos/oneplus-2.svg", CreatedAt = SeedTime, UpdatedAt = SeedTime },
                new { Id = 10, Name = "LG", Slug = "lg", Country = "Korea", LogoPath = "https://cdn.worldvectorlogo.com/logos/lg.svg", CreatedAt = SeedTime, UpdatedAt = SeedTime },
                new { Id = 11, Name = "Logitech", Slug = "logitech", Country = "Switzerland", LogoPath = "https://cdn.worldvectorlogo.com/logos/logitech-gaming-2.svg", CreatedAt = SeedTime, UpdatedAt = SeedTime },
                new { Id = 12, Name = "Razer", Slug = "razer", Country = "USA", LogoPath = "https://cdn.worldvectorlogo.com/logos/razer-1.svg", CreatedAt = SeedTime, UpdatedAt = SeedTime },
                new { Id = 13, Name = "Sony", Slug = "sony", Country = "Japan", LogoPath = "https://cdn.worldvectorlogo.com/logos/sony-2svg", CreatedAt = SeedTime, UpdatedAt = SeedTime },
                new { Id = 14, Name = "Anker", Slug = "anker", Country = "China", LogoPath = "https://cdn.worldvectorlogo.com/logos/anker-logo-1.svg", CreatedAt = SeedTime, UpdatedAt = SeedTime },
                new { Id = 15, Name = "Baseus", Slug = "baseus", Country = "China", LogoPath = "https://mms.img.susercontent.com/vn-11134216-7r98o-lnicyi57m5x6fd", CreatedAt = SeedTime, UpdatedAt = SeedTime },
                new { Id = 16, Name = "Spigen", Slug = "spigen", Country = "USA", LogoPath = "https://spigen.vn/wp-content/uploads/2023/09/Spigen_Header_New_Logo.png", CreatedAt = SeedTime, UpdatedAt = SeedTime },
                new { Id = 17, Name = "UAG", Slug = "uag", Country = "USA", LogoPath = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRlpSaYkZMxWktmvmvOx7mDurTEDu0KXqz1HQ&s", CreatedAt = SeedTime, UpdatedAt = SeedTime }
            );
        }
    }
}
