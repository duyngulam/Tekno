using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tekno.Domain.Catalog;

namespace Tekno.Infrastructure.Persistence.Configurations
{
    public class ProductDetailConfiguration : IEntityTypeConfiguration<ProductDetail>
    {
        public void Configure(EntityTypeBuilder<ProductDetail> builder)
        {
            builder.ToTable("product_detail");

            builder.HasKey(pd => pd.ProductId);

            builder.Property(pd => pd.Specs).HasColumnType("jsonb");

            builder.HasOne(pd => pd.Product)
                   .WithOne(p => p.Detail)
                   .HasForeignKey<ProductDetail>(pd => pd.ProductId);

            // ========== SEED DATA ==========
            builder.HasData(
                // ===== Laptops =====
                new
                {
                    ProductId = 1,
                    Specs = """
                    {
                        "Display": "13.4-inch FHD+ InfinityEdge",
                        "CPU": "Intel Core i5 / i7",
                        "RAM": "8GB / 16GB",
                        "Storage": "512GB / 1TB SSD",
                        "Weight": "1.2kg",
                        "Battery": "52Wh",
                        "OS": "Windows 11",
                        "Warranty": "12 months"
                    }
                    """
                },
                new
                {
                    ProductId = 2,
                    Specs = """
                    {
                        "Display": "13.6-inch Liquid Retina",
                        "Chip": "Apple M2",
                        "RAM": "8GB / 16GB",
                        "Storage": "256GB / 512GB",
                        "Battery": "52.6Wh up to 18h",
                        "OS": "macOS",
                        "Weight": "1.24kg"
                    }
                    """
                },
                new
                {
                    ProductId = 3,
                    Specs = """
                    {
                        "Display": "14-inch OLED 2.8K",
                        "CPU": "Intel i5 / i7 or Ryzen 7",
                        "RAM": "8GB / 16GB",
                        "Storage": "512GB / 1TB SSD",
                        "OS": "Windows 11",
                        "Weight": "1.3kg"
                    }
                    """
                },
                new
                {
                    ProductId = 4,
                    Specs = """
                    {
                        "Display": "13.5-inch 2-in-1 Touch OLED",
                        "CPU": "Intel Core i5 / i7",
                        "RAM": "8GB / 16GB",
                        "Storage": "512GB / 1TB",
                        "Convertible": true,
                        "OS": "Windows 11 Home"
                    }
                    """
                },
                new
                {
                    ProductId = 5,
                    Specs = """
                    {
                        "Display": "14-inch IPS 2.8K",
                        "CPU": "Intel i5 / i7",
                        "RAM": "8GB / 16GB",
                        "Storage": "512GB / 1TB",
                        "Security": "Fingerprint + TPM 2.0",
                        "OS": "Windows 11 Pro"
                    }
                    """
                },

                // ===== Smartphones =====
                new
                {
                    ProductId = 10,
                    Specs = """
                    {
                        "Display": "6.1-inch OLED 120Hz",
                        "Chip": "Apple A17 Pro",
                        "RAM": "6GB",
                        "Storage": "128GB / 256GB",
                        "Camera": "48MP + 12MP + 12MP",
                        "Battery": "3279mAh",
                        "OS": "iOS 17"
                    }
                    """
                },
                new
                {
                    ProductId = 11,
                    Specs = """
                    {
                        "Display": "6.7-inch Dynamic AMOLED 120Hz",
                        "Chip": "Snapdragon 8 Gen 3",
                        "RAM": "8GB",
                        "Storage": "128GB / 256GB",
                        "Camera": "200MP + 12MP + 10MP",
                        "Battery": "5000mAh",
                        "OS": "Android 14"
                    }
                    """
                },
                new
                {
                    ProductId = 12,
                    Specs = """
                    {
                        "Display": "6.3-inch AMOLED 120Hz",
                        "Chip": "Google Tensor G4",
                        "RAM": "8GB",
                        "Storage": "128GB / 256GB",
                        "Camera": "50MP + 12MP",
                        "Battery": "4700mAh",
                        "OS": "Android 14"
                    }
                    """
                },
                new
                {
                    ProductId = 13,
                    Specs = """
                    {
                        "Display": "6.7-inch AMOLED QHD+",
                        "Chip": "Snapdragon 8 Gen 3",
                        "RAM": "12GB",
                        "Storage": "256GB / 512GB",
                        "Camera": "50MP + 50MP + 50MP (Leica)",
                        "OS": "HyperOS (Android 14)"
                    }
                    """
                },
                new
                {
                    ProductId = 14,
                    Specs = """
                    {
                        "Display": "6.8-inch AMOLED 120Hz",
                        "Chip": "Snapdragon 8 Gen 3",
                        "RAM": "12GB",
                        "Storage": "256GB / 512GB",
                        "Battery": "5400mAh 100W charging",
                        "OS": "OxygenOS 14"
                    }
                    """
                },

                // ===== Tablets =====
                new
                {
                    ProductId = 20,
                    Specs = """
                    {
                        "Display": "12.9-inch Liquid Retina XDR",
                        "Chip": "Apple M2",
                        "RAM": "8GB / 16GB",
                        "Storage": "128GB / 256GB",
                        "OS": "iPadOS 17",
                        "PencilSupport": "Apple Pencil 2"
                    }
                    """
                },
                new
                {
                    ProductId = 21,
                    Specs = """
                    {
                        "Display": "11-inch AMOLED 120Hz",
                        "Chip": "Snapdragon 8 Gen 2",
                        "RAM": "8GB / 12GB",
                        "Storage": "128GB / 256GB",
                        "OS": "Android 14"
                    }
                    """
                },
                new
                {
                    ProductId = 22,
                    Specs = """
                    {
                        "Display": "12.7-inch LCD 144Hz",
                        "Chip": "MediaTek Dimensity 7050",
                        "RAM": "8GB",
                        "Storage": "128GB",
                        "Battery": "10200mAh",
                        "OS": "Android 13"
                    }
                    """
                },

                // ===== Monitors =====
                new
                {
                    ProductId = 30,
                    Specs = """
                    {
                        "Display": "27-inch IPS 4K UHD",
                        "Resolution": "3840x2160",
                        "RefreshRate": "60Hz",
                        "Ports": "HDMI, DisplayPort, USB-C",
                        "ColorGamut": "99% sRGB",
                        "Warranty": "24 months"
                    }
                    """
                },
                new
                {
                    ProductId = 31,
                    Specs = """
                    {
                        "Display": "32-inch VA QHD",
                        "Resolution": "2560x1440",
                        "RefreshRate": "165Hz",
                        "Ports": "HDMI, DisplayPort",
                        "Sync": "G-Sync Compatible"
                    }
                    """
                },

                // ===== Accessories =====
                new
                {
                    ProductId = 40,
                    Specs = """{ "Type": "Wireless", "Layout": "Full-size", "Backlight": "Yes", "Battery": "USB-C rechargeable" }"""
                },
                new
                {
                    ProductId = 41,
                    Specs = """{ "Type": "Mechanical", "Switch": "Razer Green", "Backlight": "RGB", "Connection": "Wired" }"""
                },
                new
                {
                    ProductId = 50,
                    Specs = """{ "Sensor": "Logitech Darkfield", "Connection": "Bluetooth / USB", "Battery": "70 days", "Buttons": 7 }"""
                },
                new
                {
                    ProductId = 51,
                    Specs = """{ "Sensor": "Focus Pro 30K", "Weight": "58g", "Connection": "Wireless", "Battery": "80h" }"""
                },
                new
                {
                    ProductId = 60,
                    Specs = """{ "Type": "Over-ear", "ANC": "Yes", "Battery": "30h", "Charging": "USB-C", "Microphone": "Yes" }"""
                },
                new
                {
                    ProductId = 61,
                    Specs = """{ "Type": "In-ear", "ANC": "Yes", "Battery": "6h + 24h", "Wireless": "Bluetooth 5.3" }"""
                },
                new
                {
                    ProductId = 70,
                    Specs = """{ "Power": "65W", "Ports": "2x USB-C, 1x USB-A", "Material": "GaN", "Input": "100–240V" }"""
                },
                new
                {
                    ProductId = 71,
                    Specs = """{ "Length": "1.5m", "Connector": "USB-C to USB-C", "Material": "Nylon braided", "MaxPower": "100W" }"""
                },
                new
                {
                    ProductId = 80,
                    Specs = """{ "Material": "TPU", "ShockResistant": "Yes", "CompatibleDevices": "iPhone 15" }"""
                },
                new
                {
                    ProductId = 81,
                    Specs = """{ "Material": "Ballistic Nylon", "Fits": "13–15 inch laptops", "WaterResistant": "Yes" }"""
                }
            );
        }
    }
}
