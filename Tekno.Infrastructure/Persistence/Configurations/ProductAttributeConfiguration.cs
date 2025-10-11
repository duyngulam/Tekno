using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tekno.Domain.Catalog;

namespace Tekno.Infrastructure.Persistence.Configurations
{
    public class ProductAttributeConfiguration : IEntityTypeConfiguration<ProductAttribute>
    {
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
                new { id = 1, name = "Color", inputType = "select", isGlobal = true },
                new { id = 2, name = "Warranty Period", inputType = "number", isGlobal = true },

                // ===== Laptop =====
                new { id = 10, name = "Screen Size", inputType = "select", categoryId = 1 },
                new { id = 11, name = "CPU", inputType = "select", categoryId = 1 },
                new { id = 12, name = "RAM", inputType = "select", categoryId = 1 },
                new { id = 13, name = "Storage", inputType = "select", categoryId = 1 },
                new { id = 14, name = "GPU", inputType = "select", categoryId = 1 },

                // ===== Smartphone =====
                new { id = 20, name = "Screen Size", inputType = "select", categoryId = 2 },
                new { id = 21, name = "Battery Capacity", inputType = "number", categoryId = 2 },
                new { id = 22, name = "Camera Resolution", inputType = "select", categoryId = 2 },
                new { id = 23, name = "RAM", inputType = "select", categoryId = 2 },
                new { id = 24, name = "Storage", inputType = "select", categoryId = 2 },

                // ===== Tablet =====
                new { id = 30, name = "Screen Size", inputType = "select", categoryId = 3 },
                new { id = 31, name = "Battery Capacity", inputType = "number", categoryId = 3 },
                new { id = 32, name = "RAM", inputType = "select", categoryId = 3 },
                new { id = 33, name = "Storage", inputType = "select", categoryId = 3 },

                // ===== Computer & Office (CategoryId = 6) =====
                new { id = 40, name = "Processor Type", inputType = "select", categoryId = 6 },
                new { id = 41, name = "RAM Type", inputType = "select", categoryId = 6 },
                new { id = 42, name = "GPU Model", inputType = "select", categoryId = 6 },

                // ===== Monitor =====
                new { id = 50, name = "Screen Size", inputType = "select", categoryId = 8 },
                new { id = 51, name = "Refresh Rate", inputType = "select", categoryId = 8 },
                new { id = 52, name = "Resolution", inputType = "select", categoryId = 8 },
                new { id = 53, name = "Panel Type", inputType = "select", categoryId = 8 },

                // ===== Accessories (CategoryId = 4) =====
                new { id = 60, name = "Compatibility", inputType = "select", categoryId = 4 },
                new { id = 61, name = "Connection Type", inputType = "select", categoryId = 4 },

                // ===== Keyboard =====
                new { id = 70, name = "Switch Type", inputType = "select", categoryId = 13 },
                new { id = 71, name = "Backlight", inputType = "select", categoryId = 13 },
                new { id = 72, name = "Layout", inputType = "select", categoryId = 13 },
                new { id = 73, name = "Connection Type", inputType = "select", categoryId = 13 },

                // ===== Mouse =====
                new { id = 80, name = "DPI", inputType = "number", categoryId = 14 },
                new { id = 81, name = "Connection Type", inputType = "select", categoryId = 14 },
                new { id = 82, name = "RGB Lighting", inputType = "select", categoryId = 14 },

                // ===== Headphones =====
                new { id = 90, name = "Type", inputType = "select", categoryId = 15 },
                new { id = 91, name = "Connection Type", inputType = "select", categoryId = 15 },
                new { id = 92, name = "Has Microphone", inputType = "select", categoryId = 15 },

                // ===== Charger & Cable =====
                new { id = 100, name = "Connector Type", inputType = "select", categoryId = 16 },
                new { id = 101, name = "Power Output (W)", inputType = "number", categoryId = 16 },
                new { id = 102, name = "Cable Length (m)", inputType = "number", categoryId = 16 },

                // ===== Case & Cover =====
                new { id = 110, name = "Material", inputType = "select", categoryId = 17 },
                new { id = 111, name = "Device Type", inputType = "select", categoryId = 17 },
                new { id = 112, name = "Shock Resistant", inputType = "select", categoryId = 17 }
            );
        }
    }
}
