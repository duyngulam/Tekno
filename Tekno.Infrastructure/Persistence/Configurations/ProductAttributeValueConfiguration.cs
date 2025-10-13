using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tekno.Domain.Catalog;

namespace Tekno.Infrastructure.Persistence.Configurations
{
    public class ProductAttributeValueConfiguration : IEntityTypeConfiguration<AttributeValue>
    {
        public void Configure(EntityTypeBuilder<AttributeValue> builder)
        {
            builder.ToTable("attribute_value");

            builder.HasKey(v => v.Id);

            builder.Property(v => v.Value)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(v => new { v.AttributeId, v.Value }).IsUnique();

            builder.HasOne(v => v.Attribute)
                   .WithMany(a => a.Values)
                   .HasForeignKey(v => v.AttributeId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Seed mẫu AttributeValue
            builder.HasData(
                // ===== Color =====
                new { Id = 1, AttributeId = 1, Value = "Black" },
                new { Id = 2, AttributeId = 1, Value = "White" },
                new { Id = 3, AttributeId = 1, Value = "Silver" },
                new { Id = 4, AttributeId = 1, Value = "Blue" },
                new { Id = 5, AttributeId = 1, Value = "Red" },

                // ===== Brand =====
                new { Id = 10, AttributeId = 2, Value = "Apple" },
                new { Id = 11, AttributeId = 2, Value = "Samsung" },
                new { Id = 12, AttributeId = 2, Value = "Asus" },
                new { Id = 13, AttributeId = 2, Value = "HP" },
                new { Id = 14, AttributeId = 2, Value = "Dell" },

                // ===== CPU ===== (Laptop)
                new { Id = 20, AttributeId = 11, Value = "Intel i5" },
                new { Id = 21, AttributeId = 11, Value = "Intel i7" },
                new { Id = 22, AttributeId = 11, Value = "AMD Ryzen 5" },
                new { Id = 23, AttributeId = 11, Value = "AMD Ryzen 7" },

                // ===== RAM ===== (Laptop / Smartphone)
                new { Id = 30, AttributeId = 12, Value = "8GB" },
                new { Id = 31, AttributeId = 12, Value = "16GB" },
                new { Id = 32, AttributeId = 12, Value = "32GB" },

                // ===== Storage =====
                new { Id = 40, AttributeId = 13, Value = "256GB SSD" },
                new { Id = 41, AttributeId = 13, Value = "512GB SSD" },
                new { Id = 42, AttributeId = 13, Value = "1TB SSD" },

                // ===== Screen Size (Monitor / Laptop) =====
                new { Id = 50, AttributeId = 10, Value = "13 inch" },
                new { Id = 51, AttributeId = 10, Value = "15 inch" },
                new { Id = 52, AttributeId = 10, Value = "17 inch" },

                // ===== GPU =====
                new { Id = 60, AttributeId = 14, Value = "RTX 4060" },
                new { Id = 61, AttributeId = 14, Value = "RTX 4070" },
                new { Id = 62, AttributeId = 14, Value = "GTX 1650" },

                // ===== Switch Type (Keyboard) =====
                new { Id = 70, AttributeId = 70, Value = "Red" },
                new { Id = 71, AttributeId = 70, Value = "Blue" },
                new { Id = 72, AttributeId = 70, Value = "Brown" },

                // ===== Connection Type (Global + Keyboard + Mouse + Headphone) =====
                new { Id = 80, AttributeId = 61, Value = "Wired" },
                new { Id = 81, AttributeId = 61, Value = "Wireless" },
                new { Id = 82, AttributeId = 73, Value = "Wired" },
                new { Id = 83, AttributeId = 81, Value = "Bluetooth" },
                new { Id = 84, AttributeId = 91, Value = "3.5mm" },
                new { Id = 85, AttributeId = 91, Value = "USB-C" },

                // ===== Material (Case & Cover) =====
                new { Id = 100, AttributeId = 110, Value = "Silicone" },
                new { Id = 101, AttributeId = 110, Value = "Leather" },
                new { Id = 102, AttributeId = 110, Value = "Plastic" }
            );
        }
    }
}
