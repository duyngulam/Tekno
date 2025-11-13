using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tekno.Domain.Catalog;

namespace Tekno.Infrastructure.Persistence.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        private static readonly DateTime SeedTime = new DateTime(2025, 10, 12, 9, 34, 0, DateTimeKind.Utc);
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("product");
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Name).IsRequired().HasMaxLength(150);
            builder.Property(p => p.BasePrice).IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(p => p.Status).IsRequired().HasMaxLength(50).HasDefaultValue("available");
            builder.Property(p => p.Slug).IsRequired().HasMaxLength(200);
            builder.HasIndex(p => p.Slug).IsUnique();

            builder.Property(p => p.CreatedAt).HasColumnType("timestamptz").HasDefaultValueSql("NOW()");
            builder.Property(p => p.UpdatedAt).HasColumnType("timestamptz").HasDefaultValueSql("NOW()");
            builder.Property(p => p.Description).HasColumnType("text");
            builder.Property(p => p.Overview).HasColumnType("text");
            builder.Property(p => p.Specs).HasColumnType("jsonb");

            builder.HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId);

            builder.HasOne(p => p.Brand)
                .WithMany(b => b.Products)
                .HasForeignKey(p => p.BrandId);

            //// ========== SEED PRODUCT DATA ==========
            builder.HasData(
                // ===== Laptops =====
                new { Id = 1, Name = "Dell XPS 13", Slug = "dell-xps-13", BrandId = 1, CategoryId = 1, BasePrice = 1699.00M, Status = "available", Overview = "Premium ultrabook with compact design.", Description = "Dell XPS 13 series laptops designed for professionals.", CreatedAt = SeedTime, UpdatedAt = SeedTime,
                    Specs = """
        [
            {"Name":"Display","Value":["13.4-inch FHD+ InfinityEdge"]},
            {"Name":"CPU","Value":["Intel i5","Intel i7"]},
            {"Name":"RAM","Value":["8GB","16GB"]},
            {"Name":"Storage","Value":["512GB","1TB SSD"]},
            {"Name":"Weight","Value":["1.2kg"]},
            {"Name":"Battery","Value":["52Wh"]},
            {"Name":"OS","Value":["Windows 11"]},
            {"Name":"Warranty","Value":["12 months"]}
        ]
        """ },
                new { Id = 2, Name = "MacBook Air", Slug = "macbook-air", BrandId = 2, CategoryId = 1, BasePrice = 1199.00M, Status = "available", Overview = "Ultra-thin and lightweight laptop by Apple.", Description = "MacBook Air powered by Apple M-series chips.", CreatedAt = SeedTime, UpdatedAt = SeedTime,
                    Specs = """
        [
            {"Name":"Display","Value":["13.6-inch Liquid Retina"]},
            {"Name":"Chip","Value":["Apple M2"]},
            {"Name":"RAM","Value":["8GB","16GB"]},
            {"Name":"Storage","Value":["256GB","512GB"]},
            {"Name":"Battery","Value":["52.6Wh up to 18h"]},
            {"Name":"OS","Value":["macOS"]},
            {"Name":"Weight","Value":["1.24kg"]}
        ]
        """
                },
                new { Id = 3, Name = "Asus ZenBook", Slug = "asus-zenbook", BrandId = 3, CategoryId = 1, BasePrice = 1450.00M, Status = "available", Overview = "Portable productivity ultrabook.", Description = "ZenBook series with Intel and AMD variants.", CreatedAt = SeedTime, UpdatedAt = SeedTime,
                    Specs = """
        [
            {"Name":"Display","Value":["14-inch OLED 2.8K"]},
            {"Name":"CPU","Value":["Intel i5","Intel i7","Ryzen 7"]},
            {"Name":"RAM","Value":["8GB","16GB"]},
            {"Name":"Storage","Value":["512GB","1TB SSD"]},
            {"Name":"OS","Value":["Windows 11"]},
            {"Name":"Weight","Value":["1.3kg"]}
        ]
        """
                },
                new { Id = 4, Name = "HP Spectre x360", Slug = "hp-spectre-x360", BrandId = 4, CategoryId = 1, BasePrice = 1799.00M, Status = "available", Overview = "Convertible premium laptop.", Description = "2-in-1 design with touch and pen support.", CreatedAt = SeedTime, UpdatedAt = SeedTime,
                    Specs = """
        [
            {"Name":"Display","Value":["13.5-inch 2-in-1 Touch OLED"]},
            {"Name":"CPU","Value":["Intel i5","Intel i7"]},
            {"Name":"RAM","Value":["8GB","16GB"]},
            {"Name":"Storage","Value":["512GB","1TB"]},
            {"Name":"Convertible","Value":["true"]},
            {"Name":"OS","Value":["Windows 11 Home"]}
        ]
        """
                },
                new { Id = 5, Name = "Lenovo ThinkPad X1 Carbon", Slug = "thinkpad-x1-carbon", BrandId = 5, CategoryId = 1, BasePrice = 1999.00M, Status = "available", Overview = "Business ultrabook with robust build.", Description = "ThinkPad X1 Carbon for professionals.", CreatedAt = SeedTime, UpdatedAt = SeedTime,
                    Specs = """
        [
            {"Name":"Display","Value":["14-inch IPS 2.8K"]},
            {"Name":"CPU","Value":["Intel i5","Intel i7"]},
            {"Name":"RAM","Value":["8GB","16GB"]},
            {"Name":"Storage","Value":["512GB","1TB"]},
            {"Name":"Security","Value":["Fingerprint","TPM 2.0"]},
            {"Name":"OS","Value":["Windows 11 Pro"]}
        ]
        """
                },

                // ===== Smartphones =====
                new { Id = 10, Name = "iPhone 15 Pro", Slug = "iphone-15-pro", BrandId = 2, CategoryId = 2, BasePrice = 999.00M, Status = "available", Overview = "Apple flagship smartphone.", Description = "iPhone 15 Pro with titanium frame and A17 Pro chip.", CreatedAt = SeedTime, UpdatedAt = SeedTime,
                    Specs = """
        [
            {"Name":"Display","Value":["6.1-inch OLED 120Hz"]},
            {"Name":"Chip","Value":["Apple A17 Pro"]},
            {"Name":"RAM","Value":["6GB"]},
            {"Name":"Storage","Value":["128GB","256GB"]},
            {"Name":"Camera","Value":["48MP","12MP","12MP"]},
            {"Name":"Battery","Value":["3279mAh"]},
            {"Name":"OS","Value":["iOS 17"]}
        ]
        """
                },
                new { Id = 11, Name = "Samsung Galaxy S24", Slug = "galaxy-s24", BrandId = 6, CategoryId = 2, BasePrice = 899.00M, Status = "available", Overview = "Next-gen Android flagship.", Description = "Galaxy S24 with AMOLED display and powerful camera.", CreatedAt = SeedTime, UpdatedAt = SeedTime,
                    Specs = """
        [
            {"Name":"Display","Value":["6.7-inch Dynamic AMOLED 120Hz"]},
            {"Name":"Chip","Value":["Snapdragon 8 Gen 3"]},
            {"Name":"RAM","Value":["8GB"]},
            {"Name":"Storage","Value":["128GB","256GB"]},
            {"Name":"Camera","Value":["200MP","12MP","10MP"]},
            {"Name":"Battery","Value":["5000mAh"]},
            {"Name":"OS","Value":["Android 14"]}
        ]
        """
                },
                new { Id = 12, Name = "Google Pixel 9", Slug = "pixel-9", BrandId = 7, CategoryId = 2, BasePrice = 799.00M, Status = "available", Overview = "Pure Android experience.", Description = "Pixel 9 with Tensor G4 chip and AI photography.", CreatedAt = SeedTime, UpdatedAt = SeedTime,
                    Specs = """
        [
            {"Name":"Display","Value":["6.3-inch AMOLED 120Hz"]},
            {"Name":"Chip","Value":["Google Tensor G4"]},
            {"Name":"RAM","Value":["8GB"]},
            {"Name":"Storage","Value":["128GB","256GB"]},
            {"Name":"Camera","Value":["50MP","12MP"]},
            {"Name":"Battery","Value":["4700mAh"]},
            {"Name":"OS","Value":["Android 14"]}
        ]
        """
                },
                new { Id = 13, Name = "Xiaomi 14 Pro", Slug = "xiaomi-14-pro", BrandId = 8, CategoryId = 2, BasePrice = 850.00M, Status = "available", Overview = "High-end performance smartphone.", Description = "Xiaomi 14 Pro with Leica cameras.", CreatedAt = SeedTime, UpdatedAt = SeedTime,
                    Specs = """
        [
            {"Name":"Display","Value":["6.7-inch AMOLED QHD+"]},
            {"Name":"Chip","Value":["Snapdragon 8 Gen 3"]},
            {"Name":"RAM","Value":["12GB"]},
            {"Name":"Storage","Value":["256GB","512GB"]},
            {"Name":"Camera","Value":["50MP","50MP","50MP (Leica)"]},
            {"Name":"OS","Value":["HyperOS (Android 14)"]}
        ]
        """
                },
                new { Id = 14, Name = "OnePlus 12", Slug = "oneplus-12", BrandId = 9, CategoryId = 2, BasePrice = 749.00M, Status = "available", Overview = "Performance-focused smartphone.", Description = "OnePlus 12 offers fast charging and high refresh rate.", CreatedAt = SeedTime, UpdatedAt = SeedTime,
                    Specs = """
        [
            {"Name":"Display","Value":["6.8-inch AMOLED 120Hz"]},
            {"Name":"Chip","Value":["Snapdragon 8 Gen 3"]},
            {"Name":"RAM","Value":["12GB"]},
            {"Name":"Storage","Value":["256GB","512GB"]},
            {"Name":"Battery","Value":["5400mAh 100W charging"]},
            {"Name":"OS","Value":["OxygenOS 14"]}
        ]
        """
                },

                // ===== Tablets =====
                new { Id = 20, Name = "iPad Pro", Slug = "ipad-pro", BrandId = 2, CategoryId = 3, BasePrice = 899.00M, Status = "available", Overview = "Powerful tablet for creators.", Description = "iPad Pro with M2 chip and ProMotion display.", CreatedAt = SeedTime, UpdatedAt = SeedTime,
                    Specs = """
        [
            {"Name":"Display","Value":["12.9-inch Liquid Retina XDR"]},
            {"Name":"Chip","Value":["Apple M2"]},
            {"Name":"RAM","Value":["8GB","16GB"]},
            {"Name":"Storage","Value":["128GB","256GB"]},
            {"Name":"OS","Value":["iPadOS 17"]},
            {"Name":"PencilSupport","Value":["Apple Pencil 2"]}
        ]
        """
                },
                new { Id = 21, Name = "Samsung Galaxy Tab S9", Slug = "galaxy-tab-s9", BrandId = 6, CategoryId = 3, BasePrice = 799.00M, Status = "available", Overview = "Android flagship tablet.", Description = "Galaxy Tab S9 series with AMOLED display.", CreatedAt = SeedTime, UpdatedAt = SeedTime,
                    Specs = """
        [
            {"Name":"Display","Value":["11-inch AMOLED 120Hz"]},
            {"Name":"Chip","Value":["Snapdragon 8 Gen 2"]},
            {"Name":"RAM","Value":["8GB","12GB"]},
            {"Name":"Storage","Value":["128GB","256GB"]},
            {"Name":"OS","Value":["Android 14"]}
        ]
        """
                },
                new { Id = 22, Name = "Lenovo Tab P12", Slug = "lenovo-tab-p12", BrandId = 5, CategoryId = 3, BasePrice = 419.00M, Status = "available", Overview = "Affordable productivity tablet.", Description = "Tab P12 supports stylus and multi-tasking.", CreatedAt = SeedTime, UpdatedAt = SeedTime,
                    Specs = """
        [
            {"Name":"Display","Value":["12.7-inch LCD 144Hz"]},
            {"Name":"Chip","Value":["MediaTek Dimensity 7050"]},
            {"Name":"RAM","Value":["8GB"]},
            {"Name":"Storage","Value":["128GB"]},
            {"Name":"Battery","Value":["10200mAh"]},
            {"Name":"OS","Value":["Android 13"]}
        ]
        """
                },

                // ===== Monitors =====
                new { Id = 30, Name = "Dell UltraSharp 27", Slug = "dell-ultrasharp-27", BrandId = 1, CategoryId = 8, BasePrice = 699.00M, Status = "available", Overview = "Professional 4K monitor.", Description = "Color-accurate UltraSharp series for designers.", CreatedAt = SeedTime, UpdatedAt = SeedTime,
                    Specs = """
        [
            {"Name":"Display","Value":["27-inch IPS 4K UHD"]},
            {"Name":"Resolution","Value":["3840x2160"]},
            {"Name":"RefreshRate","Value":["60Hz"]},
            {"Name":"Ports","Value":["HDMI","DisplayPort","USB-C"]},
            {"Name":"ColorGamut","Value":["99% sRGB"]},
            {"Name":"Warranty","Value":["24 months"]}
        ]
        """
                },
                new { Id = 31, Name = "LG Ultragear 32", Slug = "lg-ultragear-32", BrandId = 10, CategoryId = 8, BasePrice = 450.00M, Status = "available", Overview = "Gaming monitor with 165Hz refresh rate.", Description = "High-performance display for gamers.", CreatedAt = SeedTime, UpdatedAt = SeedTime,
                    Specs = """
        [
            {"Name":"Display","Value":["32-inch VA QHD"]},
            {"Name":"Resolution","Value":["2560x1440"]},
            {"Name":"RefreshRate","Value":["165Hz"]},
            {"Name":"Ports","Value":["HDMI","DisplayPort"]},
            {"Name":"Sync","Value":["G-Sync Compatible"]}
        ]
        """
                },

                // ===== Keyboards =====
                new { Id = 40, Name = "Logitech MX Keys", Slug = "logitech-mx-keys", BrandId = 11, CategoryId = 13, BasePrice = 119.99M, Status = "available", Overview = "Wireless keyboard for professionals.", Description = "Backlit keyboard with multi-device support.", CreatedAt = SeedTime, UpdatedAt = SeedTime,
                    Specs = """
        [
            {"Name":"Type","Value":["Wireless"]},
            {"Name":"Layout","Value":["Full-size"]},
            {"Name":"Backlight","Value":["Yes"]},
            {"Name":"Battery","Value":["USB-C rechargeable"]}
        ]
        """
                },
                new { Id = 41, Name = "Razer BlackWidow V4", Slug = "razer-blackwidow-v4", BrandId = 12, CategoryId = 13, BasePrice = 169.99M, Status = "available", Overview = "Mechanical gaming keyboard.", Description = "RGB lighting and tactile feedback.", CreatedAt = SeedTime, UpdatedAt = SeedTime,
                    pecs = """
        [
            {"Name":"Type","Value":["Mechanical"]},
            {"Name":"Switch","Value":["Razer Green"]},
            {"Name":"Backlight","Value":["RGB"]},
            {"Name":"Connection","Value":["Wired"]}
        ]
        """
                },

                // ===== Mouse =====
                new { Id = 50, Name = "Logitech MX Master 3S", Slug = "logitech-mx-master-3s", BrandId = 11, CategoryId = 14, BasePrice = 99.99M, Status = "available", Overview = "Ergonomic productivity mouse.", Description = "Supports multiple devices with customizable buttons.", CreatedAt = SeedTime, UpdatedAt = SeedTime,
                    Specs = """
        [
            {"Name":"Sensor","Value":["Logitech Darkfield"]},
            {"Name":"Connection","Value":["Bluetooth","USB"]},
            {"Name":"Battery","Value":["70 days"]},
            {"Name":"Buttons","Value":["7"]}
        ]
        """
                },
                new { Id = 51, Name = "Razer Viper V2 Pro", Slug = "razer-viper-v2-pro", BrandId = 12, CategoryId = 14, BasePrice = 149.99M, Status = "available", Overview = "Ultra-light gaming mouse.", Description = "Wireless mouse with high precision sensor.", CreatedAt = SeedTime, UpdatedAt = SeedTime,
                    Specs = """
        [
            {"Name":"Sensor","Value":["Focus Pro 30K"]},
            {"Name":"Weight","Value":["58g"]},
            {"Name":"Connection","Value":["Wireless"]},
            {"Name":"Battery","Value":["80h"]}
        ]
        """
                },

                // ===== Headphones =====
                new { Id = 60, Name = "Sony WH-1000XM5", Slug = "sony-wh-1000xm5", BrandId = 13, CategoryId = 15, BasePrice = 399.99M, Status = "available", Overview = "Noise-cancelling wireless headphones.", Description = "Industry-leading noise cancellation for audio lovers.", CreatedAt = SeedTime, UpdatedAt = SeedTime,
                    Specs = """
        [
            {"Name":"Type","Value":["Over-ear"]},
            {"Name":"ANC","Value":["Yes"]},
            {"Name":"Battery","Value":["30h"]},
            {"Name":"Charging","Value":["USB-C"]},
            {"Name":"Microphone","Value":["Yes"]}
        ]
        """
                },
                new { Id = 61, Name = "Apple AirPods Pro 2", Slug = "airpods-pro-2", BrandId = 2, CategoryId = 15, BasePrice = 249.00M, Status = "available", Overview = "Wireless earbuds with active noise cancellation.", Description = "Compact design and adaptive sound control.", CreatedAt = SeedTime, UpdatedAt = SeedTime,
                    Specs = """
        [
            {"Name":"Type","Value":["In-ear"]},
            {"Name":"ANC","Value":["Yes"]},
            {"Name":"Battery","Value":["6h","24h"]},
            {"Name":"Wireless","Value":["Bluetooth 5.3"]}
        ]
        """
                },

                // ===== Chargers & Cables =====
                new { Id = 70, Name = "Anker 65W GaN Charger", Slug = "anker-65w-gan", BrandId = 14, CategoryId = 16, BasePrice = 49.99M, Status = "available", Overview = "Fast charger with GaN technology.", Description = "Compact USB-C charger for laptops and phones.", CreatedAt = SeedTime, UpdatedAt = SeedTime,
                    Specs = """
        [
            {"Name":"Power","Value":["65W"]},
            {"Name":"Ports","Value":["2x USB-C","1x USB-A"]},
            {"Name":"Material","Value":["GaN"]},
            {"Name":"Input","Value":["100–240V"]}
        ]
        """
                },
                new { Id = 71, Name = "Baseus USB-C Cable 1.5m", Slug = "baseus-usb-c-cable", BrandId = 15, CategoryId = 16, BasePrice = 14.99M, Status = "available", Overview = "Durable braided charging cable.", Description = "Supports fast charging and data transfer.", CreatedAt = SeedTime, UpdatedAt = SeedTime,
                    Specs = """
        [
            {"Name":"Length","Value":["1.5m"]},
            {"Name":"Connector","Value":["USB-C to USB-C"]},
            {"Name":"Material","Value":["Nylon braided"]},
            {"Name":"MaxPower","Value":["100W"]}
        ]
        """
                },

                // ===== Cases & Covers =====
                new { Id = 80, Name = "Spigen Rugged Armor Case", Slug = "spigen-rugged-armor", BrandId = 16, CategoryId = 17, BasePrice = 29.99M, Status = "available", Overview = "Protective phone case.", Description = "Shock-absorbing TPU material.", CreatedAt = SeedTime, UpdatedAt = SeedTime,
                    Specs = """
        [
            {"Name":"Material","Value":["TPU"]},
            {"Name":"ShockResistant","Value":["Yes"]},
            {"Name":"CompatibleDevices","Value":["iPhone 15"]}
        ]
        """
                },
                new { Id = 81, Name = "UAG Plasma Laptop Sleeve", Slug = "uag-laptop-sleeve", BrandId = 17, CategoryId = 17, BasePrice = 49.99M, Status = "available", Overview = "Durable protective sleeve.", Description = "Designed for 13–15 inch laptops.", CreatedAt = SeedTime, UpdatedAt = SeedTime,
                    Specs = """
        [
            {"Name":"Material","Value":["Ballistic Nylon"]},
            {"Name":"Fits","Value":["13–15 inch laptops"]},
            {"Name":"WaterResistant","Value":["Yes"]}
        ]
        """
                }
            );
        }
    }
}
