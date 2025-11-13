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
        """
                },
                new
                {
                    ProductId = 2,
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
                new
                {
                    ProductId = 3,
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
                new
                {
                    ProductId = 4,
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
                new
                {
                    ProductId = 5,
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
                new
                {
                    ProductId = 10,
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
                new
                {
                    ProductId = 11,
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
                new
                {
                    ProductId = 12,
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
                new
                {
                    ProductId = 13,
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
                new
                {
                    ProductId = 14,
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
                new
                {
                    ProductId = 20,
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
                new
                {
                    ProductId = 21,
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
                new
                {
                    ProductId = 22,
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
                new
                {
                    ProductId = 30,
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
                new
                {
                    ProductId = 31,
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

                // ===== Accessories =====
                new
                {
                    ProductId = 40,
                    Specs = """
        [
            {"Name":"Type","Value":["Wireless"]},
            {"Name":"Layout","Value":["Full-size"]},
            {"Name":"Backlight","Value":["Yes"]},
            {"Name":"Battery","Value":["USB-C rechargeable"]}
        ]
        """
                },
                new
                {
                    ProductId = 41,
                    Specs = """
        [
            {"Name":"Type","Value":["Mechanical"]},
            {"Name":"Switch","Value":["Razer Green"]},
            {"Name":"Backlight","Value":["RGB"]},
            {"Name":"Connection","Value":["Wired"]}
        ]
        """
                },
                new
                {
                    ProductId = 50,
                    Specs = """
        [
            {"Name":"Sensor","Value":["Logitech Darkfield"]},
            {"Name":"Connection","Value":["Bluetooth","USB"]},
            {"Name":"Battery","Value":["70 days"]},
            {"Name":"Buttons","Value":["7"]}
        ]
        """
                },
                new
                {
                    ProductId = 51,
                    Specs = """
        [
            {"Name":"Sensor","Value":["Focus Pro 30K"]},
            {"Name":"Weight","Value":["58g"]},
            {"Name":"Connection","Value":["Wireless"]},
            {"Name":"Battery","Value":["80h"]}
        ]
        """
                },
                new
                {
                    ProductId = 60,
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
                new
                {
                    ProductId = 61,
                    Specs = """
        [
            {"Name":"Type","Value":["In-ear"]},
            {"Name":"ANC","Value":["Yes"]},
            {"Name":"Battery","Value":["6h","24h"]},
            {"Name":"Wireless","Value":["Bluetooth 5.3"]}
        ]
        """
                },
                new
                {
                    ProductId = 70,
                    Specs = """
        [
            {"Name":"Power","Value":["65W"]},
            {"Name":"Ports","Value":["2x USB-C","1x USB-A"]},
            {"Name":"Material","Value":["GaN"]},
            {"Name":"Input","Value":["100–240V"]}
        ]
        """
                },
                new
                {
                    ProductId = 71,
                    Specs = """
        [
            {"Name":"Length","Value":["1.5m"]},
            {"Name":"Connector","Value":["USB-C to USB-C"]},
            {"Name":"Material","Value":["Nylon braided"]},
            {"Name":"MaxPower","Value":["100W"]}
        ]
        """
                },
                new
                {
                    ProductId = 80,
                    Specs = """
        [
            {"Name":"Material","Value":["TPU"]},
            {"Name":"ShockResistant","Value":["Yes"]},
            {"Name":"CompatibleDevices","Value":["iPhone 15"]}
        ]
        """
                },
                new
                {
                    ProductId = 81,
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
