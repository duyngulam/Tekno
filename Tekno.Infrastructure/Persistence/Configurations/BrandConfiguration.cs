using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tekno.Domain.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Tekno.Infrastructure.Persistence.Configurations
{
    public class BrandConfiguration : IEntityTypeConfiguration<Brand>
    {
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
            builder.HasMany(b => b.Products)
                .WithOne(p => p.Brand)
                .HasForeignKey(p => p.BrandId);
            builder.HasData(
                new
                {
                    Id = 1,
                    Name = "Apple",
                    Slug = "apple",
                    Country = "USA",
                    LogoUrl = "https://worldvectorlogo.com/logo/apple-13",
                    CreatedAt = DateTime.UtcNow
                },
                new
                {
                    Id = 2,
                    Name = "Samsung",
                    Slug = "samsung",
                    Country = "Korea",
                    LogoUrl = "https://worldvectorlogo.com/logo/samsung-8",
                    CreatedAt = DateTime.UtcNow
                },
                new
                {
                    Id = 3,
                    Name = "Dell",
                    Slug = "dell",
                    Country = "USA",
                    LogoUrl = "https://worldvectorlogo.com/logo/dell-2",
                    CreatedAt = DateTime.UtcNow
                },
                new
                {
                    Id = 4,
                    Name = "Asus",
                    Slug = "asus",
                    Country = "Taiwan",
                    LogoUrl = "https://worldvectorlogo.com/logo/asus-4",
                    CreatedAt = DateTime.UtcNow
                },
                new
                {
                    Id = 5,
                    Name = "Xiaomi",
                    Slug = "xiaomi",
                    Country = "China",
                    LogoUrl = "https://worldvectorlogo.com/logo/xiaomi-5",
                    CreatedAt = DateTime.UtcNow
                },
                new
                {
                    Id = 6,
                    Name = "Lenovo",
                    Slug = "lenovo",
                    Country = "China",
                    LogoUrl = "https://worldvectorlogo.com/logo/lenovo-2",
                    CreatedAt = DateTime.UtcNow
                },
                new
                {
                    Id = 7,
                    Name = "MSI",
                    Slug = "msi",
                    Country = "Taiwan",
                    LogoUrl = "https://worldvectorlogo.com/logo/msi-5",
                    CreatedAt = DateTime.UtcNow
                },
                new
                {
                    Id = 8,
                    Name = "HP",
                    Slug = "hp",
                    Country = "USA",
                    LogoUrl = "https://worldvectorlogo.com/logo/HP-5",
                    CreatedAt = DateTime.UtcNow
                }
            );

        }
    }
}
