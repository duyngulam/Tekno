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

            builder.Property(b => b.LogoUrl)
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
                new { Id = 1, Name = "Dell", Slug = "dell", Country = "USA", LogoUrl = "https://worldvectorlogo.com/logo/dell-2", CreatedAt = SeedTime, UpdatedAt = SeedTime },
                new { Id = 2, Name = "Apple", Slug = "apple", Country = "USA", LogoUrl = "https://worldvectorlogo.com/logo/apple-13", CreatedAt = SeedTime, UpdatedAt = SeedTime },
                new { Id = 3, Name = "Asus", Slug = "asus", Country = "Taiwan", LogoUrl = "https://worldvectorlogo.com/logo/asus-4", CreatedAt = SeedTime, UpdatedAt = SeedTime },
                new { Id = 4, Name = "HP", Slug = "hp", Country = "USA", LogoUrl = "https://worldvectorlogo.com/logo/HP-5", CreatedAt = SeedTime, UpdatedAt = SeedTime },
                new { Id = 5, Name = "Lenovo", Slug = "lenovo", Country = "China", LogoUrl = "https://worldvectorlogo.com/logo/lenovo-2", CreatedAt = SeedTime, UpdatedAt = SeedTime },
                new { Id = 6, Name = "Samsung", Slug = "samsung", Country = "Korea", LogoUrl = "https://worldvectorlogo.com/logo/samsung-8", CreatedAt = SeedTime, UpdatedAt = SeedTime },
                new { Id = 7, Name = "Google", Slug = "google", Country = "USA", LogoUrl = "https://worldvectorlogo.com/logo/google-1", CreatedAt = SeedTime, UpdatedAt = SeedTime },
                new { Id = 8, Name = "Xiaomi", Slug = "xiaomi", Country = "China", LogoUrl = "https://worldvectorlogo.com/logo/xiaomi-5", CreatedAt = SeedTime, UpdatedAt = SeedTime },

                // BRANDS ĐÃ BỔ SUNG (9-17)
                new { Id = 9, Name = "OnePlus", Slug = "oneplus", Country = "China", LogoUrl = "https://worldvectorlogo.com/logo/oneplus-2", CreatedAt = SeedTime, UpdatedAt = SeedTime },
                new { Id = 10, Name = "LG", Slug = "lg", Country = "Korea", LogoUrl = "https://worldvectorlogo.com/logo/lg", CreatedAt = SeedTime, UpdatedAt = SeedTime },
                new { Id = 11, Name = "Logitech", Slug = "logitech", Country = "Switzerland", LogoUrl = "https://worldvectorlogo.com/logo/logitech-gaming-2", CreatedAt = SeedTime, UpdatedAt = SeedTime },
                new { Id = 12, Name = "Razer", Slug = "razer", Country = "USA", LogoUrl = "https://worldvectorlogo.com/logo/razer-1", CreatedAt = SeedTime, UpdatedAt = SeedTime },
                new { Id = 13, Name = "Sony", Slug = "sony", Country = "Japan", LogoUrl = "https://worldvectorlogo.com/logo/sony-2", CreatedAt = SeedTime, UpdatedAt = SeedTime },
                new { Id = 14, Name = "Anker", Slug = "anker", Country = "China", LogoUrl = "https://worldvectorlogo.com/logo/anker-logo-1", CreatedAt = SeedTime, UpdatedAt = SeedTime },
                new { Id = 15, Name = "Baseus", Slug = "baseus", Country = "China", LogoUrl = "https://mms.img.susercontent.com/vn-11134216-7r98o-lnicyi57m5x6fd", CreatedAt = SeedTime, UpdatedAt = SeedTime },
                new { Id = 16, Name = "Spigen", Slug = "spigen", Country = "USA", LogoUrl = "https://spigen.vn/wp-content/uploads/2023/09/Spigen_Header_New_Logo.png", CreatedAt = SeedTime, UpdatedAt = SeedTime },
                new { Id = 17, Name = "UAG", Slug = "uag", Country = "USA", LogoUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRlpSaYkZMxWktmvmvOx7mDurTEDu0KXqz1HQ&s", CreatedAt = SeedTime, UpdatedAt = SeedTime }
            );
        }
    }
}
