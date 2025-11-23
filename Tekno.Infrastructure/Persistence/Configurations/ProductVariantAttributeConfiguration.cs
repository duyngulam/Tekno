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


            // ===== Re-aligned seed data to match updated AttributeValue ids =====
            builder.HasData(
                // ====== Laptop: Dell XPS 13 ======
                new { VariantId = 1, AttributeId = 1, ValueId = 3 },   // Color = Silver (valueId 3)
                new { VariantId = 1, AttributeId = 12, ValueId = 17 }, // RAM = 8GB (valueId 17)
                new { VariantId = 1, AttributeId = 13, ValueId = 21 }, // Storage = 512GB (valueId 21)

                new { VariantId = 2, AttributeId = 1, ValueId = 1 },   // Color = Black (valueId 1)
                new { VariantId = 2, AttributeId = 12, ValueId = 18 }, // RAM = 16GB (valueId 18)
                new { VariantId = 2, AttributeId = 13, ValueId = 22 }, // Storage = 1TB (valueId 22)

                // ====== Laptop: MacBook Air M2 ======
                new { VariantId = 3, AttributeId = 1, ValueId = 3 },   // Color = Silver (valueId 3)
                new { VariantId = 3, AttributeId = 12, ValueId = 17 }, // RAM = 8GB (valueId 17)
                new { VariantId = 3, AttributeId = 13, ValueId = 20 }, // Storage = 256GB (valueId 20)

                new { VariantId = 4, AttributeId = 1, ValueId = 3 },   // Color = Silver
                new { VariantId = 4, AttributeId = 12, ValueId = 18 }, // RAM = 16GB
                new { VariantId = 4, AttributeId = 13, ValueId = 21 }, // Storage = 512GB

                // ====== Smartphone: iPhone 15 Pro ======
                new { VariantId = 11, AttributeId = 1, ValueId = 3 },  // Color = Silver
                new { VariantId = 11, AttributeId = 24, ValueId = 43 },// Storage = 128GB (smartphone storage mapping 42/43/44)

                new { VariantId = 12, AttributeId = 1, ValueId = 1 },  // Color = Black
                new { VariantId = 12, AttributeId = 24, ValueId = 44 },// Storage = 256GB

                // ====== Smartphone: Galaxy S24 ======
                new { VariantId = 13, AttributeId = 1, ValueId = 3 },  // Color = Silver/Gray
                new { VariantId = 13, AttributeId = 24, ValueId = 43 },// Storage = 128GB

                new { VariantId = 14, AttributeId = 1, ValueId = 4 },  // Color = Blue
                new { VariantId = 14, AttributeId = 24, ValueId = 44 },// Storage = 256GB

                // ====== Tablet: iPad Pro (assumes attribute 33 is tablet storage) ======
                new { VariantId = 17, AttributeId = 1, ValueId = 3 },  // Color = Silver
                new { VariantId = 17, AttributeId = 33, ValueId = 43 },// Storage = 128GB (align to 43)

                new { VariantId = 18, AttributeId = 1, ValueId = 1 },  // Color = Black
                new { VariantId = 18, AttributeId = 33, ValueId = 44 },// Storage = 256GB

                // ====== Monitor: Dell U2720Q ======
                new { VariantId = 21, AttributeId = 50, ValueId = 52 },// Screen Size = 27"
                new { VariantId = 21, AttributeId = 52, ValueId = 58 },// Resolution = 4K

                // ====== Keyboard: MX Keys ======
                // NOTE: switch types/values may differ; choose nearest seeded values
                new { VariantId = 23, AttributeId = 70, ValueId = 70 },// Switch Type = "Red" (70)
                new { VariantId = 23, AttributeId = 73, ValueId = 74 },// Connection Type = Wireless (73 -> 74)

                // ====== Mouse: MX Master 3S ======
                new { VariantId = 25, AttributeId = 80, ValueId = 80 },// DPI = 800
                new { VariantId = 25, AttributeId = 81, ValueId = 83 },// Connection = Wireless

                // ====== Headphones: Sony WH-1000XM5 ======
                new { VariantId = 27, AttributeId = 90, ValueId = 91 },// Type = Over-Ear
                new { VariantId = 27, AttributeId = 91, ValueId = 94 },// Connection = Bluetooth

                // ====== Charger: Anker 65W ======
                new { VariantId = 29, AttributeId = 100, ValueId = 100 },// Connector = USB-C
                new { VariantId = 29, AttributeId = 101, ValueId = 101 },// Power Output = 65

                // ====== Case: Spigen Rugged ======
                new { VariantId = 31, AttributeId = 110, ValueId = 110 },// Material = Silicone
                new { VariantId = 31, AttributeId = 111, ValueId = 113 },// Device Type = 1phone
                new { VariantId = 31, AttributeId = 112, ValueId = 115 } // Shock Resistant = Yes
            );
        }
    }
}
