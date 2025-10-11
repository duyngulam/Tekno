using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tekno.Domain.Catalog;

namespace Tekno.Infrastructure.Persistence.Configurations
{
    public class ProductVariantAttributeConfiguration : IEntityTypeConfiguration<ProductVariantAttribute>
    {
        public void Configure(EntityTypeBuilder<ProductVariantAttribute> builder)
        {
            builder.ToTable("product_variant_attribute");

            builder.HasKey(pva => new { pva.VariantId, pva.AttributeId });

            builder.HasOne(pva => pva.Variant)
                   .WithMany(v => v.VariantAttributes)
                   .HasForeignKey(pva => pva.VariantId);

            builder.HasOne(pva => pva.Attribute)
                   .WithMany()
                   .HasForeignKey(pva => pva.AttributeId);

            builder.HasOne(pva => pva.Value)
                   .WithMany()
                   .HasForeignKey(pva => pva.ValueId);
            // ===================== SEED DỮ LIỆU CHO PRODUCT VARIANT ATTRIBUTES =====================
            builder.HasData(
                // ====== Laptop: Dell XPS 13 ======
                new { VariantId = 1, AttributeId = 1, ValueId = 1 },   // Color = Silver
                new { VariantId = 1, AttributeId = 12, ValueId = 21 }, // RAM = 8GB
                new { VariantId = 1, AttributeId = 13, ValueId = 31 }, // Storage = 512GB

                new { VariantId = 2, AttributeId = 1, ValueId = 2 },   // Color = Black
                new { VariantId = 2, AttributeId = 12, ValueId = 22 }, // RAM = 16GB
                new { VariantId = 2, AttributeId = 13, ValueId = 32 }, // Storage = 1TB

                // ====== Laptop: MacBook Air M2 ======
                new { VariantId = 3, AttributeId = 1, ValueId = 3 },   // Color = Gray
                new { VariantId = 3, AttributeId = 12, ValueId = 21 }, // RAM = 8GB
                new { VariantId = 3, AttributeId = 13, ValueId = 30 }, // Storage = 256GB

                new { VariantId = 4, AttributeId = 1, ValueId = 1 },   // Color = Silver
                new { VariantId = 4, AttributeId = 12, ValueId = 22 }, // RAM = 16GB
                new { VariantId = 4, AttributeId = 13, ValueId = 31 }, // Storage = 512GB

                // ====== Smartphone: iPhone 15 Pro ======
                new { VariantId = 11, AttributeId = 1, ValueId = 1 },  // Color = Silver
                new { VariantId = 11, AttributeId = 24, ValueId = 30 },// Storage = 128GB

                new { VariantId = 12, AttributeId = 1, ValueId = 2 },  // Color = Black
                new { VariantId = 12, AttributeId = 24, ValueId = 31 },// Storage = 256GB

                // ====== Smartphone: Galaxy S24 ======
                new { VariantId = 13, AttributeId = 1, ValueId = 3 },  // Color = Gray
                new { VariantId = 13, AttributeId = 24, ValueId = 30 },// Storage = 128GB

                new { VariantId = 14, AttributeId = 1, ValueId = 4 },  // Color = Blue
                new { VariantId = 14, AttributeId = 24, ValueId = 31 },// Storage = 256GB

                // ====== Tablet: iPad Pro ======
                new { VariantId = 17, AttributeId = 1, ValueId = 1 },  // Color = Silver
                new { VariantId = 17, AttributeId = 33, ValueId = 30 },// Storage = 128GB

                new { VariantId = 18, AttributeId = 1, ValueId = 2 },  // Color = Black
                new { VariantId = 18, AttributeId = 33, ValueId = 31 },// Storage = 256GB

                // ====== Monitor: Dell U2720Q ======
                new { VariantId = 21, AttributeId = 50, ValueId = 40 },// Screen Size = 27”
                new { VariantId = 21, AttributeId = 52, ValueId = 42 },// Resolution = 4K

                // ====== Keyboard: MX Keys ======
                new { VariantId = 23, AttributeId = 70, ValueId = 50 },// Switch Type = Membrane
                new { VariantId = 23, AttributeId = 73, ValueId = 60 },// Connection Type = Wireless

                // ====== Mouse: MX Master 3S ======
                new { VariantId = 25, AttributeId = 80, ValueId = 70 },// DPI = 8000
                new { VariantId = 25, AttributeId = 81, ValueId = 60 },// Connection = Wireless

                // ====== Headphones: Sony WH-1000XM5 ======
                new { VariantId = 27, AttributeId = 90, ValueId = 80 },// Type = Over-Ear
                new { VariantId = 27, AttributeId = 91, ValueId = 60 },// Connection = Wireless
                new { VariantId = 27, AttributeId = 92, ValueId = 81 },// Has Microphone = Yes

                // ====== Charger: Anker 65W ======
                new { VariantId = 29, AttributeId = 100, ValueId = 90 },// Connector = USB-C
                new { VariantId = 29, AttributeId = 101, ValueId = 91 },// Power Output = 65W

                // ====== Case: Spigen Rugged ======
                new { VariantId = 31, AttributeId = 110, ValueId = 100 },// Material = TPU
                new { VariantId = 31, AttributeId = 111, ValueId = 101 },// Device Type = Phone
                new { VariantId = 31, AttributeId = 112, ValueId = 102 } // Shock Resistant = Yes
            );
        }
    }
}
