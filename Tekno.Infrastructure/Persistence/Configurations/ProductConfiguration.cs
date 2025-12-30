using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tekno.Domain.Catalog;

namespace Tekno.Infrastructure.Persistence.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        private static readonly DateTime SeedTime = new DateTime(2025, 1, 15, 10, 0, 0, DateTimeKind.Utc);
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
            builder.Property(p => p.TotalSold).HasDefaultValue(0);
            builder.Property(p => p.AverageRating).HasDefaultValue(0);
            builder.Property(p => p.TotalReviews).HasDefaultValue(0);

            builder.HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId);

            builder.HasOne(p => p.Brand)
                .WithMany(b => b.Products)
                .HasForeignKey(p => p.BrandId);

            //// ========== SEED PRODUCT DATA (VND Pricing) =========
            builder.HasData(
                // ===== Laptops =====
                new { Id = 1, Name = "Dell XPS 13", Slug = "dell-xps-13", BrandId = 1, CategoryId = 1, BasePrice = 25990000M, Status = "available", Overview = "Laptop cao cấp thiết kế siêu mỏng, hiệu suất mạnh mẽ cho doanh nhân.", Description = "Dell XPS 13 với màn hình InfinityEdge, Intel Core thế hệ mới, vỏ nhôm nguyên khối cao cấp.", CreatedAt = SeedTime, UpdatedAt = SeedTime,
                    Specs = """
        [
            {"Name":"Màn hình","Value":["13.4 inch FHD+ (1920x1200)","InfinityEdge","Chống chói"]},
            {"Name":"CPU","Value":["Intel Core i5-1340P","Intel Core i7-1360P"]},
            {"Name":"RAM","Value":["8GB LPDDR5","16GB LPDDR5"]},
            {"Name":"Ổ cứng","Value":["512GB SSD NVMe","1TB SSD NVMe"]},
            {"Name":"Trọng lượng","Value":["1.24 kg"]},
            {"Name":"Pin","Value":["52Wh - Sử dụng 12 giờ"]},
            {"Name":"Hệ điều hành","Value":["Windows 11 Home"]},
            {"Name":"Bảo hành","Value":["12 tháng chính hãng"]}
        ]
        """ },
        
                new { Id = 2, Name = "MacBook Air M2", Slug = "macbook-air-m2", BrandId = 2, CategoryId = 1, BasePrice = 28990000M, Status = "available", Overview = "MacBook Air với chip M2 mạnh mẽ, pin trâu, thiết kế sang trọng.", Description = "MacBook Air M2 13.6 inch, màn hình Liquid Retina, hiệu năng vượt trội, pin 18 giờ.", CreatedAt = SeedTime, UpdatedAt = SeedTime,
                    Specs = """
        [
            {"Name":"Màn hình","Value":["13.6 inch Liquid Retina","2560 x 1664 pixels"]},
            {"Name":"Chip","Value":["Apple M2 8-core CPU","10-core GPU"]},
            {"Name":"RAM","Value":["8GB Unified Memory","16GB Unified Memory"]},
            {"Name":"Ổ cứng","Value":["256GB SSD","512GB SSD"]},
            {"Name":"Pin","Value":["52.6Wh - Lên đến 18 giờ"]},
            {"Name":"Hệ điều hành","Value":["macOS Sonoma"]},
            {"Name":"Trọng lượng","Value":["1.24 kg"]}
        ]
        """
                },
                
                new { Id = 3, Name = "Asus ZenBook 14 OLED", Slug = "asus-zenbook-14-oled", BrandId = 3, CategoryId = 1, BasePrice = 22490000M, Status = "available", Overview = "Ultrabook với màn hình OLED tuyệt đẹp, hiệu năng cao.", Description = "Asus ZenBook 14 OLED màn hình 2.8K, Intel Core i5/i7 thế hệ mới, mỏng nhẹ 1.39kg.", CreatedAt = SeedTime, UpdatedAt = SeedTime,
                    Specs = """
        [
            {"Name":"Màn hình","Value":["14 inch OLED 2.8K (2880x1800)","90Hz","400 nits"]},
            {"Name":"CPU","Value":["Intel Core i5-13500H","Intel Core i7-13700H"]},
            {"Name":"RAM","Value":["8GB LPDDR5","16GB LPDDR5"]},
            {"Name":"Ổ cứng","Value":["512GB SSD PCIe 4.0","1TB SSD PCIe 4.0"]},
            {"Name":"Hệ điều hành","Value":["Windows 11 Home"]},
            {"Name":"Trọng lượng","Value":["1.39 kg"]}
        ]
        """
                },
                
                new { Id = 4, Name = "HP Spectre x360 14", Slug = "hp-spectre-x360-14", BrandId = 4, CategoryId = 1, BasePrice = 42990000M, Status = "available", Overview = "Laptop xoay 360° cao cấp, màn hình cảm ứng, thiết kế sang trọng.", Description = "HP Spectre x360 14 màn hình OLED cảm ứng, Intel Core i7, hỗ trợ bút stylus.", CreatedAt = SeedTime, UpdatedAt = SeedTime,
                    Specs = """
        [
            {"Name":"Màn hình","Value":["13.5 inch OLED cảm ứng","3:2 (3000x2000)","400 nits"]},
            {"Name":"CPU","Value":["Intel Core i5-1335U","Intel Core i7-1355U"]},
            {"Name":"RAM","Value":["8GB LPDDR4x","16GB LPDDR4x"]},
            {"Name":"Ổ cứng","Value":["512GB SSD NVMe","1TB SSD NVMe"]},
            {"Name":"Xoay 360°","Value":["Có - Chế độ Tablet"]},
            {"Name":"Hệ điều hành","Value":["Windows 11 Home"]}
        ]
        """
                },
                
                new { Id = 5, Name = "Lenovo ThinkPad X1 Carbon Gen 11", Slug = "thinkpad-x1-carbon-gen11", BrandId = 5, CategoryId = 1, BasePrice = 48990000M, Status = "available", Overview = "Laptop doanh nghiệp cao cấp, bền bỉ, bảo mật tốt.", Description = "ThinkPad X1 Carbon Gen 11 với màn hình 2.8K, Intel Core thế hệ 13, vỏ carbon siêu nhẹ.", CreatedAt = SeedTime, UpdatedAt = SeedTime,
                    Specs = """
        [
            {"Name":"Màn hình","Value":["14 inch 2.8K (2880x1800)","Low Blue Light"]},
            {"Name":"CPU","Value":["Intel Core i5-1335U","Intel Core i7-1355U"]},
            {"Name":"RAM","Value":["16GB LPDDR5"]},
            {"Name":"Ổ cứng","Value":["512GB SSD NVMe","1TB SSD NVMe"]},
            {"Name":"Bảo mật","Value":["Vân tay","TPM 2.0","Webcam Privacy Shutter"]},
            {"Name":"Hệ điều hành","Value":["Windows 11 Pro"]}
        ]
        """
                },

                // ===== Smartphones =====
                new { Id = 10, Name = "iPhone 15 Pro Max", Slug = "iphone-15-pro-max", BrandId = 2, CategoryId = 2, BasePrice = 33990000M, Status = "available", Overview = "iPhone cao cấp nhất với khung Titanium, camera 48MP, chip A17 Pro.", Description = "iPhone 15 Pro Max 256GB với Dynamic Island, Always-On Display, camera zoom 5x.", CreatedAt = SeedTime, UpdatedAt = SeedTime,
                    Specs = """
        [
            {"Name":"Màn hình","Value":["6.7 inch Super Retina XDR OLED","ProMotion 120Hz","Always-On"]},
            {"Name":"Chip","Value":["Apple A17 Pro 3nm"]},
            {"Name":"RAM","Value":["8GB"]},
            {"Name":"Bộ nhớ","Value":["256GB","512GB","1TB"]},
            {"Name":"Camera","Value":["48MP Main","12MP Ultra Wide","12MP Telephoto 5x"]},
            {"Name":"Pin","Value":["4422mAh","29 giờ video"]},
            {"Name":"Hệ điều hành","Value":["iOS 17"]}
        ]
        """
                },
                
                new { Id = 11, Name = "Samsung Galaxy S24 Ultra", Slug = "samsung-galaxy-s24-ultra", BrandId = 6, CategoryId = 2, BasePrice = 29990000M, Status = "available", Overview = "Android flagship với S Pen, camera 200MP, màn hình Dynamic AMOLED 2X.", Description = "Galaxy S24 Ultra 256GB Snapdragon 8 Gen 3, màn hình 6.8 inch, pin 5000mAh.", CreatedAt = SeedTime, UpdatedAt = SeedTime,
                    Specs = """
        [
            {"Name":"Màn hình","Value":["6.8 inch Dynamic AMOLED 2X","QHD+ (3120x1440)","120Hz"]},
            {"Name":"Chip","Value":["Snapdragon 8 Gen 3 for Galaxy"]},
            {"Name":"RAM","Value":["12GB"]},
            {"Name":"Bộ nhớ","Value":["256GB","512GB","1TB"]},
            {"Name":"Camera","Value":["200MP chính","12MP Ultra Wide","50MP Periscope 5x","10MP Telephoto 3x"]},
            {"Name":"Pin","Value":["5000mAh - Sạc nhanh 45W"]},
            {"Name":"Hệ điều hành","Value":["Android 14 - One UI 6.1"]}
        ]
        """
                },
                
                new { Id = 12, Name = "Google Pixel 8 Pro", Slug = "google-pixel-8-pro", BrandId = 7, CategoryId = 2, BasePrice = 24990000M, Status = "available", Overview = "Điện thoại AI với camera thông minh nhất, cập nhật 7 năm.", Description = "Pixel 8 Pro 256GB Tensor G3, camera AI ma thuật, màn hình LTPO OLED 120Hz.", CreatedAt = SeedTime, UpdatedAt = SeedTime,
                    Specs = """
        [
            {"Name":"Màn hình","Value":["6.7 inch LTPO OLED","QHD+ (3120x1440)","120Hz"]},
            {"Name":"Chip","Value":["Google Tensor G3"]},
            {"Name":"RAM","Value":["12GB LPDDR5X"]},
            {"Name":"Bộ nhớ","Value":["128GB","256GB","512GB"]},
            {"Name":"Camera","Value":["50MP Main","48MP Ultra Wide","48MP Telephoto 5x"]},
            {"Name":"Pin","Value":["5050mAh - Sạc nhanh 30W"]},
            {"Name":"Hệ điều hành","Value":["Android 14 - Cập nhật 7 năm"]}
        ]
        """
                },
                
                new { Id = 13, Name = "Xiaomi 14", Slug = "xiaomi-14", BrandId = 8, CategoryId = 2, BasePrice = 18990000M, Status = "available", Overview = "Flagship Xiaomi với camera Leica, hiệu năng mạnh mẽ, giá tốt.", Description = "Xiaomi 14 Snapdragon 8 Gen 3, màn hình AMOLED 120Hz, camera Leica Summilux.", CreatedAt = SeedTime, UpdatedAt = SeedTime,
                    Specs = """
        [
            {"Name":"Màn hình","Value":["6.36 inch AMOLED","FHD+ (2670x1200)","120Hz"]},
            {"Name":"Chip","Value":["Snapdragon 8 Gen 3"]},
            {"Name":"RAM","Value":["12GB"]},
            {"Name":"Bộ nhớ","Value":["256GB","512GB"]},
            {"Name":"Camera","Value":["50MP Leica Main","50MP Ultra Wide","50MP Telephoto 3.2x"]},
            {"Name":"Hệ điều hành","Value":["HyperOS (Android 14)"]}
        ]
        """
                },
                
                new { Id = 14, Name = "OnePlus 12", Slug = "oneplus-12", BrandId = 9, CategoryId = 2, BasePrice = 19990000M, Status = "available", Overview = "Flagship killer với sạc nhanh 100W, màn hình LTPO AMOLED.", Description = "OnePlus 12 Snapdragon 8 Gen 3, màn hình 120Hz, pin 5400mAh sạc siêu nhanh.", CreatedAt = SeedTime, UpdatedAt = SeedTime,
                    Specs = """
        [
            {"Name":"Màn hình","Value":["6.82 inch LTPO AMOLED","QHD+ (3168x1440)","120Hz"]},
            {"Name":"Chip","Value":["Snapdragon 8 Gen 3"]},
            {"Name":"RAM","Value":["12GB","16GB"]},
            {"Name":"Bộ nhớ","Value":["256GB","512GB"]},
            {"Name":"Pin","Value":["5400mAh - Sạc nhanh 100W SuperVOOC"]},
            {"Name":"Hệ điều hành","Value":["OxygenOS 14 (Android 14)"]}
        ]
        """
                },

                // ===== Tablets =====
                new { Id = 20, Name = "iPad Pro M2 11 inch", Slug = "ipad-pro-m2-11", BrandId = 2, CategoryId = 3, BasePrice = 24990000M, Status = "available", Overview = "Máy tính bảng mạnh nhất với chip M2, màn hình Liquid Retina XDR.", Description = "iPad Pro 11 inch M2 với ProMotion 120Hz, hỗ trợ Apple Pencil 2 và Magic Keyboard.", CreatedAt = SeedTime, UpdatedAt = SeedTime,
                    Specs = """
        [
            {"Name":"Màn hình","Value":["11 inch Liquid Retina","IPS LCD (2388x1668)","ProMotion 120Hz"]},
            {"Name":"Chip","Value":["Apple M2 8-core CPU","10-core GPU"]},
            {"Name":"RAM","Value":["8GB","16GB"]},
            {"Name":"Bộ nhớ","Value":["128GB","256GB","512GB"]},
            {"Name":"Hệ điều hành","Value":["iPadOS 17"]},
            {"Name":"Phụ kiện","Value":["Apple Pencil 2","Magic Keyboard"]}
        ]
        """
                },
                
                new { Id = 21, Name = "Samsung Galaxy Tab S9", Slug = "samsung-galaxy-tab-s9", BrandId = 6, CategoryId = 3, BasePrice = 19990000M, Status = "available", Overview = "Tablet Android cao cấp với màn hình Dynamic AMOLED 2X, S Pen.", Description = "Galaxy Tab S9 11 inch Snapdragon 8 Gen 2, kháng nước IP68, S Pen kèm theo.", CreatedAt = SeedTime, UpdatedAt = SeedTime,
                    Specs = """
        [
            {"Name":"Màn hình","Value":["11 inch Dynamic AMOLED 2X","WQXGA (2560x1600)","120Hz"]},
            {"Name":"Chip","Value":["Snapdragon 8 Gen 2"]},
            {"Name":"RAM","Value":["8GB","12GB"]},
            {"Name":"Bộ nhớ","Value":["128GB","256GB"]},
            {"Name":"Hệ điều hành","Value":["Android 13 - One UI 5.1"]},
            {"Name":"Kháng nước","Value":["IP68"]}
        ]
        """
                },
                
                new { Id = 22, Name = "Xiaomi Pad 6", Slug = "xiaomi-pad-6", BrandId = 8, CategoryId = 3, BasePrice = 8990000M, Status = "available", Overview = "Tablet tầm trung với màn hình 144Hz, loa 4 kênh Dolby Atmos.", Description = "Xiaomi Pad 6 11 inch Snapdragon 870, màn hình LCD 144Hz, pin 8840mAh.", CreatedAt = SeedTime, UpdatedAt = SeedTime,
                    Specs = """
        [
            {"Name":"Màn hình","Value":["11 inch LCD","2.8K (2880x1800)","144Hz"]},
            {"Name":"Chip","Value":["Snapdragon 870"]},
            {"Name":"RAM","Value":["6GB","8GB"]},
            {"Name":"Bộ nhớ","Value":["128GB","256GB"]},
            {"Name":"Pin","Value":["8840mAh - Sạc nhanh 33W"]},
            {"Name":"Hệ điều hành","Value":["MIUI Pad 14"]}
        ]
        """
                },

                // ===== Monitors =====
                new { Id = 30, Name = "Dell UltraSharp U2723DE", Slug = "dell-ultrasharp-u2723de", BrandId = 1, CategoryId = 8, BasePrice = 17990000M, Status = "available", Overview = "Màn hình 27 inch QHD IPS, độ phủ màu 95% DCI-P3, USB-C 90W.", Description = "Dell U2723DE 27 inch QHD IPS Black, độ chính xác màu cao cho thiết kế đồ họa.", CreatedAt = SeedTime, UpdatedAt = SeedTime,
                    Specs = """
        [
            {"Name":"Kích thước","Value":["27 inch"]},
            {"Name":"Độ phân giải","Value":["QHD 2560x1440"]},
            {"Name":"Tấm nền","Value":["IPS Black"]},
            {"Name":"Tần số quét","Value":["60Hz"]},
            {"Name":"Cổng kết nối","Value":["HDMI 2.0","DisplayPort 1.4","USB-C 90W"]},
            {"Name":"Độ phủ màu","Value":["95% DCI-P3","100% sRGB"]},
            {"Name":"Bảo hành","Value":["36 tháng"]}
        ]
        """
                },
                
                new { Id = 31, Name = "LG UltraGear 27GN800-B", Slug = "lg-ultragear-27gn800", BrandId = 10, CategoryId = 8, BasePrice = 8490000M, Status = "available", Overview = "Màn hình gaming 27 inch QHD 144Hz, tấm nền IPS Nano, G-Sync.", Description = "LG 27GN800 27 inch QHD IPS 144Hz, thời gian phản hồi 1ms, HDR10.", CreatedAt = SeedTime, UpdatedAt = SeedTime,
                    Specs = """
        [
            {"Name":"Kích thước","Value":["27 inch"]},
            {"Name":"Độ phân giải","Value":["QHD 2560x1440"]},
            {"Name":"Tấm nền","Value":["IPS Nano Color"]},
            {"Name":"Tần số quét","Value":["144Hz"]},
            {"Name":"Cổng kết nối","Value":["HDMI 2.0 x2","DisplayPort 1.4"]},
            {"Name":"Đồng bộ","Value":["G-Sync Compatible","FreeSync Premium"]}
        ]
        """
                },

                // ===== Keyboards =====
                new { Id = 40, Name = "Logitech MX Keys", Slug = "logitech-mx-keys", BrandId = 11, CategoryId = 13, BasePrice = 2990000M, Status = "available", Overview = "Bàn phím không dây cao cấp cho văn phòng, đèn nền thông minh.", Description = "Logitech MX Keys với phím Perfect Stroke, kết nối multi-device, pin 10 ngày.", CreatedAt = SeedTime, UpdatedAt = SeedTime,
                    Specs = """
        [
            {"Name":"Kiểu kết nối","Value":["Bluetooth","USB Receiver"]},
            {"Name":"Layout","Value":["Full-size"]},
            {"Name":"Đèn nền","Value":["Có - Tự động điều chỉnh"]},
            {"Name":"Pin","Value":["Sạc USB-C - 10 ngày với đèn"]},
            {"Name":"Multi-device","Value":["3 thiết bị"]}
        ]
        """
                },
                
                new { Id = 41, Name = "Razer BlackWidow V4 Pro", Slug = "razer-blackwidow-v4-pro", BrandId = 12, CategoryId = 13, BasePrice = 5990000M, Status = "available", Overview = "Bàn phím cơ gaming cao cấp RGB Chroma, switch Green Clicky.", Description = "Razer BlackWidow V4 Pro với Command Dial, switch cơ học, RGB per-key.", CreatedAt = SeedTime, UpdatedAt = SeedTime,
                    Specs = """
        [
            {"Name":"Kiểu","Value":["Cơ học"]},
            {"Name":"Switch","Value":["Razer Green Clicky"]},
            {"Name":"Đèn nền","Value":["RGB Chroma per-key"]},
            {"Name":"Kết nối","Value":["USB Type-C có dây"]},
            {"Name":"Tính năng","Value":["Command Dial","Media keys"]}
        ]
        """
                },

                // ===== Mouse =====
                new { Id = 50, Name = "Logitech MX Master 3S", Slug = "logitech-mx-master-3s", BrandId = 11, CategoryId = 14, BasePrice = 2490000M, Status = "available", Overview = "Chuột không dây ergonomic cho năng suất cao, sensor 8K DPI.", Description = "Logitech MX Master 3S với MagSpeed Wheel, kết nối multi-device, pin 70 ngày.", CreatedAt = SeedTime, UpdatedAt = SeedTime,
                    Specs = """
        [
            {"Name":"Sensor","Value":["Darkfield 8000 DPI"]},
            {"Name":"Kết nối","Value":["Bluetooth","USB Receiver"]},
            {"Name":"Pin","Value":["70 ngày"]},
            {"Name":"Số nút","Value":["7 nút có thể tùy chỉnh"]},
            {"Name":"Multi-device","Value":["3 thiết bị"]}
        ]
        """
                },
                
                new { Id = 51, Name = "Razer Viper V2 Pro", Slug = "razer-viper-v2-pro", BrandId = 12, CategoryId = 14, BasePrice = 3990000M, Status = "available", Overview = "Chuột gaming không dây siêu nhẹ 58g, sensor Focus Pro 30K.", Description = "Razer Viper V2 Pro với HyperSpeed Wireless, optical switch Gen 3, pin 80 giờ.", CreatedAt = SeedTime, UpdatedAt = SeedTime,
                    Specs = """
        [
            {"Name":"Sensor","Value":["Focus Pro 30K DPI"]},
            {"Name":"Trọng lượng","Value":["58g"]},
            {"Name":"Kết nối","Value":["HyperSpeed Wireless 2.4GHz"]},
            {"Name":"Pin","Value":["80 giờ"]},
            {"Name":"Switch","Value":["Optical Gen 3"]}
        ]
        """
                },

                // ===== Headphones =====
                new { Id = 60, Name = "Sony WH-1000XM5", Slug = "sony-wh-1000xm5", BrandId = 13, CategoryId = 15, BasePrice = 8990000M, Status = "available", Overview = "Tai nghe chống ồn hàng đầu, chất âm Hi-Res, pin 30 giờ.", Description = "Sony WH-1000XM5 với ANC thế hệ mới, 8 micro, hỗ trợ LDAC và DSEE Extreme.", CreatedAt = SeedTime, UpdatedAt = SeedTime,
                    Specs = """
        [
            {"Name":"Kiểu","Value":["Over-ear"]},
            {"Name":"ANC","Value":["Có - 8 micro"]},
            {"Name":"Pin","Value":["30 giờ"]},
            {"Name":"Sạc","Value":["USB-C - Sạc nhanh 3 phút = 3 giờ"]},
            {"Name":"Codec","Value":["LDAC","AAC","SBC"]},
            {"Name":"Mic","Value":["Có - AI Noise Reduction"]}
        ]
        """
                },
                
                new { Id = 61, Name = "Apple AirPods Pro 2", Slug = "apple-airpods-pro-2", BrandId = 2, CategoryId = 15, BasePrice = 6490000M, Status = "available", Overview = "Tai nghe true wireless với ANC tốt nhất, chip H2, sạc MagSafe.", Description = "AirPods Pro 2 với Adaptive Audio, Transparency Mode, case sạc MagSafe và loa tìm kiếm.", CreatedAt = SeedTime, UpdatedAt = SeedTime,
                    Specs = """
        [
            {"Name":"Kiểu","Value":["In-ear True Wireless"]},
            {"Name":"Chip","Value":["Apple H2"]},
            {"Name":"ANC","Value":["Có - Adaptive"]},
            {"Name":"Pin","Value":["6 giờ tai nghe","30 giờ với case"]},
            {"Name":"Sạc","Value":["MagSafe","USB-C","Apple Watch charger"]},
            {"Name":"Kháng nước","Value":["IPX4"]}
        ]
        """
                },

                // ===== Chargers & Cables =====
                new { Id = 70, Name = "Anker 747 GaNPrime 150W", Slug = "anker-747-ganprime-150w", BrandId = 14, CategoryId = 16, BasePrice = 2490000M, Status = "available", Overview = "Sạc nhanh GaN 150W, 4 cổng (3 USB-C + 1 USB-A), sạc cùng lúc 4 thiết bị.", Description = "Anker 747 Charger với công nghệ GaN, ActiveShield 2.0, sạc laptop, điện thoại đồng thời.", CreatedAt = SeedTime, UpdatedAt = SeedTime,
                    Specs = """
        [
            {"Name":"Công suất","Value":["150W tối đa"]},
            {"Name":"Cổng","Value":["3x USB-C (100W+45W+45W)","1x USB-A (22.5W)"]},
            {"Name":"Công nghệ","Value":["GaN III","ActiveShield 2.0"]},
            {"Name":"Điện áp","Value":["100-240V"]},
            {"Name":"Bảo vệ","Value":["Quá nhiệt","Quá dòng","Ngắn mạch"]}
        ]
        """
                },
                
                new { Id = 71, Name = "Baseus 100W USB-C Cable 2m", Slug = "baseus-100w-usb-c-cable", BrandId = 15, CategoryId = 16, BasePrice = 290000M, Status = "available", Overview = "Cáp sạc nhanh USB-C to USB-C 100W, dây bện nylon bền, dài 2m.", Description = "Baseus 100W Cable với E-Marker chip, hỗ trợ PD 3.0, QC 4.0, data transfer 480Mbps.", CreatedAt = SeedTime, UpdatedAt = SeedTime,
                    Specs = """
        [
            {"Name":"Chiều dài","Value":["2 mét"]},
            {"Name":"Đầu nối","Value":["USB-C to USB-C"]},
            {"Name":"Vật liệu","Value":["Nylon braided + Zinc alloy"]},
            {"Name":"Công suất","Value":["100W (20V/5A)"]},
            {"Name":"Tốc độ dữ liệu","Value":["480Mbps"]}
        ]
        """
                },

                // ===== Cases & Covers =====
                new { Id = 80, Name = "Spigen Rugged Armor iPhone 15 Pro", Slug = "spigen-rugged-armor-iphone-15-pro", BrandId = 16, CategoryId = 17, BasePrice = 490000M, Status = "available", Overview = "Ốp lưng chống sốc cho iPhone 15 Pro, thiết kế Carbon Fiber.", Description = "Spigen Rugged Armor với vật liệu TPU mềm, viền chống trầy, tản nhiệt tốt.", CreatedAt = SeedTime, UpdatedAt = SeedTime,
                    Specs = """
        [
            {"Name":"Vật liệu","Value":["TPU dẻo"]},
            {"Name":"Chống sốc","Value":["Có - Air Cushion Technology"]},
            {"Name":"Tương thích","Value":["iPhone 15 Pro (6.1 inch)"]},
            {"Name":"Thiết kế","Value":["Carbon Fiber texture"]},
            {"Name":"Viền camera","Value":["Nổi bảo vệ camera"]}
        ]
        """
                },
                
                new { Id = 81, Name = "Tomtoc Laptop Sleeve 13-14 inch", Slug = "tomtoc-laptop-sleeve-13-14", BrandId = 17, CategoryId = 17, BasePrice = 790000M, Status = "available", Overview = "Túi chống sốc laptop 13-14 inch, lót CornerArmor, chống nước.", Description = "Tomtoc 360° Protective Laptop Sleeve với CornerArmor bảo vệ góc, YKK zipper.", CreatedAt = SeedTime, UpdatedAt = SeedTime,
                    Specs = """
        [
            {"Name":"Vật liệu","Value":["Ballistic Nylon 1680D"]},
            {"Name":"Kích thước","Value":["13-14 inch laptops"]},
            {"Name":"Chống nước","Value":["Có - Lớp phủ water-resistant"]},
            {"Name":"Lót đệm","Value":["CornerArmor + Foam padding"]},
            {"Name":"Khóa kéo","Value":["YKK RC Fuse"]}
        ]
        """
                }
            );
        }
    }
}
