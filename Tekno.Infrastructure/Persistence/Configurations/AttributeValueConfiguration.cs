using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tekno.Domain.Catalog;

namespace Tekno.Infrastructure.Persistence.Configurations
{
    public class AttributeValueConfiguration : IEntityTypeConfiguration<AttributeValue>
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



            // Seed cleaned / corrected AttributeValue data.
            // NOTE: Values are grouped by AttributeId defined in ProductAttributeConfiguration.
            builder.HasData(
                // ===== Color (AttributeId = 1) =====
                new { Id = 1, AttributeId = 1, Value = "Black" },
                new { Id = 2, AttributeId = 1, Value = "White" },
                new { Id = 3, AttributeId = 1, Value = "Silver" },
                new { Id = 4, AttributeId = 1, Value = "Blue" },
                new { Id = 5, AttributeId = 1, Value = "Red" },

                // ===== Warranty Period (AttributeId = 2, numeric input) =====
                new { Id = 6, AttributeId = 2, Value = "12" },
                new { Id = 7, AttributeId = 2, Value = "24" },
                new { Id = 8, AttributeId = 2, Value = "36" },

                // ===== Laptop (CategoryId = 1) =====
                // Screen Size (AttributeId = 10)
                new { Id = 10, AttributeId = 10, Value = "13 inch" },
                new { Id = 11, AttributeId = 10, Value = "15 inch" },
                new { Id = 12, AttributeId = 10, Value = "17 inch" },

                // CPU (AttributeId = 11)
                new { Id = 13, AttributeId = 11, Value = "Intel i5" },
                new { Id = 14, AttributeId = 11, Value = "Intel i7" },
                new { Id = 15, AttributeId = 11, Value = "AMD Ryzen 5" },
                new { Id = 16, AttributeId = 11, Value = "AMD Ryzen 7" },

                // RAM (AttributeId = 12)
                new { Id = 17, AttributeId = 12, Value = "8GB" },
                new { Id = 18, AttributeId = 12, Value = "16GB" },
                new { Id = 19, AttributeId = 12, Value = "32GB" },

                // Storage (AttributeId = 13)
                new { Id = 20, AttributeId = 13, Value = "256GB SSD" },
                new { Id = 21, AttributeId = 13, Value = "512GB SSD" },
                new { Id = 22, AttributeId = 13, Value = "1TB SSD" },

                // GPU (AttributeId = 14)
                new { Id = 23, AttributeId = 14, Value = "RTX 4060" },
                new { Id = 24, AttributeId = 14, Value = "RTX 4070" },
                new { Id = 25, AttributeId = 14, Value = "GTX 1650" },

                // ===== Smartphone (CategoryId = 2) =====
                // Screen Size (AttributeId = 20)
                new { Id = 30, AttributeId = 20, Value = "5.5 inch" },
                new { Id = 31, AttributeId = 20, Value = "6.1 inch" },
                new { Id = 32, AttributeId = 20, Value = "6.7 inch" },

                // Battery Capacity (AttributeId = 21)
                new { Id = 33, AttributeId = 21, Value = "3000" },
                new { Id = 34, AttributeId = 21, Value = "4000" },
                new { Id = 35, AttributeId = 21, Value = "5000" },

                // Camera Resolution (AttributeId = 22)
                new { Id = 36, AttributeId = 22, Value = "12MP" },
                new { Id = 37, AttributeId = 22, Value = "48MP" },
                new { Id = 38, AttributeId = 22, Value = "108MP" },

                // RAM for smartphone (AttributeId = 23)
                new { Id = 39, AttributeId = 23, Value = "4GB" },
                new { Id = 40, AttributeId = 23, Value = "6GB" },
                new { Id = 41, AttributeId = 23, Value = "8GB" },

                // Storage for smartphone (AttributeId = 24)
                new { Id = 42, AttributeId = 24, Value = "64GB" },
                new { Id = 43, AttributeId = 24, Value = "128GB" },
                new { Id = 44, AttributeId = 24, Value = "256GB" },

                // ===== Monitor (CategoryId = 8) =====
                // Screen Size (AttributeId = 50)
                new { Id = 50, AttributeId = 50, Value = "21 inch" },
                new { Id = 51, AttributeId = 50, Value = "24 inch" },
                new { Id = 52, AttributeId = 50, Value = "27 inch" },

                // Refresh Rate (AttributeId = 51)
                new { Id = 53, AttributeId = 51, Value = "60Hz" },
                new { Id = 54, AttributeId = 51, Value = "120Hz" },
                new { Id = 55, AttributeId = 51, Value = "144Hz" },

                // Resolution (AttributeId = 52)
                new { Id = 56, AttributeId = 52, Value = "1080p" },
                new { Id = 57, AttributeId = 52, Value = "1440p" },
                new { Id = 58, AttributeId = 52, Value = "4K" },

                // ===== Accessories / Connection-type groups =====
                // Accessories: Compatibility (AttributeId = 60)
                new { Id = 60, AttributeId = 60, Value = "USB" },
                new { Id = 61, AttributeId = 60, Value = "USB-C" },

                // Accessories: Connection Type (AttributeId = 61)
                new { Id = 62, AttributeId = 61, Value = "Wired" },
                new { Id = 63, AttributeId = 61, Value = "Wireless" },

                // Keyboard: Switch Type (AttributeId = 70)
                new { Id = 70, AttributeId = 70, Value = "Red" },
                new { Id = 71, AttributeId = 70, Value = "Blue" },
                new { Id = 72, AttributeId = 70, Value = "Brown" },

                // Keyboard: Connection Type (AttributeId = 73)
                new { Id = 73, AttributeId = 73, Value = "Wired" },
                new { Id = 74, AttributeId = 73, Value = "Wireless" },

                // Mouse: DPI (AttributeId = 80)
                new { Id = 80, AttributeId = 80, Value = "800" },
                new { Id = 81, AttributeId = 80, Value = "1600" },

                // Mouse: Connection Type (AttributeId = 81)
                new { Id = 82, AttributeId = 81, Value = "Wired" },
                new { Id = 83, AttributeId = 81, Value = "Wireless" },

                // Headphone: Type (AttributeId = 90)
                new { Id = 90, AttributeId = 90, Value = "In-Ear" },
                new { Id = 91, AttributeId = 90, Value = "Over-Ear" },

                // Headphone: Connection Type (AttributeId = 91)
                new { Id = 92, AttributeId = 91, Value = "3.5mm" },
                new { Id = 93, AttributeId = 91, Value = "USB-C" },
                new { Id = 94, AttributeId = 91, Value = "Bluetooth" },

                // Charger (AttributeId = 100)
                new { Id = 100, AttributeId = 100, Value = "USB-C" },
                new { Id = 101, AttributeId = 101, Value = "65" },

                // Case & Cover (AttributeId = 110)
                new { Id = 110, AttributeId = 110, Value = "Silicone" },
                new { Id = 111, AttributeId = 110, Value = "Leather" },
                new { Id = 112, AttributeId = 110, Value = "Plastic" },
                new { Id = 113, AttributeId = 111, Value = "IPhone 17" },
                new { Id = 114, AttributeId = 111, Value = "Samsung Galaxy S24" },
                new { Id = 115, AttributeId = 112, Value = "Yes" }
            );
        }
    }
}
