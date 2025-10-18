using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tekno.Domain.Catalog;

namespace Tekno.Infrastructure.Persistence.Configurations
{
    public class ProductAttributeConfiguration : IEntityTypeConfiguration<ProductAttribute>
    {
        private static readonly DateTime SeedTime = new DateTime(2025, 10, 12, 9, 34, 0, DateTimeKind.Utc);
        public void Configure(EntityTypeBuilder<ProductAttribute> builder)
        {
            builder.ToTable("product_attribute");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Name)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(a => a.InputType)
                   .HasMaxLength(50)
                   .HasDefaultValue("select");

            builder.Property(a => a.IsGlobal)
                   .HasDefaultValue(false);

            builder.Property(a => a.CreatedAt)
                   .HasDefaultValueSql("NOW()");

            // Quan hệ với Category
            builder.HasOne(a => a.Category)
                   .WithMany(c => c.Attributes)
                   .HasForeignKey(a => a.CategoryId)
                   .OnDelete(DeleteBehavior.SetNull);

            builder.HasMany(a => a.Values)
                   .WithOne(av => av.Attribute)
                   .HasForeignKey(av => av.AttributeId)
                   .OnDelete(DeleteBehavior.Cascade);

            // ====== Seed data cho thuộc tính sản phẩm ======
            builder.HasData(
                // ===== Global Attributes =====
                new { Id = 1, Name = "Color", InputType = "select", IsGlobal = true, CreatedAt = SeedTime },
                new { Id = 2, Name = "Warranty Period", InputType = "number", IsGlobal = true, CreatedAt = SeedTime },

                // ===== Laptop (CategoryId = 1) =====
                new { Id = 10, Name = "Screen Size", InputType = "select", CategoryId = 1, CreatedAt = SeedTime },
                new { Id = 11, Name = "CPU", InputType = "select", CategoryId = 1, CreatedAt = SeedTime },
                new { Id = 12, Name = "RAM", InputType = "select", CategoryId = 1, CreatedAt = SeedTime },
                new { Id = 13, Name = "Storage", InputType = "select", CategoryId = 1, CreatedAt = SeedTime },
                new { Id = 14, Name = "GPU", InputType = "select", CategoryId = 1, CreatedAt = SeedTime },

                // ===== Smartphone (CategoryId = 2) =====
                new { Id = 20, Name = "Screen Size", InputType = "select", CategoryId = 2, CreatedAt = SeedTime },
                new { Id = 21, Name = "Battery Capacity", InputType = "number", CategoryId = 2, CreatedAt = SeedTime },
                new { Id = 22, Name = "Camera Resolution", InputType = "select", CategoryId = 2, CreatedAt = SeedTime },
                new { Id = 23, Name = "RAM", InputType = "select", CategoryId = 2, CreatedAt = SeedTime },
                new { Id = 24, Name = "Storage", InputType = "select", CategoryId = 2, CreatedAt = SeedTime },

                // ===== Tablet (CategoryId = 3) =====
                new { Id = 30, Name = "Screen Size", InputType = "select", CategoryId = 3, CreatedAt = SeedTime },
                new { Id = 31, Name = "Battery Capacity", InputType = "number", CategoryId = 3, CreatedAt = SeedTime },
                new { Id = 32, Name = "RAM", InputType = "select", CategoryId = 3, CreatedAt = SeedTime },
                new { Id = 33, Name = "Storage", InputType = "select", CategoryId = 3, CreatedAt = SeedTime },

                // ===== Computer & Office (CategoryId = 6) =====
                new { Id = 40, Name = "Processor Type", InputType = "select", CategoryId = 6, CreatedAt = SeedTime },
                new { Id = 41, Name = "RAM Type", InputType = "select", CategoryId = 6, CreatedAt = SeedTime },
                new { Id = 42, Name = "GPU Model", InputType = "select", CategoryId = 6, CreatedAt = SeedTime },

                // ===== Monitor (CategoryId = 8) =====
                new { Id = 50, Name = "Screen Size", InputType = "select", CategoryId = 8, CreatedAt = SeedTime },
                new { Id = 51, Name = "Refresh Rate", InputType = "select", CategoryId = 8, CreatedAt = SeedTime },
                new { Id = 52, Name = "Resolution", InputType = "select", CategoryId = 8, CreatedAt = SeedTime },
                new { Id = 53, Name = "Panel Type", InputType = "select", CategoryId = 8, CreatedAt = SeedTime },

                // ===== Accessories (CategoryId = 4) =====
                new { Id = 60, Name = "Compatibility", InputType = "select", CategoryId = 4, CreatedAt = SeedTime },
                new { Id = 61, Name = "Connection Type", InputType = "select", CategoryId = 4, CreatedAt = SeedTime },

                // ===== Keyboard (CategoryId = 13) =====
                new { Id = 70, Name = "Switch Type", InputType = "select", CategoryId = 13, CreatedAt = SeedTime },
                new { Id = 71, Name = "Backlight", InputType = "select", CategoryId = 13, CreatedAt = SeedTime },
                new { Id = 72, Name = "Layout", InputType = "select", CategoryId = 13, CreatedAt = SeedTime },
                new { Id = 73, Name = "Connection Type", InputType = "select", CategoryId = 13, CreatedAt = SeedTime },

                // ===== Mouse (CategoryId = 14) =====
                new { Id = 80, Name = "DPI", InputType = "number", CategoryId = 14, CreatedAt = SeedTime },
                new { Id = 81, Name = "Connection Type", InputType = "select", CategoryId = 14, CreatedAt = SeedTime },
                new { Id = 82, Name = "RGB Lighting", InputType = "select", CategoryId = 14, CreatedAt = SeedTime },

                // ===== Headphones (CategoryId = 15) =====
                new { Id = 90, Name = "Type", InputType = "select", CategoryId = 15, CreatedAt = SeedTime },
                new { Id = 91, Name = "Connection Type", InputType = "select", CategoryId = 15, CreatedAt = SeedTime },
                new { Id = 92, Name = "Has Microphone", InputType = "select", CategoryId = 15, CreatedAt = SeedTime },

                // ===== Charger & Cable (CategoryId = 16) =====
                new { Id = 100, Name = "Connector Type", InputType = "select", CategoryId = 16, CreatedAt = SeedTime },
                new { Id = 101, Name = "Power Output (W)", InputType = "number", CategoryId = 16, CreatedAt = SeedTime },
                new { Id = 102, Name = "Cable Length (m)", InputType = "number", CategoryId = 16, CreatedAt = SeedTime },

                // ===== Case & Cover (CategoryId = 17) =====
                new { Id = 110, Name = "Material", InputType = "select", CategoryId = 17, CreatedAt = SeedTime },
                new { Id = 111, Name = "Device Type", InputType = "select", CategoryId = 17, CreatedAt = SeedTime },
                new { Id = 112, Name = "Shock Resistant", InputType = "select", CategoryId = 17 }
            );
        }
    }
}
