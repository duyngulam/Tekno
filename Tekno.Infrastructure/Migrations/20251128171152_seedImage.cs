using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Tekno.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class seedImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "category",
                keyColumn: "Id",
                keyValue: 4,
                column: "ImageUrl",
                value: "https://res.cloudinary.com/dwa3wh9yb/image/upload/v1764337712/adapter-20w-apple-5_1_1_odasww.webp");

            migrationBuilder.UpdateData(
                table: "category",
                keyColumn: "Id",
                keyValue: 13,
                column: "IconPath",
                value: "https://res.cloudinary.com/dwa3wh9yb/image/upload/v1764336081/keyboard_k2vqvu.svg");

            migrationBuilder.UpdateData(
                table: "category",
                keyColumn: "Id",
                keyValue: 15,
                column: "ImageUrl",
                value: "https://res.cloudinary.com/dwa3wh9yb/image/upload/v1764337713/headphone_qn6mqd.jpg");

            migrationBuilder.UpdateData(
                table: "product",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "BasePrice", "CreatedAt", "Description", "Overview", "Specs", "UpdatedAt" },
                values: new object[] { 25990000m, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), "Dell XPS 13 với màn hình InfinityEdge, Intel Core thế hệ mới, vỏ nhôm nguyên khối cao cấp.", "Laptop cao cấp thiết kế siêu mỏng, hiệu suất mạnh mẽ cho doanh nhân.", "[\r\n    {\"Name\":\"Màn hình\",\"Value\":[\"13.4 inch FHD+ (1920x1200)\",\"InfinityEdge\",\"Chống chói\"]},\r\n    {\"Name\":\"CPU\",\"Value\":[\"Intel Core i5-1340P\",\"Intel Core i7-1360P\"]},\r\n    {\"Name\":\"RAM\",\"Value\":[\"8GB LPDDR5\",\"16GB LPDDR5\"]},\r\n    {\"Name\":\"Ổ cứng\",\"Value\":[\"512GB SSD NVMe\",\"1TB SSD NVMe\"]},\r\n    {\"Name\":\"Trọng lượng\",\"Value\":[\"1.24 kg\"]},\r\n    {\"Name\":\"Pin\",\"Value\":[\"52Wh - Sử dụng 12 giờ\"]},\r\n    {\"Name\":\"Hệ điều hành\",\"Value\":[\"Windows 11 Home\"]},\r\n    {\"Name\":\"Bảo hành\",\"Value\":[\"12 tháng chính hãng\"]}\r\n]", new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "product",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "BasePrice", "CreatedAt", "Description", "Name", "Overview", "Slug", "Specs", "UpdatedAt" },
                values: new object[] { 28990000m, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), "MacBook Air M2 13.6 inch, màn hình Liquid Retina, hiệu năng vượt trội, pin 18 giờ.", "MacBook Air M2", "MacBook Air với chip M2 mạnh mẽ, pin trâu, thiết kế sang trọng.", "macbook-air-m2", "[\r\n    {\"Name\":\"Màn hình\",\"Value\":[\"13.6 inch Liquid Retina\",\"2560 x 1664 pixels\"]},\r\n    {\"Name\":\"Chip\",\"Value\":[\"Apple M2 8-core CPU\",\"10-core GPU\"]},\r\n    {\"Name\":\"RAM\",\"Value\":[\"8GB Unified Memory\",\"16GB Unified Memory\"]},\r\n    {\"Name\":\"Ổ cứng\",\"Value\":[\"256GB SSD\",\"512GB SSD\"]},\r\n    {\"Name\":\"Pin\",\"Value\":[\"52.6Wh - Lên đến 18 giờ\"]},\r\n    {\"Name\":\"Hệ điều hành\",\"Value\":[\"macOS Sonoma\"]},\r\n    {\"Name\":\"Trọng lượng\",\"Value\":[\"1.24 kg\"]}\r\n]", new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "product",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "BasePrice", "CreatedAt", "Description", "Name", "Overview", "Slug", "Specs", "UpdatedAt" },
                values: new object[] { 22490000m, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), "Asus ZenBook 14 OLED màn hình 2.8K, Intel Core i5/i7 thế hệ mới, mỏng nhẹ 1.39kg.", "Asus ZenBook 14 OLED", "Ultrabook với màn hình OLED tuyệt đẹp, hiệu năng cao.", "asus-zenbook-14-oled", "[\r\n    {\"Name\":\"Màn hình\",\"Value\":[\"14 inch OLED 2.8K (2880x1800)\",\"90Hz\",\"400 nits\"]},\r\n    {\"Name\":\"CPU\",\"Value\":[\"Intel Core i5-13500H\",\"Intel Core i7-13700H\"]},\r\n    {\"Name\":\"RAM\",\"Value\":[\"8GB LPDDR5\",\"16GB LPDDR5\"]},\r\n    {\"Name\":\"Ổ cứng\",\"Value\":[\"512GB SSD PCIe 4.0\",\"1TB SSD PCIe 4.0\"]},\r\n    {\"Name\":\"Hệ điều hành\",\"Value\":[\"Windows 11 Home\"]},\r\n    {\"Name\":\"Trọng lượng\",\"Value\":[\"1.39 kg\"]}\r\n]", new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "product",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "BasePrice", "CreatedAt", "Description", "Name", "Overview", "Slug", "Specs", "UpdatedAt" },
                values: new object[] { 42990000m, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), "HP Spectre x360 14 màn hình OLED cảm ứng, Intel Core i7, hỗ trợ bút stylus.", "HP Spectre x360 14", "Laptop xoay 360° cao cấp, màn hình cảm ứng, thiết kế sang trọng.", "hp-spectre-x360-14", "[\r\n    {\"Name\":\"Màn hình\",\"Value\":[\"13.5 inch OLED cảm ứng\",\"3:2 (3000x2000)\",\"400 nits\"]},\r\n    {\"Name\":\"CPU\",\"Value\":[\"Intel Core i5-1335U\",\"Intel Core i7-1355U\"]},\r\n    {\"Name\":\"RAM\",\"Value\":[\"8GB LPDDR4x\",\"16GB LPDDR4x\"]},\r\n    {\"Name\":\"Ổ cứng\",\"Value\":[\"512GB SSD NVMe\",\"1TB SSD NVMe\"]},\r\n    {\"Name\":\"Xoay 360°\",\"Value\":[\"Có - Chế độ Tablet\"]},\r\n    {\"Name\":\"Hệ điều hành\",\"Value\":[\"Windows 11 Home\"]}\r\n]", new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "product",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "BasePrice", "CreatedAt", "Description", "Name", "Overview", "Slug", "Specs", "UpdatedAt" },
                values: new object[] { 48990000m, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), "ThinkPad X1 Carbon Gen 11 với màn hình 2.8K, Intel Core thế hệ 13, vỏ carbon siêu nhẹ.", "Lenovo ThinkPad X1 Carbon Gen 11", "Laptop doanh nghiệp cao cấp, bền bỉ, bảo mật tốt.", "thinkpad-x1-carbon-gen11", "[\r\n    {\"Name\":\"Màn hình\",\"Value\":[\"14 inch 2.8K (2880x1800)\",\"Low Blue Light\"]},\r\n    {\"Name\":\"CPU\",\"Value\":[\"Intel Core i5-1335U\",\"Intel Core i7-1355U\"]},\r\n    {\"Name\":\"RAM\",\"Value\":[\"16GB LPDDR5\"]},\r\n    {\"Name\":\"Ổ cứng\",\"Value\":[\"512GB SSD NVMe\",\"1TB SSD NVMe\"]},\r\n    {\"Name\":\"Bảo mật\",\"Value\":[\"Vân tay\",\"TPM 2.0\",\"Webcam Privacy Shutter\"]},\r\n    {\"Name\":\"Hệ điều hành\",\"Value\":[\"Windows 11 Pro\"]}\r\n]", new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "product",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "BasePrice", "CreatedAt", "Description", "Name", "Overview", "Slug", "Specs", "UpdatedAt" },
                values: new object[] { 33990000m, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), "iPhone 15 Pro Max 256GB với Dynamic Island, Always-On Display, camera zoom 5x.", "iPhone 15 Pro Max", "iPhone cao cấp nhất với khung Titanium, camera 48MP, chip A17 Pro.", "iphone-15-pro-max", "[\r\n    {\"Name\":\"Màn hình\",\"Value\":[\"6.7 inch Super Retina XDR OLED\",\"ProMotion 120Hz\",\"Always-On\"]},\r\n    {\"Name\":\"Chip\",\"Value\":[\"Apple A17 Pro 3nm\"]},\r\n    {\"Name\":\"RAM\",\"Value\":[\"8GB\"]},\r\n    {\"Name\":\"Bộ nhớ\",\"Value\":[\"256GB\",\"512GB\",\"1TB\"]},\r\n    {\"Name\":\"Camera\",\"Value\":[\"48MP Main\",\"12MP Ultra Wide\",\"12MP Telephoto 5x\"]},\r\n    {\"Name\":\"Pin\",\"Value\":[\"4422mAh\",\"29 giờ video\"]},\r\n    {\"Name\":\"Hệ điều hành\",\"Value\":[\"iOS 17\"]}\r\n]", new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "product",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "BasePrice", "CreatedAt", "Description", "Name", "Overview", "Slug", "Specs", "UpdatedAt" },
                values: new object[] { 29990000m, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), "Galaxy S24 Ultra 256GB Snapdragon 8 Gen 3, màn hình 6.8 inch, pin 5000mAh.", "Samsung Galaxy S24 Ultra", "Android flagship với S Pen, camera 200MP, màn hình Dynamic AMOLED 2X.", "samsung-galaxy-s24-ultra", "[\r\n    {\"Name\":\"Màn hình\",\"Value\":[\"6.8 inch Dynamic AMOLED 2X\",\"QHD+ (3120x1440)\",\"120Hz\"]},\r\n    {\"Name\":\"Chip\",\"Value\":[\"Snapdragon 8 Gen 3 for Galaxy\"]},\r\n    {\"Name\":\"RAM\",\"Value\":[\"12GB\"]},\r\n    {\"Name\":\"Bộ nhớ\",\"Value\":[\"256GB\",\"512GB\",\"1TB\"]},\r\n    {\"Name\":\"Camera\",\"Value\":[\"200MP chính\",\"12MP Ultra Wide\",\"50MP Periscope 5x\",\"10MP Telephoto 3x\"]},\r\n    {\"Name\":\"Pin\",\"Value\":[\"5000mAh - Sạc nhanh 45W\"]},\r\n    {\"Name\":\"Hệ điều hành\",\"Value\":[\"Android 14 - One UI 6.1\"]}\r\n]", new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "product",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "BasePrice", "CreatedAt", "Description", "Name", "Overview", "Slug", "Specs", "UpdatedAt" },
                values: new object[] { 24990000m, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), "Pixel 8 Pro 256GB Tensor G3, camera AI ma thuật, màn hình LTPO OLED 120Hz.", "Google Pixel 8 Pro", "Điện thoại AI với camera thông minh nhất, cập nhật 7 năm.", "google-pixel-8-pro", "[\r\n    {\"Name\":\"Màn hình\",\"Value\":[\"6.7 inch LTPO OLED\",\"QHD+ (3120x1440)\",\"120Hz\"]},\r\n    {\"Name\":\"Chip\",\"Value\":[\"Google Tensor G3\"]},\r\n    {\"Name\":\"RAM\",\"Value\":[\"12GB LPDDR5X\"]},\r\n    {\"Name\":\"Bộ nhớ\",\"Value\":[\"128GB\",\"256GB\",\"512GB\"]},\r\n    {\"Name\":\"Camera\",\"Value\":[\"50MP Main\",\"48MP Ultra Wide\",\"48MP Telephoto 5x\"]},\r\n    {\"Name\":\"Pin\",\"Value\":[\"5050mAh - Sạc nhanh 30W\"]},\r\n    {\"Name\":\"Hệ điều hành\",\"Value\":[\"Android 14 - Cập nhật 7 năm\"]}\r\n]", new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "product",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "BasePrice", "CreatedAt", "Description", "Name", "Overview", "Slug", "Specs", "UpdatedAt" },
                values: new object[] { 18990000m, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), "Xiaomi 14 Snapdragon 8 Gen 3, màn hình AMOLED 120Hz, camera Leica Summilux.", "Xiaomi 14", "Flagship Xiaomi với camera Leica, hiệu năng mạnh mẽ, giá tốt.", "xiaomi-14", "[\r\n    {\"Name\":\"Màn hình\",\"Value\":[\"6.36 inch AMOLED\",\"FHD+ (2670x1200)\",\"120Hz\"]},\r\n    {\"Name\":\"Chip\",\"Value\":[\"Snapdragon 8 Gen 3\"]},\r\n    {\"Name\":\"RAM\",\"Value\":[\"12GB\"]},\r\n    {\"Name\":\"Bộ nhớ\",\"Value\":[\"256GB\",\"512GB\"]},\r\n    {\"Name\":\"Camera\",\"Value\":[\"50MP Leica Main\",\"50MP Ultra Wide\",\"50MP Telephoto 3.2x\"]},\r\n    {\"Name\":\"Hệ điều hành\",\"Value\":[\"HyperOS (Android 14)\"]}\r\n]", new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "product",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "BasePrice", "CreatedAt", "Description", "Overview", "Specs", "UpdatedAt" },
                values: new object[] { 19990000m, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), "OnePlus 12 Snapdragon 8 Gen 3, màn hình 120Hz, pin 5400mAh sạc siêu nhanh.", "Flagship killer với sạc nhanh 100W, màn hình LTPO AMOLED.", "[\r\n    {\"Name\":\"Màn hình\",\"Value\":[\"6.82 inch LTPO AMOLED\",\"QHD+ (3168x1440)\",\"120Hz\"]},\r\n    {\"Name\":\"Chip\",\"Value\":[\"Snapdragon 8 Gen 3\"]},\r\n    {\"Name\":\"RAM\",\"Value\":[\"12GB\",\"16GB\"]},\r\n    {\"Name\":\"Bộ nhớ\",\"Value\":[\"256GB\",\"512GB\"]},\r\n    {\"Name\":\"Pin\",\"Value\":[\"5400mAh - Sạc nhanh 100W SuperVOOC\"]},\r\n    {\"Name\":\"Hệ điều hành\",\"Value\":[\"OxygenOS 14 (Android 14)\"]}\r\n]", new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "product",
                keyColumn: "Id",
                keyValue: 20,
                columns: new[] { "BasePrice", "CreatedAt", "Description", "Name", "Overview", "Slug", "Specs", "UpdatedAt" },
                values: new object[] { 24990000m, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), "iPad Pro 11 inch M2 với ProMotion 120Hz, hỗ trợ Apple Pencil 2 và Magic Keyboard.", "iPad Pro M2 11 inch", "Máy tính bảng mạnh nhất với chip M2, màn hình Liquid Retina XDR.", "ipad-pro-m2-11", "[\r\n    {\"Name\":\"Màn hình\",\"Value\":[\"11 inch Liquid Retina\",\"IPS LCD (2388x1668)\",\"ProMotion 120Hz\"]},\r\n    {\"Name\":\"Chip\",\"Value\":[\"Apple M2 8-core CPU\",\"10-core GPU\"]},\r\n    {\"Name\":\"RAM\",\"Value\":[\"8GB\",\"16GB\"]},\r\n    {\"Name\":\"Bộ nhớ\",\"Value\":[\"128GB\",\"256GB\",\"512GB\"]},\r\n    {\"Name\":\"Hệ điều hành\",\"Value\":[\"iPadOS 17\"]},\r\n    {\"Name\":\"Phụ kiện\",\"Value\":[\"Apple Pencil 2\",\"Magic Keyboard\"]}\r\n]", new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "product",
                keyColumn: "Id",
                keyValue: 21,
                columns: new[] { "BasePrice", "CreatedAt", "Description", "Overview", "Slug", "Specs", "UpdatedAt" },
                values: new object[] { 19990000m, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), "Galaxy Tab S9 11 inch Snapdragon 8 Gen 2, kháng nước IP68, S Pen kèm theo.", "Tablet Android cao cấp với màn hình Dynamic AMOLED 2X, S Pen.", "samsung-galaxy-tab-s9", "[\r\n    {\"Name\":\"Màn hình\",\"Value\":[\"11 inch Dynamic AMOLED 2X\",\"WQXGA (2560x1600)\",\"120Hz\"]},\r\n    {\"Name\":\"Chip\",\"Value\":[\"Snapdragon 8 Gen 2\"]},\r\n    {\"Name\":\"RAM\",\"Value\":[\"8GB\",\"12GB\"]},\r\n    {\"Name\":\"Bộ nhớ\",\"Value\":[\"128GB\",\"256GB\"]},\r\n    {\"Name\":\"Hệ điều hành\",\"Value\":[\"Android 13 - One UI 5.1\"]},\r\n    {\"Name\":\"Kháng nước\",\"Value\":[\"IP68\"]}\r\n]", new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "product",
                keyColumn: "Id",
                keyValue: 22,
                columns: new[] { "BasePrice", "BrandId", "CreatedAt", "Description", "Name", "Overview", "Slug", "Specs", "UpdatedAt" },
                values: new object[] { 8990000m, 8, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), "Xiaomi Pad 6 11 inch Snapdragon 870, màn hình LCD 144Hz, pin 8840mAh.", "Xiaomi Pad 6", "Tablet tầm trung với màn hình 144Hz, loa 4 kênh Dolby Atmos.", "xiaomi-pad-6", "[\r\n    {\"Name\":\"Màn hình\",\"Value\":[\"11 inch LCD\",\"2.8K (2880x1800)\",\"144Hz\"]},\r\n    {\"Name\":\"Chip\",\"Value\":[\"Snapdragon 870\"]},\r\n    {\"Name\":\"RAM\",\"Value\":[\"6GB\",\"8GB\"]},\r\n    {\"Name\":\"Bộ nhớ\",\"Value\":[\"128GB\",\"256GB\"]},\r\n    {\"Name\":\"Pin\",\"Value\":[\"8840mAh - Sạc nhanh 33W\"]},\r\n    {\"Name\":\"Hệ điều hành\",\"Value\":[\"MIUI Pad 14\"]}\r\n]", new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "product",
                keyColumn: "Id",
                keyValue: 30,
                columns: new[] { "BasePrice", "CreatedAt", "Description", "Name", "Overview", "Slug", "Specs", "UpdatedAt" },
                values: new object[] { 17990000m, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), "Dell U2723DE 27 inch QHD IPS Black, độ chính xác màu cao cho thiết kế đồ họa.", "Dell UltraSharp U2723DE", "Màn hình 27 inch QHD IPS, độ phủ màu 95% DCI-P3, USB-C 90W.", "dell-ultrasharp-u2723de", "[\r\n    {\"Name\":\"Kích thước\",\"Value\":[\"27 inch\"]},\r\n    {\"Name\":\"Độ phân giải\",\"Value\":[\"QHD 2560x1440\"]},\r\n    {\"Name\":\"Tấm nền\",\"Value\":[\"IPS Black\"]},\r\n    {\"Name\":\"Tần số quét\",\"Value\":[\"60Hz\"]},\r\n    {\"Name\":\"Cổng kết nối\",\"Value\":[\"HDMI 2.0\",\"DisplayPort 1.4\",\"USB-C 90W\"]},\r\n    {\"Name\":\"Độ phủ màu\",\"Value\":[\"95% DCI-P3\",\"100% sRGB\"]},\r\n    {\"Name\":\"Bảo hành\",\"Value\":[\"36 tháng\"]}\r\n]", new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "product",
                keyColumn: "Id",
                keyValue: 31,
                columns: new[] { "BasePrice", "CreatedAt", "Description", "Name", "Overview", "Slug", "Specs", "UpdatedAt" },
                values: new object[] { 8490000m, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), "LG 27GN800 27 inch QHD IPS 144Hz, thời gian phản hồi 1ms, HDR10.", "LG UltraGear 27GN800-B", "Màn hình gaming 27 inch QHD 144Hz, tấm nền IPS Nano, G-Sync.", "lg-ultragear-27gn800", "[\r\n    {\"Name\":\"Kích thước\",\"Value\":[\"27 inch\"]},\r\n    {\"Name\":\"Độ phân giải\",\"Value\":[\"QHD 2560x1440\"]},\r\n    {\"Name\":\"Tấm nền\",\"Value\":[\"IPS Nano Color\"]},\r\n    {\"Name\":\"Tần số quét\",\"Value\":[\"144Hz\"]},\r\n    {\"Name\":\"Cổng kết nối\",\"Value\":[\"HDMI 2.0 x2\",\"DisplayPort 1.4\"]},\r\n    {\"Name\":\"Đồng bộ\",\"Value\":[\"G-Sync Compatible\",\"FreeSync Premium\"]}\r\n]", new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "product",
                keyColumn: "Id",
                keyValue: 40,
                columns: new[] { "BasePrice", "CreatedAt", "Description", "Overview", "Specs", "UpdatedAt" },
                values: new object[] { 2990000m, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), "Logitech MX Keys với phím Perfect Stroke, kết nối multi-device, pin 10 ngày.", "Bàn phím không dây cao cấp cho văn phòng, đèn nền thông minh.", "[\r\n    {\"Name\":\"Kiểu kết nối\",\"Value\":[\"Bluetooth\",\"USB Receiver\"]},\r\n    {\"Name\":\"Layout\",\"Value\":[\"Full-size\"]},\r\n    {\"Name\":\"Đèn nền\",\"Value\":[\"Có - Tự động điều chỉnh\"]},\r\n    {\"Name\":\"Pin\",\"Value\":[\"Sạc USB-C - 10 ngày với đèn\"]},\r\n    {\"Name\":\"Multi-device\",\"Value\":[\"3 thiết bị\"]}\r\n]", new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "product",
                keyColumn: "Id",
                keyValue: 41,
                columns: new[] { "BasePrice", "CreatedAt", "Description", "Name", "Overview", "Slug", "Specs", "UpdatedAt" },
                values: new object[] { 5990000m, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), "Razer BlackWidow V4 Pro với Command Dial, switch cơ học, RGB per-key.", "Razer BlackWidow V4 Pro", "Bàn phím cơ gaming cao cấp RGB Chroma, switch Green Clicky.", "razer-blackwidow-v4-pro", "[\r\n    {\"Name\":\"Kiểu\",\"Value\":[\"Cơ học\"]},\r\n    {\"Name\":\"Switch\",\"Value\":[\"Razer Green Clicky\"]},\r\n    {\"Name\":\"Đèn nền\",\"Value\":[\"RGB Chroma per-key\"]},\r\n    {\"Name\":\"Kết nối\",\"Value\":[\"USB Type-C có dây\"]},\r\n    {\"Name\":\"Tính năng\",\"Value\":[\"Command Dial\",\"Media keys\"]}\r\n]", new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "product",
                keyColumn: "Id",
                keyValue: 50,
                columns: new[] { "BasePrice", "CreatedAt", "Description", "Overview", "Specs", "UpdatedAt" },
                values: new object[] { 2490000m, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), "Logitech MX Master 3S với MagSpeed Wheel, kết nối multi-device, pin 70 ngày.", "Chuột không dây ergonomic cho năng suất cao, sensor 8K DPI.", "[\r\n    {\"Name\":\"Sensor\",\"Value\":[\"Darkfield 8000 DPI\"]},\r\n    {\"Name\":\"Kết nối\",\"Value\":[\"Bluetooth\",\"USB Receiver\"]},\r\n    {\"Name\":\"Pin\",\"Value\":[\"70 ngày\"]},\r\n    {\"Name\":\"Số nút\",\"Value\":[\"7 nút có thể tùy chỉnh\"]},\r\n    {\"Name\":\"Multi-device\",\"Value\":[\"3 thiết bị\"]}\r\n]", new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "product",
                keyColumn: "Id",
                keyValue: 51,
                columns: new[] { "BasePrice", "CreatedAt", "Description", "Overview", "Specs", "UpdatedAt" },
                values: new object[] { 3990000m, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), "Razer Viper V2 Pro với HyperSpeed Wireless, optical switch Gen 3, pin 80 giờ.", "Chuột gaming không dây siêu nhẹ 58g, sensor Focus Pro 30K.", "[\r\n    {\"Name\":\"Sensor\",\"Value\":[\"Focus Pro 30K DPI\"]},\r\n    {\"Name\":\"Trọng lượng\",\"Value\":[\"58g\"]},\r\n    {\"Name\":\"Kết nối\",\"Value\":[\"HyperSpeed Wireless 2.4GHz\"]},\r\n    {\"Name\":\"Pin\",\"Value\":[\"80 giờ\"]},\r\n    {\"Name\":\"Switch\",\"Value\":[\"Optical Gen 3\"]}\r\n]", new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "product",
                keyColumn: "Id",
                keyValue: 60,
                columns: new[] { "BasePrice", "CreatedAt", "Description", "Overview", "Specs", "UpdatedAt" },
                values: new object[] { 8990000m, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), "Sony WH-1000XM5 với ANC thế hệ mới, 8 micro, hỗ trợ LDAC và DSEE Extreme.", "Tai nghe chống ồn hàng đầu, chất âm Hi-Res, pin 30 giờ.", "[\r\n    {\"Name\":\"Kiểu\",\"Value\":[\"Over-ear\"]},\r\n    {\"Name\":\"ANC\",\"Value\":[\"Có - 8 micro\"]},\r\n    {\"Name\":\"Pin\",\"Value\":[\"30 giờ\"]},\r\n    {\"Name\":\"Sạc\",\"Value\":[\"USB-C - Sạc nhanh 3 phút = 3 giờ\"]},\r\n    {\"Name\":\"Codec\",\"Value\":[\"LDAC\",\"AAC\",\"SBC\"]},\r\n    {\"Name\":\"Mic\",\"Value\":[\"Có - AI Noise Reduction\"]}\r\n]", new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "product",
                keyColumn: "Id",
                keyValue: 61,
                columns: new[] { "BasePrice", "CreatedAt", "Description", "Overview", "Slug", "Specs", "UpdatedAt" },
                values: new object[] { 6490000m, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), "AirPods Pro 2 với Adaptive Audio, Transparency Mode, case sạc MagSafe và loa tìm kiếm.", "Tai nghe true wireless với ANC tốt nhất, chip H2, sạc MagSafe.", "apple-airpods-pro-2", "[\r\n    {\"Name\":\"Kiểu\",\"Value\":[\"In-ear True Wireless\"]},\r\n    {\"Name\":\"Chip\",\"Value\":[\"Apple H2\"]},\r\n    {\"Name\":\"ANC\",\"Value\":[\"Có - Adaptive\"]},\r\n    {\"Name\":\"Pin\",\"Value\":[\"6 giờ tai nghe\",\"30 giờ với case\"]},\r\n    {\"Name\":\"Sạc\",\"Value\":[\"MagSafe\",\"USB-C\",\"Apple Watch charger\"]},\r\n    {\"Name\":\"Kháng nước\",\"Value\":[\"IPX4\"]}\r\n]", new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "product",
                keyColumn: "Id",
                keyValue: 70,
                columns: new[] { "BasePrice", "CreatedAt", "Description", "Name", "Overview", "Slug", "Specs", "UpdatedAt" },
                values: new object[] { 2490000m, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), "Anker 747 Charger với công nghệ GaN, ActiveShield 2.0, sạc laptop, điện thoại đồng thời.", "Anker 747 GaNPrime 150W", "Sạc nhanh GaN 150W, 4 cổng (3 USB-C + 1 USB-A), sạc cùng lúc 4 thiết bị.", "anker-747-ganprime-150w", "[\r\n    {\"Name\":\"Công suất\",\"Value\":[\"150W tối đa\"]},\r\n    {\"Name\":\"Cổng\",\"Value\":[\"3x USB-C (100W+45W+45W)\",\"1x USB-A (22.5W)\"]},\r\n    {\"Name\":\"Công nghệ\",\"Value\":[\"GaN III\",\"ActiveShield 2.0\"]},\r\n    {\"Name\":\"Điện áp\",\"Value\":[\"100-240V\"]},\r\n    {\"Name\":\"Bảo vệ\",\"Value\":[\"Quá nhiệt\",\"Quá dòng\",\"Ngắn mạch\"]}\r\n]", new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "product",
                keyColumn: "Id",
                keyValue: 71,
                columns: new[] { "BasePrice", "CreatedAt", "Description", "Name", "Overview", "Slug", "Specs", "UpdatedAt" },
                values: new object[] { 290000m, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), "Baseus 100W Cable với E-Marker chip, hỗ trợ PD 3.0, QC 4.0, data transfer 480Mbps.", "Baseus 100W USB-C Cable 2m", "Cáp sạc nhanh USB-C to USB-C 100W, dây bện nylon bền, dài 2m.", "baseus-100w-usb-c-cable", "[\r\n    {\"Name\":\"Chiều dài\",\"Value\":[\"2 mét\"]},\r\n    {\"Name\":\"Đầu nối\",\"Value\":[\"USB-C to USB-C\"]},\r\n    {\"Name\":\"Vật liệu\",\"Value\":[\"Nylon braided + Zinc alloy\"]},\r\n    {\"Name\":\"Công suất\",\"Value\":[\"100W (20V/5A)\"]},\r\n    {\"Name\":\"Tốc độ dữ liệu\",\"Value\":[\"480Mbps\"]}\r\n]", new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "product",
                keyColumn: "Id",
                keyValue: 80,
                columns: new[] { "BasePrice", "CreatedAt", "Description", "Name", "Overview", "Slug", "Specs", "UpdatedAt" },
                values: new object[] { 490000m, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), "Spigen Rugged Armor với vật liệu TPU mềm, viền chống trầy, tản nhiệt tốt.", "Spigen Rugged Armor iPhone 15 Pro", "Ốp lưng chống sốc cho iPhone 15 Pro, thiết kế Carbon Fiber.", "spigen-rugged-armor-iphone-15-pro", "[\r\n    {\"Name\":\"Vật liệu\",\"Value\":[\"TPU dẻo\"]},\r\n    {\"Name\":\"Chống sốc\",\"Value\":[\"Có - Air Cushion Technology\"]},\r\n    {\"Name\":\"Tương thích\",\"Value\":[\"iPhone 15 Pro (6.1 inch)\"]},\r\n    {\"Name\":\"Thiết kế\",\"Value\":[\"Carbon Fiber texture\"]},\r\n    {\"Name\":\"Viền camera\",\"Value\":[\"Nổi bảo vệ camera\"]}\r\n]", new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "product",
                keyColumn: "Id",
                keyValue: 81,
                columns: new[] { "BasePrice", "CreatedAt", "Description", "Name", "Overview", "Slug", "Specs", "UpdatedAt" },
                values: new object[] { 790000m, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), "Tomtoc 360° Protective Laptop Sleeve với CornerArmor bảo vệ góc, YKK zipper.", "Tomtoc Laptop Sleeve 13-14 inch", "Túi chống sốc laptop 13-14 inch, lót CornerArmor, chống nước.", "tomtoc-laptop-sleeve-13-14", "[\r\n    {\"Name\":\"Vật liệu\",\"Value\":[\"Ballistic Nylon 1680D\"]},\r\n    {\"Name\":\"Kích thước\",\"Value\":[\"13-14 inch laptops\"]},\r\n    {\"Name\":\"Chống nước\",\"Value\":[\"Có - Lớp phủ water-resistant\"]},\r\n    {\"Name\":\"Lót đệm\",\"Value\":[\"CornerArmor + Foam padding\"]},\r\n    {\"Name\":\"Khóa kéo\",\"Value\":[\"YKK RC Fuse\"]}\r\n]", new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.InsertData(
                table: "product_image",
                columns: new[] { "Id", "ImageUrl", "IsPrimary", "ProductId", "SortOrder" },
                values: new object[] { 1, "https://i.dell.com/is/image/DellContent/content/dam/ss2/product-images/dell-client-products/notebooks/xps-notebooks/13-9315/media-gallery/notebook-xps-9315-nt-blue-gallery-4.psd?fmt=png-alpha&pscan=auto&scl=1&hei=804&wid=1180&qlt=100,1&resMode=sharp2&size=1180,804&chrss=full", true, 1, 1 });

            migrationBuilder.InsertData(
                table: "product_image",
                columns: new[] { "Id", "ImageUrl", "ProductId", "SortOrder" },
                values: new object[] { 2, "https://i.dell.com/is/image/DellContent/content/dam/ss2/product-images/dell-client-products/notebooks/xps-notebooks/13-9315/media-gallery/notebook-xps-9315-nt-blue-gallery-2.psd?fmt=png-alpha&pscan=auto&scl=1&hei=804&wid=1534&qlt=100,1&resMode=sharp2&size=1534,804&chrss=full", 1, 2 });

            migrationBuilder.InsertData(
                table: "product_image",
                columns: new[] { "Id", "ImageUrl", "IsPrimary", "ProductId", "SortOrder" },
                values: new object[] { 3, "https://store.storeimages.cdn-apple.com/8756/as-images.apple.com/is/mba13-midnight-select-202402?wid=904&hei=840&fmt=jpeg&qlt=90&.v=1708367688034", true, 2, 1 });

            migrationBuilder.InsertData(
                table: "product_image",
                columns: new[] { "Id", "ImageUrl", "ProductId", "SortOrder" },
                values: new object[] { 4, "https://store.storeimages.cdn-apple.com/8756/as-images.apple.com/is/macbook-air-midnight-config-202402?wid=820&hei=498&fmt=jpeg&qlt=90&.v=1708367398169", 2, 2 });

            migrationBuilder.InsertData(
                table: "product_image",
                columns: new[] { "Id", "ImageUrl", "IsPrimary", "ProductId", "SortOrder" },
                values: new object[] { 5, "https://dlcdnwebimgs.asus.com/gain/8E4B3A6D-5F51-465F-8B7E-BD8BE8C77C52/w717/h525", true, 3, 1 });

            migrationBuilder.InsertData(
                table: "product_image",
                columns: new[] { "Id", "ImageUrl", "ProductId", "SortOrder" },
                values: new object[] { 6, "https://dlcdnwebimgs.asus.com/gain/82CB6B3D-6C1D-4A7E-9A0C-1D9F3E8B8C8D/w717/h525", 3, 2 });

            migrationBuilder.InsertData(
                table: "product_image",
                columns: new[] { "Id", "ImageUrl", "IsPrimary", "ProductId", "SortOrder" },
                values: new object[] { 7, "https://ssl-product-images.www8-hp.com/digmedialib/prodimg/lowres/c08111766.png", true, 4, 1 });

            migrationBuilder.InsertData(
                table: "product_image",
                columns: new[] { "Id", "ImageUrl", "ProductId", "SortOrder" },
                values: new object[] { 8, "https://ssl-product-images.www8-hp.com/digmedialib/prodimg/lowres/c08111768.png", 4, 2 });

            migrationBuilder.InsertData(
                table: "product_image",
                columns: new[] { "Id", "ImageUrl", "IsPrimary", "ProductId", "SortOrder" },
                values: new object[] { 9, "https://p3-ofp.static.pub/fes/cms/2023/03/13/exh3cw4xp3hqh5w22bkv5gxkd00ozq528090.png", true, 5, 1 });

            migrationBuilder.InsertData(
                table: "product_image",
                columns: new[] { "Id", "ImageUrl", "ProductId", "SortOrder" },
                values: new object[] { 10, "https://p1-ofp.static.pub/fes/cms/2023/03/13/ulhzrk7v2chz1bmz5aatsjluwrv24c287126.png", 5, 2 });

            migrationBuilder.InsertData(
                table: "product_image",
                columns: new[] { "Id", "ImageUrl", "IsPrimary", "ProductId", "SortOrder" },
                values: new object[] { 11, "https://store.storeimages.cdn-apple.com/8756/as-images.apple.com/is/iphone-15-pro-max-bluetitanium-select?wid=470&hei=556&fmt=png-alpha&.v=1692845702781", true, 10, 1 });

            migrationBuilder.InsertData(
                table: "product_image",
                columns: new[] { "Id", "ImageUrl", "ProductId", "SortOrder" },
                values: new object[] { 12, "https://store.storeimages.cdn-apple.com/8756/as-images.apple.com/is/iphone-15-pro-max-bluetitanium-select-202309_GEO_US?wid=470&hei=556&fmt=png-alpha&.v=1693009279096", 10, 2 });

            migrationBuilder.InsertData(
                table: "product_image",
                columns: new[] { "Id", "ImageUrl", "IsPrimary", "ProductId", "SortOrder" },
                values: new object[] { 13, "https://images.samsung.com/is/image/samsung/p6pim/vn/2401/gallery/vn-galaxy-s24-s928-sm-s928bztgxxv-thumb-539572408", true, 11, 1 });

            migrationBuilder.InsertData(
                table: "product_image",
                columns: new[] { "Id", "ImageUrl", "ProductId", "SortOrder" },
                values: new object[] { 14, "https://images.samsung.com/is/image/samsung/p6pim/vn/2401/gallery/vn-galaxy-s24-s928-sm-s928bztgxxv-539572407", 11, 2 });

            migrationBuilder.InsertData(
                table: "product_image",
                columns: new[] { "Id", "ImageUrl", "IsPrimary", "ProductId", "SortOrder" },
                values: new object[] { 15, "https://lh3.googleusercontent.com/VkQhN5T5M3j3TZ5h1Q0-wM7K9j7vK2jqj6iqZE5n6Hx1Q2E5Q7R8T9U0V1W2X3Y4Z5", true, 12, 1 });

            migrationBuilder.InsertData(
                table: "product_image",
                columns: new[] { "Id", "ImageUrl", "ProductId", "SortOrder" },
                values: new object[] { 16, "https://lh3.googleusercontent.com/0wCDsVFj6f1ck0jRqjD3Q7C8tB7vE8kI0hH9tG8sF7rE6qD5cC4bB3aA2zZ1yY0xX", 12, 2 });

            migrationBuilder.InsertData(
                table: "product_image",
                columns: new[] { "Id", "ImageUrl", "IsPrimary", "ProductId", "SortOrder" },
                values: new object[] { 17, "https://i02.appmifile.com/mi-com-product/fly-birds/xiaomi-14/pc/2d8b8c0c0a2a93fe0f4f4c8c0c0a2a93.png", true, 13, 1 });

            migrationBuilder.InsertData(
                table: "product_image",
                columns: new[] { "Id", "ImageUrl", "ProductId", "SortOrder" },
                values: new object[] { 18, "https://i02.appmifile.com/mi-com-product/fly-birds/xiaomi-14/pc/9d7f8e9f0e1e92ed1e3e2c9d0d1e92ed.png", 13, 2 });

            migrationBuilder.InsertData(
                table: "product_image",
                columns: new[] { "Id", "ImageUrl", "IsPrimary", "ProductId", "SortOrder" },
                values: new object[] { 19, "https://oasis.opstatics.com/content/dam/oasis/page/2024/global/product/12/image/kv-img.png", true, 14, 1 });

            migrationBuilder.InsertData(
                table: "product_image",
                columns: new[] { "Id", "ImageUrl", "ProductId", "SortOrder" },
                values: new object[] { 20, "https://oasis.opstatics.com/content/dam/oasis/page/2024/global/product/12/image/gallery-img-1.png", 14, 2 });

            migrationBuilder.InsertData(
                table: "product_image",
                columns: new[] { "Id", "ImageUrl", "IsPrimary", "ProductId", "SortOrder" },
                values: new object[] { 21, "https://store.storeimages.cdn-apple.com/8756/as-images.apple.com/is/ipad-pro-11-select-wifi-spacegray-202210?wid=470&hei=556&fmt=png-alpha&.v=1664411207213", true, 20, 1 });

            migrationBuilder.InsertData(
                table: "product_image",
                columns: new[] { "Id", "ImageUrl", "ProductId", "SortOrder" },
                values: new object[] { 22, "https://store.storeimages.cdn-apple.com/8756/as-images.apple.com/is/ipad-pro-11-finish-select-202210-space-gray-wifi?wid=5120&hei=2880&fmt=p-jpg&qlt=80&.v=1664411443597", 20, 2 });

            migrationBuilder.InsertData(
                table: "product_image",
                columns: new[] { "Id", "ImageUrl", "IsPrimary", "ProductId", "SortOrder" },
                values: new object[] { 23, "https://images.samsung.com/is/image/samsung/p6pim/vn/sm-x710nzaaxxv/gallery/vn-galaxy-tab-s9-5g-x710-sm-x710nzaaxxv-thumb-537168467", true, 21, 1 });

            migrationBuilder.InsertData(
                table: "product_image",
                columns: new[] { "Id", "ImageUrl", "ProductId", "SortOrder" },
                values: new object[] { 24, "https://images.samsung.com/is/image/samsung/p6pim/vn/sm-x710nzaaxxv/gallery/vn-galaxy-tab-s9-5g-x710-sm-x710nzaaxxv-537168466", 21, 2 });

            migrationBuilder.InsertData(
                table: "product_image",
                columns: new[] { "Id", "ImageUrl", "IsPrimary", "ProductId", "SortOrder" },
                values: new object[] { 25, "https://i02.appmifile.com/mi-com-product/fly-birds/xiaomi-pad-6/pc/pad6-kv.png", true, 22, 1 });

            migrationBuilder.InsertData(
                table: "product_image",
                columns: new[] { "Id", "ImageUrl", "ProductId", "SortOrder" },
                values: new object[] { 26, "https://i02.appmifile.com/mi-com-product/fly-birds/xiaomi-pad-6/pc/pad6-gallery-1.png", 22, 2 });

            migrationBuilder.InsertData(
                table: "product_image",
                columns: new[] { "Id", "ImageUrl", "IsPrimary", "ProductId", "SortOrder" },
                values: new object[,]
                {
                    { 27, "https://i.dell.com/is/image/DellContent/content/dam/ss2/product-images/dell-electronics-and-accessories/dell-monitors/u-series/u2723de/media-gallery/monitor-u2723de-gallery-1.psd?fmt=pjpg&pscan=auto&scl=1&wid=3337&hei=2503&qlt=100,1&resMode=sharp2&size=3337,2503&chrss=full&imwidth=5000", true, 30, 1 },
                    { 28, "https://www.lg.com/vn/images/man-hinh-may-tinh/md07577031/gallery/D-01.jpg", true, 31, 1 },
                    { 29, "https://resource.logitech.com/w_800,c_limit,q_auto,f_auto,dpr_1.0/d_transparent.gif/content/dam/logitech/en/products/keyboards/mx-keys/gallery/mx-keys-gallery-graphite-01.png?v=1", true, 40, 1 }
                });

            migrationBuilder.InsertData(
                table: "product_image",
                columns: new[] { "Id", "ImageUrl", "ProductId", "SortOrder" },
                values: new object[] { 30, "https://resource.logitech.com/w_800,c_limit,q_auto,f_auto,dpr_1.0/d_transparent.gif/content/dam/logitech/en/products/keyboards/mx-keys/gallery/mx-keys-gallery-graphite-02.png?v=1", 40, 2 });

            migrationBuilder.InsertData(
                table: "product_image",
                columns: new[] { "Id", "ImageUrl", "IsPrimary", "ProductId", "SortOrder" },
                values: new object[,]
                {
                    { 31, "https://assets2.razerzone.com/images/pnx.assets/618d0f3581cf6ac3177c4ae2af69e0fa/razer-blackwidow-v4-pro-gallery-hero-1500x1000.png", true, 41, 1 },
                    { 32, "https://resource.logitech.com/w_800,c_lpad,ar_1:1,q_auto,f_auto,dpr_1.0/d_transparent.gif/content/dam/logitech/en/products/mice/mx-master-3s/gallery/mx-master-3s-mouse-top-view-graphite.png?v=1", true, 50, 1 }
                });

            migrationBuilder.InsertData(
                table: "product_image",
                columns: new[] { "Id", "ImageUrl", "ProductId", "SortOrder" },
                values: new object[] { 33, "https://resource.logitech.com/w_800,c_lpad,ar_1:1,q_auto,f_auto,dpr_1.0/d_transparent.gif/content/dam/logitech/en/products/mice/mx-master-3s/gallery/mx-master-3s-mouse-side-view-graphite.png?v=1", 50, 2 });

            migrationBuilder.InsertData(
                table: "product_image",
                columns: new[] { "Id", "ImageUrl", "IsPrimary", "ProductId", "SortOrder" },
                values: new object[,]
                {
                    { 34, "https://assets2.razerzone.com/images/pnx.assets/82f433e3c3e59ce939a70eba8c26a21b/razer-viper-v2-pro-gallery-hero-1500x1000.png", true, 51, 1 },
                    { 35, "https://www.sony.com.vn/image/5d02da5df552836db894cead8a68f5f3?fmt=pjpeg&wid=330&bgcolor=FFFFFF&bgc=FFFFFF", true, 60, 1 }
                });

            migrationBuilder.InsertData(
                table: "product_image",
                columns: new[] { "Id", "ImageUrl", "ProductId", "SortOrder" },
                values: new object[] { 36, "https://www.sony.com.vn/image/c5fb8db9f5be1ab2e6f55bcb59a2c3b7?fmt=pjpeg&wid=330&bgcolor=FFFFFF&bgc=FFFFFF", 60, 2 });

            migrationBuilder.InsertData(
                table: "product_image",
                columns: new[] { "Id", "ImageUrl", "IsPrimary", "ProductId", "SortOrder" },
                values: new object[,]
                {
                    { 37, "https://store.storeimages.cdn-apple.com/8756/as-images.apple.com/is/MQD83?wid=1144&hei=1144&fmt=jpeg&qlt=90&.v=1660803972361", true, 61, 1 },
                    { 38, "https://m.media-amazon.com/images/I/61ue5fO88JL._AC_SL1500_.jpg", true, 70, 1 },
                    { 39, "https://m.media-amazon.com/images/I/61DQwwGO9dL._AC_SL1500_.jpg", true, 71, 1 },
                    { 40, "https://m.media-amazon.com/images/I/71GxKxT3GCL._AC_SL1500_.jpg", true, 80, 1 },
                    { 41, "https://m.media-amazon.com/images/I/71Vy5eF8qpL._AC_SL1500_.jpg", true, 81, 1 }
                });

            migrationBuilder.UpdateData(
                table: "product_variant",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Price", "Stock" },
                values: new object[] { new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), 25990000m, 15 });

            migrationBuilder.UpdateData(
                table: "product_variant",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "Price", "Stock" },
                values: new object[] { new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), 32990000m, 8 });

            migrationBuilder.UpdateData(
                table: "product_variant",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "Price", "Stock" },
                values: new object[] { new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), 28990000m, 12 });

            migrationBuilder.UpdateData(
                table: "product_variant",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "Price", "Stock" },
                values: new object[] { new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), 36990000m, 7 });

            migrationBuilder.UpdateData(
                table: "product_variant",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAt", "Price", "Sku", "Stock" },
                values: new object[] { new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), 22490000m, "ZEN14-I5-8-512", 20 });

            migrationBuilder.UpdateData(
                table: "product_variant",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CreatedAt", "Price", "Sku", "Stock" },
                values: new object[] { new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), 28990000m, "ZEN14-I7-16-1TB", 10 });

            migrationBuilder.UpdateData(
                table: "product_variant",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "CreatedAt", "Price", "Stock" },
                values: new object[] { new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), 42990000m, 12 });

            migrationBuilder.UpdateData(
                table: "product_variant",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "CreatedAt", "Price", "Stock" },
                values: new object[] { new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), 52990000m, 6 });

            migrationBuilder.UpdateData(
                table: "product_variant",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "CreatedAt", "Price", "Sku", "Stock" },
                values: new object[] { new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), 48990000m, "X1C11-I5-16-512", 10 });

            migrationBuilder.UpdateData(
                table: "product_variant",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "CreatedAt", "Price", "Sku", "Stock" },
                values: new object[] { new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), 58990000m, "X1C11-I7-16-1TB", 5 });

            migrationBuilder.UpdateData(
                table: "product_variant",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "CreatedAt", "Price", "Sku", "Stock" },
                values: new object[] { new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), 33990000m, "IP15PM-256-NAT", 25 });

            migrationBuilder.UpdateData(
                table: "product_variant",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "CreatedAt", "Price", "Sku" },
                values: new object[] { new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), 39990000m, "IP15PM-512-NAT" });

            migrationBuilder.UpdateData(
                table: "product_variant",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "CreatedAt", "Price", "ProductId", "Sku", "Stock" },
                values: new object[] { new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), 33990000m, 10, "IP15PM-256-BLK", 20 });

            migrationBuilder.UpdateData(
                table: "product_variant",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "CreatedAt", "Price", "Sku", "Stock" },
                values: new object[] { new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), 29990000m, "S24U-256-GRAY", 30 });

            migrationBuilder.UpdateData(
                table: "product_variant",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "CreatedAt", "Price", "ProductId", "Sku" },
                values: new object[] { new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), 34990000m, 11, "S24U-512-GRAY" });

            migrationBuilder.UpdateData(
                table: "product_variant",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "CreatedAt", "Price", "ProductId", "Sku", "Stock" },
                values: new object[] { new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), 29990000m, 11, "S24U-256-VIOL", 25 });

            migrationBuilder.UpdateData(
                table: "product_variant",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "CreatedAt", "Price", "ProductId", "Sku" },
                values: new object[] { new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), 24990000m, 12, "PIX8P-128-OBSI" });

            migrationBuilder.UpdateData(
                table: "product_variant",
                keyColumn: "Id",
                keyValue: 18,
                columns: new[] { "CreatedAt", "Price", "ProductId", "Sku" },
                values: new object[] { new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), 27990000m, 12, "PIX8P-256-OBSI" });

            migrationBuilder.UpdateData(
                table: "product_variant",
                keyColumn: "Id",
                keyValue: 19,
                columns: new[] { "CreatedAt", "Price", "ProductId", "Sku", "Stock" },
                values: new object[] { new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), 27990000m, 12, "PIX8P-256-BAY", 12 });

            migrationBuilder.UpdateData(
                table: "product_variant",
                keyColumn: "Id",
                keyValue: 20,
                columns: new[] { "CreatedAt", "Price", "ProductId", "Sku", "Stock" },
                values: new object[] { new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), 18990000m, 13, "MI14-12-256-BLK", 35 });

            migrationBuilder.UpdateData(
                table: "product_variant",
                keyColumn: "Id",
                keyValue: 21,
                columns: new[] { "CreatedAt", "Price", "ProductId", "Sku", "Stock" },
                values: new object[] { new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), 21990000m, 13, "MI14-12-512-BLK", 22 });

            migrationBuilder.UpdateData(
                table: "product_variant",
                keyColumn: "Id",
                keyValue: 22,
                columns: new[] { "CreatedAt", "Price", "ProductId", "Sku", "Stock" },
                values: new object[] { new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), 19990000m, 14, "OP12-12-256-BLK", 28 });

            migrationBuilder.UpdateData(
                table: "product_variant",
                keyColumn: "Id",
                keyValue: 23,
                columns: new[] { "CreatedAt", "Price", "ProductId", "Sku", "Stock" },
                values: new object[] { new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), 23990000m, 14, "OP12-16-512-GRN", 18 });

            migrationBuilder.UpdateData(
                table: "product_variant",
                keyColumn: "Id",
                keyValue: 24,
                columns: new[] { "CreatedAt", "Price", "ProductId", "Sku", "Stock" },
                values: new object[] { new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), 24990000m, 20, "IPADPRO11-128-SIL", 18 });

            migrationBuilder.UpdateData(
                table: "product_variant",
                keyColumn: "Id",
                keyValue: 25,
                columns: new[] { "CreatedAt", "Price", "ProductId", "Sku", "Stock" },
                values: new object[] { new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), 28990000m, 20, "IPADPRO11-256-SIL", 12 });

            migrationBuilder.UpdateData(
                table: "product_variant",
                keyColumn: "Id",
                keyValue: 26,
                columns: new[] { "CreatedAt", "Price", "ProductId", "Sku", "Stock" },
                values: new object[] { new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), 28990000m, 20, "IPADPRO11-256-SPC", 10 });

            migrationBuilder.UpdateData(
                table: "product_variant",
                keyColumn: "Id",
                keyValue: 27,
                columns: new[] { "CreatedAt", "Price", "ProductId", "Sku", "Stock" },
                values: new object[] { new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), 19990000m, 21, "TABS9-8-128-GRAY", 25 });

            migrationBuilder.UpdateData(
                table: "product_variant",
                keyColumn: "Id",
                keyValue: 28,
                columns: new[] { "CreatedAt", "Price", "ProductId", "Sku", "Stock" },
                values: new object[] { new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), 24990000m, 21, "TABS9-12-256-GRAY", 15 });

            migrationBuilder.UpdateData(
                table: "product_variant",
                keyColumn: "Id",
                keyValue: 29,
                columns: new[] { "CreatedAt", "Price", "ProductId", "Sku" },
                values: new object[] { new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), 8990000m, 22, "MIPAD6-6-128-GRAY" });

            migrationBuilder.UpdateData(
                table: "product_variant",
                keyColumn: "Id",
                keyValue: 30,
                columns: new[] { "CreatedAt", "Price", "ProductId", "Sku", "Stock" },
                values: new object[] { new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), 10990000m, 22, "MIPAD6-8-256-BLUE", 30 });

            migrationBuilder.UpdateData(
                table: "product_variant",
                keyColumn: "Id",
                keyValue: 31,
                columns: new[] { "CreatedAt", "Price", "ProductId", "Sku", "Stock" },
                values: new object[] { new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), 17990000m, 30, "U2723DE-27QHD", 15 });

            migrationBuilder.UpdateData(
                table: "product_variant",
                keyColumn: "Id",
                keyValue: 32,
                columns: new[] { "CreatedAt", "Price", "ProductId", "Sku", "Stock" },
                values: new object[] { new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), 8490000m, 31, "27GN800-QHD144", 22 });

            migrationBuilder.InsertData(
                table: "product_variant",
                columns: new[] { "Id", "CreatedAt", "Price", "ProductId", "Sku", "Status", "Stock", "VariantSpecsJson" },
                values: new object[,]
                {
                    { 33, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), 2990000m, 40, "MXKEYS-GRAY", "available", 35, null },
                    { 34, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), 2990000m, 40, "MXKEYS-WHT", "available", 28, null },
                    { 35, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), 5990000m, 41, "BW-V4PRO-GRN", "available", 20, null },
                    { 36, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), 2490000m, 50, "MXM3S-GRAPH", "available", 40, null },
                    { 37, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), 2490000m, 50, "MXM3S-PALE", "available", 32, null },
                    { 38, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), 3990000m, 51, "VIPER-V2PRO-BLK", "available", 25, null },
                    { 39, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), 3990000m, 51, "VIPER-V2PRO-WHT", "available", 20, null },
                    { 40, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), 8990000m, 60, "XM5-BLK", "available", 22, null },
                    { 41, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), 8990000m, 60, "XM5-SLV", "available", 18, null },
                    { 42, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), 6490000m, 61, "AIRPODSPRO2-USBC", "available", 35, null },
                    { 43, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), 2490000m, 70, "ANKER747-150W", "available", 45, null },
                    { 44, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), 290000m, 71, "BASEUS-C2C-2M-BLK", "available", 120, null },
                    { 45, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), 290000m, 71, "BASEUS-C2C-2M-WHT", "available", 100, null },
                    { 46, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), 490000m, 80, "SPIGEN-IP15P-BLK", "available", 80, null },
                    { 47, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), 490000m, 80, "SPIGEN-IP15P-CLEAR", "available", 70, null },
                    { 48, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), 790000m, 81, "TOMTOC-13-BLK", "available", 50, null },
                    { 49, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), 790000m, 81, "TOMTOC-14-BLK", "available", 45, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 35);

            migrationBuilder.DeleteData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 36);

            migrationBuilder.DeleteData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 37);

            migrationBuilder.DeleteData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 38);

            migrationBuilder.DeleteData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 39);

            migrationBuilder.DeleteData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 40);

            migrationBuilder.DeleteData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 41);

            migrationBuilder.DeleteData(
                table: "product_variant",
                keyColumn: "Id",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "product_variant",
                keyColumn: "Id",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "product_variant",
                keyColumn: "Id",
                keyValue: 35);

            migrationBuilder.DeleteData(
                table: "product_variant",
                keyColumn: "Id",
                keyValue: 36);

            migrationBuilder.DeleteData(
                table: "product_variant",
                keyColumn: "Id",
                keyValue: 37);

            migrationBuilder.DeleteData(
                table: "product_variant",
                keyColumn: "Id",
                keyValue: 38);

            migrationBuilder.DeleteData(
                table: "product_variant",
                keyColumn: "Id",
                keyValue: 39);

            migrationBuilder.DeleteData(
                table: "product_variant",
                keyColumn: "Id",
                keyValue: 40);

            migrationBuilder.DeleteData(
                table: "product_variant",
                keyColumn: "Id",
                keyValue: 41);

            migrationBuilder.DeleteData(
                table: "product_variant",
                keyColumn: "Id",
                keyValue: 42);

            migrationBuilder.DeleteData(
                table: "product_variant",
                keyColumn: "Id",
                keyValue: 43);

            migrationBuilder.DeleteData(
                table: "product_variant",
                keyColumn: "Id",
                keyValue: 44);

            migrationBuilder.DeleteData(
                table: "product_variant",
                keyColumn: "Id",
                keyValue: 45);

            migrationBuilder.DeleteData(
                table: "product_variant",
                keyColumn: "Id",
                keyValue: 46);

            migrationBuilder.DeleteData(
                table: "product_variant",
                keyColumn: "Id",
                keyValue: 47);

            migrationBuilder.DeleteData(
                table: "product_variant",
                keyColumn: "Id",
                keyValue: 48);

            migrationBuilder.DeleteData(
                table: "product_variant",
                keyColumn: "Id",
                keyValue: 49);

            migrationBuilder.UpdateData(
                table: "category",
                keyColumn: "Id",
                keyValue: 4,
                column: "ImageUrl",
                value: "https://res.cloudinary.com/dwa3wh9yb/image/upload/v1764337714/mouse_enodsx.png");

            migrationBuilder.UpdateData(
                table: "category",
                keyColumn: "Id",
                keyValue: 13,
                column: "IconPath",
                value: "https://res.cloudinary.com/dwa3wh9yb/image/upload/v1760540871/tekno/category/icon/f0p9oqwzazwy19qvhclr.png");

            migrationBuilder.UpdateData(
                table: "category",
                keyColumn: "Id",
                keyValue: 15,
                column: "ImageUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "product",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "BasePrice", "CreatedAt", "Description", "Overview", "Specs", "UpdatedAt" },
                values: new object[] { 1699.00m, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "Dell XPS 13 series laptops designed for professionals.", "Premium ultrabook with compact design.", "[\r\n    {\"Name\":\"Display\",\"Value\":[\"13.4-inch FHD+ InfinityEdge\"]},\r\n    {\"Name\":\"CPU\",\"Value\":[\"Intel i5\",\"Intel i7\"]},\r\n    {\"Name\":\"RAM\",\"Value\":[\"8GB\",\"16GB\"]},\r\n    {\"Name\":\"Storage\",\"Value\":[\"512GB\",\"1TB SSD\"]},\r\n    {\"Name\":\"Weight\",\"Value\":[\"1.2kg\"]},\r\n    {\"Name\":\"Battery\",\"Value\":[\"52Wh\"]},\r\n    {\"Name\":\"OS\",\"Value\":[\"Windows 11\"]},\r\n    {\"Name\":\"Warranty\",\"Value\":[\"12 months\"]}\r\n]", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "product",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "BasePrice", "CreatedAt", "Description", "Name", "Overview", "Slug", "Specs", "UpdatedAt" },
                values: new object[] { 1199.00m, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "MacBook Air powered by Apple M-series chips.", "MacBook Air", "Ultra-thin and lightweight laptop by Apple.", "macbook-air", "[\r\n    {\"Name\":\"Display\",\"Value\":[\"13.6-inch Liquid Retina\"]},\r\n    {\"Name\":\"Chip\",\"Value\":[\"Apple M2\"]},\r\n    {\"Name\":\"RAM\",\"Value\":[\"8GB\",\"16GB\"]},\r\n    {\"Name\":\"Storage\",\"Value\":[\"256GB\",\"512GB\"]},\r\n    {\"Name\":\"Battery\",\"Value\":[\"52.6Wh up to 18h\"]},\r\n    {\"Name\":\"OS\",\"Value\":[\"macOS\"]},\r\n    {\"Name\":\"Weight\",\"Value\":[\"1.24kg\"]}\r\n]", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "product",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "BasePrice", "CreatedAt", "Description", "Name", "Overview", "Slug", "Specs", "UpdatedAt" },
                values: new object[] { 1450.00m, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "ZenBook series with Intel and AMD variants.", "Asus ZenBook", "Portable productivity ultrabook.", "asus-zenbook", "[\r\n    {\"Name\":\"Display\",\"Value\":[\"14-inch OLED 2.8K\"]},\r\n    {\"Name\":\"CPU\",\"Value\":[\"Intel i5\",\"Intel i7\",\"Ryzen 7\"]},\r\n    {\"Name\":\"RAM\",\"Value\":[\"8GB\",\"16GB\"]},\r\n    {\"Name\":\"Storage\",\"Value\":[\"512GB\",\"1TB SSD\"]},\r\n    {\"Name\":\"OS\",\"Value\":[\"Windows 11\"]},\r\n    {\"Name\":\"Weight\",\"Value\":[\"1.3kg\"]}\r\n]", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "product",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "BasePrice", "CreatedAt", "Description", "Name", "Overview", "Slug", "Specs", "UpdatedAt" },
                values: new object[] { 1799.00m, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "2-in-1 design with touch and pen support.", "HP Spectre x360", "Convertible premium laptop.", "hp-spectre-x360", "[\r\n    {\"Name\":\"Display\",\"Value\":[\"13.5-inch 2-in-1 Touch OLED\"]},\r\n    {\"Name\":\"CPU\",\"Value\":[\"Intel i5\",\"Intel i7\"]},\r\n    {\"Name\":\"RAM\",\"Value\":[\"8GB\",\"16GB\"]},\r\n    {\"Name\":\"Storage\",\"Value\":[\"512GB\",\"1TB\"]},\r\n    {\"Name\":\"Convertible\",\"Value\":[\"true\"]},\r\n    {\"Name\":\"OS\",\"Value\":[\"Windows 11 Home\"]}\r\n]", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "product",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "BasePrice", "CreatedAt", "Description", "Name", "Overview", "Slug", "Specs", "UpdatedAt" },
                values: new object[] { 1999.00m, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "ThinkPad X1 Carbon for professionals.", "Lenovo ThinkPad X1 Carbon", "Business ultrabook with robust build.", "thinkpad-x1-carbon", "[\r\n    {\"Name\":\"Display\",\"Value\":[\"14-inch IPS 2.8K\"]},\r\n    {\"Name\":\"CPU\",\"Value\":[\"Intel i5\",\"Intel i7\"]},\r\n    {\"Name\":\"RAM\",\"Value\":[\"8GB\",\"16GB\"]},\r\n    {\"Name\":\"Storage\",\"Value\":[\"512GB\",\"1TB\"]},\r\n    {\"Name\":\"Security\",\"Value\":[\"Fingerprint\",\"TPM 2.0\"]},\r\n    {\"Name\":\"OS\",\"Value\":[\"Windows 11 Pro\"]}\r\n]", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "product",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "BasePrice", "CreatedAt", "Description", "Name", "Overview", "Slug", "Specs", "UpdatedAt" },
                values: new object[] { 999.00m, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "iPhone 15 Pro with titanium frame and A17 Pro chip.", "iPhone 15 Pro", "Apple flagship smartphone.", "iphone-15-pro", "[\r\n    {\"Name\":\"Display\",\"Value\":[\"6.1-inch OLED 120Hz\"]},\r\n    {\"Name\":\"Chip\",\"Value\":[\"Apple A17 Pro\"]},\r\n    {\"Name\":\"RAM\",\"Value\":[\"6GB\"]},\r\n    {\"Name\":\"Storage\",\"Value\":[\"128GB\",\"256GB\"]},\r\n    {\"Name\":\"Camera\",\"Value\":[\"48MP\",\"12MP\",\"12MP\"]},\r\n    {\"Name\":\"Battery\",\"Value\":[\"3279mAh\"]},\r\n    {\"Name\":\"OS\",\"Value\":[\"iOS 17\"]}\r\n]", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "product",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "BasePrice", "CreatedAt", "Description", "Name", "Overview", "Slug", "Specs", "UpdatedAt" },
                values: new object[] { 899.00m, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "Galaxy S24 with AMOLED display and powerful camera.", "Samsung Galaxy S24", "Next-gen Android flagship.", "galaxy-s24", "[\r\n    {\"Name\":\"Display\",\"Value\":[\"6.7-inch Dynamic AMOLED 120Hz\"]},\r\n    {\"Name\":\"Chip\",\"Value\":[\"Snapdragon 8 Gen 3\"]},\r\n    {\"Name\":\"RAM\",\"Value\":[\"8GB\"]},\r\n    {\"Name\":\"Storage\",\"Value\":[\"128GB\",\"256GB\"]},\r\n    {\"Name\":\"Camera\",\"Value\":[\"200MP\",\"12MP\",\"10MP\"]},\r\n    {\"Name\":\"Battery\",\"Value\":[\"5000mAh\"]},\r\n    {\"Name\":\"OS\",\"Value\":[\"Android 14\"]}\r\n]", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "product",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "BasePrice", "CreatedAt", "Description", "Name", "Overview", "Slug", "Specs", "UpdatedAt" },
                values: new object[] { 799.00m, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "Pixel 9 with Tensor G4 chip and AI photography.", "Google Pixel 9", "Pure Android experience.", "pixel-9", "[\r\n    {\"Name\":\"Display\",\"Value\":[\"6.3-inch AMOLED 120Hz\"]},\r\n    {\"Name\":\"Chip\",\"Value\":[\"Google Tensor G4\"]},\r\n    {\"Name\":\"RAM\",\"Value\":[\"8GB\"]},\r\n    {\"Name\":\"Storage\",\"Value\":[\"128GB\",\"256GB\"]},\r\n    {\"Name\":\"Camera\",\"Value\":[\"50MP\",\"12MP\"]},\r\n    {\"Name\":\"Battery\",\"Value\":[\"4700mAh\"]},\r\n    {\"Name\":\"OS\",\"Value\":[\"Android 14\"]}\r\n]", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "product",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "BasePrice", "CreatedAt", "Description", "Name", "Overview", "Slug", "Specs", "UpdatedAt" },
                values: new object[] { 850.00m, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "Xiaomi 14 Pro with Leica cameras.", "Xiaomi 14 Pro", "High-end performance smartphone.", "xiaomi-14-pro", "[\r\n    {\"Name\":\"Display\",\"Value\":[\"6.7-inch AMOLED QHD+\"]},\r\n    {\"Name\":\"Chip\",\"Value\":[\"Snapdragon 8 Gen 3\"]},\r\n    {\"Name\":\"RAM\",\"Value\":[\"12GB\"]},\r\n    {\"Name\":\"Storage\",\"Value\":[\"256GB\",\"512GB\"]},\r\n    {\"Name\":\"Camera\",\"Value\":[\"50MP\",\"50MP\",\"50MP (Leica)\"]},\r\n    {\"Name\":\"OS\",\"Value\":[\"HyperOS (Android 14)\"]}\r\n]", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "product",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "BasePrice", "CreatedAt", "Description", "Overview", "Specs", "UpdatedAt" },
                values: new object[] { 749.00m, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "OnePlus 12 offers fast charging and high refresh rate.", "Performance-focused smartphone.", "[\r\n    {\"Name\":\"Display\",\"Value\":[\"6.8-inch AMOLED 120Hz\"]},\r\n    {\"Name\":\"Chip\",\"Value\":[\"Snapdragon 8 Gen 3\"]},\r\n    {\"Name\":\"RAM\",\"Value\":[\"12GB\"]},\r\n    {\"Name\":\"Storage\",\"Value\":[\"256GB\",\"512GB\"]},\r\n    {\"Name\":\"Battery\",\"Value\":[\"5400mAh 100W charging\"]},\r\n    {\"Name\":\"OS\",\"Value\":[\"OxygenOS 14\"]}\r\n]", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "product",
                keyColumn: "Id",
                keyValue: 20,
                columns: new[] { "BasePrice", "CreatedAt", "Description", "Name", "Overview", "Slug", "Specs", "UpdatedAt" },
                values: new object[] { 899.00m, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "iPad Pro with M2 chip and ProMotion display.", "iPad Pro", "Powerful tablet for creators.", "ipad-pro", "[\r\n    {\"Name\":\"Display\",\"Value\":[\"12.9-inch Liquid Retina XDR\"]},\r\n    {\"Name\":\"Chip\",\"Value\":[\"Apple M2\"]},\r\n    {\"Name\":\"RAM\",\"Value\":[\"8GB\",\"16GB\"]},\r\n    {\"Name\":\"Storage\",\"Value\":[\"128GB\",\"256GB\"]},\r\n    {\"Name\":\"OS\",\"Value\":[\"iPadOS 17\"]},\r\n    {\"Name\":\"PencilSupport\",\"Value\":[\"Apple Pencil 2\"]}\r\n]", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "product",
                keyColumn: "Id",
                keyValue: 21,
                columns: new[] { "BasePrice", "CreatedAt", "Description", "Overview", "Slug", "Specs", "UpdatedAt" },
                values: new object[] { 799.00m, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "Galaxy Tab S9 series with AMOLED display.", "Android flagship tablet.", "galaxy-tab-s9", "[\r\n    {\"Name\":\"Display\",\"Value\":[\"11-inch AMOLED 120Hz\"]},\r\n    {\"Name\":\"Chip\",\"Value\":[\"Snapdragon 8 Gen 2\"]},\r\n    {\"Name\":\"RAM\",\"Value\":[\"8GB\",\"12GB\"]},\r\n    {\"Name\":\"Storage\",\"Value\":[\"128GB\",\"256GB\"]},\r\n    {\"Name\":\"OS\",\"Value\":[\"Android 14\"]}\r\n]", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "product",
                keyColumn: "Id",
                keyValue: 22,
                columns: new[] { "BasePrice", "BrandId", "CreatedAt", "Description", "Name", "Overview", "Slug", "Specs", "UpdatedAt" },
                values: new object[] { 419.00m, 5, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "Tab P12 supports stylus and multi-tasking.", "Lenovo Tab P12", "Affordable productivity tablet.", "lenovo-tab-p12", "[\r\n    {\"Name\":\"Display\",\"Value\":[\"12.7-inch LCD 144Hz\"]},\r\n    {\"Name\":\"Chip\",\"Value\":[\"MediaTek Dimensity 7050\"]},\r\n    {\"Name\":\"RAM\",\"Value\":[\"8GB\"]},\r\n    {\"Name\":\"Storage\",\"Value\":[\"128GB\"]},\r\n    {\"Name\":\"Battery\",\"Value\":[\"10200mAh\"]},\r\n    {\"Name\":\"OS\",\"Value\":[\"Android 13\"]}\r\n]", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "product",
                keyColumn: "Id",
                keyValue: 30,
                columns: new[] { "BasePrice", "CreatedAt", "Description", "Name", "Overview", "Slug", "Specs", "UpdatedAt" },
                values: new object[] { 699.00m, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "Color-accurate UltraSharp series for designers.", "Dell UltraSharp 27", "Professional 4K monitor.", "dell-ultrasharp-27", "[\r\n    {\"Name\":\"Display\",\"Value\":[\"27-inch IPS 4K UHD\"]},\r\n    {\"Name\":\"Resolution\",\"Value\":[\"3840x2160\"]},\r\n    {\"Name\":\"RefreshRate\",\"Value\":[\"60Hz\"]},\r\n    {\"Name\":\"Ports\",\"Value\":[\"HDMI\",\"DisplayPort\",\"USB-C\"]},\r\n    {\"Name\":\"ColorGamut\",\"Value\":[\"99% sRGB\"]},\r\n    {\"Name\":\"Warranty\",\"Value\":[\"24 months\"]}\r\n]", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "product",
                keyColumn: "Id",
                keyValue: 31,
                columns: new[] { "BasePrice", "CreatedAt", "Description", "Name", "Overview", "Slug", "Specs", "UpdatedAt" },
                values: new object[] { 450.00m, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "High-performance display for gamers.", "LG Ultragear 32", "Gaming monitor with 165Hz refresh rate.", "lg-ultragear-32", "[\r\n    {\"Name\":\"Display\",\"Value\":[\"32-inch VA QHD\"]},\r\n    {\"Name\":\"Resolution\",\"Value\":[\"2560x1440\"]},\r\n    {\"Name\":\"RefreshRate\",\"Value\":[\"165Hz\"]},\r\n    {\"Name\":\"Ports\",\"Value\":[\"HDMI\",\"DisplayPort\"]},\r\n    {\"Name\":\"Sync\",\"Value\":[\"G-Sync Compatible\"]}\r\n]", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "product",
                keyColumn: "Id",
                keyValue: 40,
                columns: new[] { "BasePrice", "CreatedAt", "Description", "Overview", "Specs", "UpdatedAt" },
                values: new object[] { 119.99m, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "Backlit keyboard with multi-device support.", "Wireless keyboard for professionals.", "[\r\n    {\"Name\":\"Type\",\"Value\":[\"Wireless\"]},\r\n    {\"Name\":\"Layout\",\"Value\":[\"Full-size\"]},\r\n    {\"Name\":\"Backlight\",\"Value\":[\"Yes\"]},\r\n    {\"Name\":\"Battery\",\"Value\":[\"USB-C rechargeable\"]}\r\n]", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "product",
                keyColumn: "Id",
                keyValue: 41,
                columns: new[] { "BasePrice", "CreatedAt", "Description", "Name", "Overview", "Slug", "Specs", "UpdatedAt" },
                values: new object[] { 169.99m, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "RGB lighting and tactile feedback.", "Razer BlackWidow V4", "Mechanical gaming keyboard.", "razer-blackwidow-v4", null, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "product",
                keyColumn: "Id",
                keyValue: 50,
                columns: new[] { "BasePrice", "CreatedAt", "Description", "Overview", "Specs", "UpdatedAt" },
                values: new object[] { 99.99m, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "Supports multiple devices with customizable buttons.", "Ergonomic productivity mouse.", "[\r\n    {\"Name\":\"Sensor\",\"Value\":[\"Logitech Darkfield\"]},\r\n    {\"Name\":\"Connection\",\"Value\":[\"Bluetooth\",\"USB\"]},\r\n    {\"Name\":\"Battery\",\"Value\":[\"70 days\"]},\r\n    {\"Name\":\"Buttons\",\"Value\":[\"7\"]}\r\n]", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "product",
                keyColumn: "Id",
                keyValue: 51,
                columns: new[] { "BasePrice", "CreatedAt", "Description", "Overview", "Specs", "UpdatedAt" },
                values: new object[] { 149.99m, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "Wireless mouse with high precision sensor.", "Ultra-light gaming mouse.", "[\r\n    {\"Name\":\"Sensor\",\"Value\":[\"Focus Pro 30K\"]},\r\n    {\"Name\":\"Weight\",\"Value\":[\"58g\"]},\r\n    {\"Name\":\"Connection\",\"Value\":[\"Wireless\"]},\r\n    {\"Name\":\"Battery\",\"Value\":[\"80h\"]}\r\n]", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "product",
                keyColumn: "Id",
                keyValue: 60,
                columns: new[] { "BasePrice", "CreatedAt", "Description", "Overview", "Specs", "UpdatedAt" },
                values: new object[] { 399.99m, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "Industry-leading noise cancellation for audio lovers.", "Noise-cancelling wireless headphones.", "[\r\n    {\"Name\":\"Type\",\"Value\":[\"Over-ear\"]},\r\n    {\"Name\":\"ANC\",\"Value\":[\"Yes\"]},\r\n    {\"Name\":\"Battery\",\"Value\":[\"30h\"]},\r\n    {\"Name\":\"Charging\",\"Value\":[\"USB-C\"]},\r\n    {\"Name\":\"Microphone\",\"Value\":[\"Yes\"]}\r\n]", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "product",
                keyColumn: "Id",
                keyValue: 61,
                columns: new[] { "BasePrice", "CreatedAt", "Description", "Overview", "Slug", "Specs", "UpdatedAt" },
                values: new object[] { 249.00m, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "Compact design and adaptive sound control.", "Wireless earbuds with active noise cancellation.", "airpods-pro-2", "[\r\n    {\"Name\":\"Type\",\"Value\":[\"In-ear\"]},\r\n    {\"Name\":\"ANC\",\"Value\":[\"Yes\"]},\r\n    {\"Name\":\"Battery\",\"Value\":[\"6h\",\"24h\"]},\r\n    {\"Name\":\"Wireless\",\"Value\":[\"Bluetooth 5.3\"]}\r\n]", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "product",
                keyColumn: "Id",
                keyValue: 70,
                columns: new[] { "BasePrice", "CreatedAt", "Description", "Name", "Overview", "Slug", "Specs", "UpdatedAt" },
                values: new object[] { 49.99m, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "Compact USB-C charger for laptops and phones.", "Anker 65W GaN Charger", "Fast charger with GaN technology.", "anker-65w-gan", "[\r\n    {\"Name\":\"Power\",\"Value\":[\"65W\"]},\r\n    {\"Name\":\"Ports\",\"Value\":[\"2x USB-C\",\"1x USB-A\"]},\r\n    {\"Name\":\"Material\",\"Value\":[\"GaN\"]},\r\n    {\"Name\":\"Input\",\"Value\":[\"100–240V\"]}\r\n]", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "product",
                keyColumn: "Id",
                keyValue: 71,
                columns: new[] { "BasePrice", "CreatedAt", "Description", "Name", "Overview", "Slug", "Specs", "UpdatedAt" },
                values: new object[] { 14.99m, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "Supports fast charging and data transfer.", "Baseus USB-C Cable 1.5m", "Durable braided charging cable.", "baseus-usb-c-cable", "[\r\n    {\"Name\":\"Length\",\"Value\":[\"1.5m\"]},\r\n    {\"Name\":\"Connector\",\"Value\":[\"USB-C to USB-C\"]},\r\n    {\"Name\":\"Material\",\"Value\":[\"Nylon braided\"]},\r\n    {\"Name\":\"MaxPower\",\"Value\":[\"100W\"]}\r\n]", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "product",
                keyColumn: "Id",
                keyValue: 80,
                columns: new[] { "BasePrice", "CreatedAt", "Description", "Name", "Overview", "Slug", "Specs", "UpdatedAt" },
                values: new object[] { 29.99m, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "Shock-absorbing TPU material.", "Spigen Rugged Armor Case", "Protective phone case.", "spigen-rugged-armor", "[\r\n    {\"Name\":\"Material\",\"Value\":[\"TPU\"]},\r\n    {\"Name\":\"ShockResistant\",\"Value\":[\"Yes\"]},\r\n    {\"Name\":\"CompatibleDevices\",\"Value\":[\"iPhone 15\"]}\r\n]", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "product",
                keyColumn: "Id",
                keyValue: 81,
                columns: new[] { "BasePrice", "CreatedAt", "Description", "Name", "Overview", "Slug", "Specs", "UpdatedAt" },
                values: new object[] { 49.99m, new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), "Designed for 13–15 inch laptops.", "UAG Plasma Laptop Sleeve", "Durable protective sleeve.", "uag-laptop-sleeve", "[\r\n    {\"Name\":\"Material\",\"Value\":[\"Ballistic Nylon\"]},\r\n    {\"Name\":\"Fits\",\"Value\":[\"13–15 inch laptops\"]},\r\n    {\"Name\":\"WaterResistant\",\"Value\":[\"Yes\"]}\r\n]", new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "product_variant",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Price", "Stock" },
                values: new object[] { new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), 1099.00m, 20 });

            migrationBuilder.UpdateData(
                table: "product_variant",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "Price", "Stock" },
                values: new object[] { new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), 1499.00m, 10 });

            migrationBuilder.UpdateData(
                table: "product_variant",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "Price", "Stock" },
                values: new object[] { new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), 999.00m, 15 });

            migrationBuilder.UpdateData(
                table: "product_variant",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "Price", "Stock" },
                values: new object[] { new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), 1299.00m, 8 });

            migrationBuilder.UpdateData(
                table: "product_variant",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAt", "Price", "Sku", "Stock" },
                values: new object[] { new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), 899.00m, "ZEN-I5-8-512", 25 });

            migrationBuilder.UpdateData(
                table: "product_variant",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CreatedAt", "Price", "Sku", "Stock" },
                values: new object[] { new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), 1199.00m, "ZEN-I7-16-1TB", 12 });

            migrationBuilder.UpdateData(
                table: "product_variant",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "CreatedAt", "Price", "Stock" },
                values: new object[] { new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), 1099.00m, 18 });

            migrationBuilder.UpdateData(
                table: "product_variant",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "CreatedAt", "Price", "Stock" },
                values: new object[] { new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), 1399.00m, 10 });

            migrationBuilder.UpdateData(
                table: "product_variant",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "CreatedAt", "Price", "Sku", "Stock" },
                values: new object[] { new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), 1199.00m, "X1-I5-8-512", 14 });

            migrationBuilder.UpdateData(
                table: "product_variant",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "CreatedAt", "Price", "Sku", "Stock" },
                values: new object[] { new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), 1499.00m, "X1-I7-16-1TB", 8 });

            migrationBuilder.UpdateData(
                table: "product_variant",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "CreatedAt", "Price", "Sku", "Stock" },
                values: new object[] { new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), 1199.00m, "IP15P-128", 20 });

            migrationBuilder.UpdateData(
                table: "product_variant",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "CreatedAt", "Price", "Sku" },
                values: new object[] { new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), 1299.00m, "IP15P-256" });

            migrationBuilder.UpdateData(
                table: "product_variant",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "CreatedAt", "Price", "ProductId", "Sku", "Stock" },
                values: new object[] { new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), 999.00m, 11, "S24-128", 25 });

            migrationBuilder.UpdateData(
                table: "product_variant",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "CreatedAt", "Price", "Sku", "Stock" },
                values: new object[] { new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), 1099.00m, "S24-256", 20 });

            migrationBuilder.UpdateData(
                table: "product_variant",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "CreatedAt", "Price", "ProductId", "Sku" },
                values: new object[] { new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), 899.00m, 12, "PIX9-128" });

            migrationBuilder.UpdateData(
                table: "product_variant",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "CreatedAt", "Price", "ProductId", "Sku", "Stock" },
                values: new object[] { new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), 999.00m, 12, "PIX9-256", 12 });

            migrationBuilder.UpdateData(
                table: "product_variant",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "CreatedAt", "Price", "ProductId", "Sku" },
                values: new object[] { new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), 1099.00m, 20, "IPADPRO-128" });

            migrationBuilder.UpdateData(
                table: "product_variant",
                keyColumn: "Id",
                keyValue: 18,
                columns: new[] { "CreatedAt", "Price", "ProductId", "Sku" },
                values: new object[] { new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), 1299.00m, 20, "IPADPRO-256" });

            migrationBuilder.UpdateData(
                table: "product_variant",
                keyColumn: "Id",
                keyValue: 19,
                columns: new[] { "CreatedAt", "Price", "ProductId", "Sku", "Stock" },
                values: new object[] { new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), 899.00m, 21, "TABS9-128", 25 });

            migrationBuilder.UpdateData(
                table: "product_variant",
                keyColumn: "Id",
                keyValue: 20,
                columns: new[] { "CreatedAt", "Price", "ProductId", "Sku", "Stock" },
                values: new object[] { new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), 999.00m, 21, "TABS9-256", 15 });

            migrationBuilder.UpdateData(
                table: "product_variant",
                keyColumn: "Id",
                keyValue: 21,
                columns: new[] { "CreatedAt", "Price", "ProductId", "Sku", "Stock" },
                values: new object[] { new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), 499.00m, 30, "DELL-U2720Q", 12 });

            migrationBuilder.UpdateData(
                table: "product_variant",
                keyColumn: "Id",
                keyValue: 22,
                columns: new[] { "CreatedAt", "Price", "ProductId", "Sku", "Stock" },
                values: new object[] { new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), 599.00m, 31, "LG-UG32", 10 });

            migrationBuilder.UpdateData(
                table: "product_variant",
                keyColumn: "Id",
                keyValue: 23,
                columns: new[] { "CreatedAt", "Price", "ProductId", "Sku", "Stock" },
                values: new object[] { new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), 119.00m, 40, "MX-KEYS-GRAY", 30 });

            migrationBuilder.UpdateData(
                table: "product_variant",
                keyColumn: "Id",
                keyValue: 24,
                columns: new[] { "CreatedAt", "Price", "ProductId", "Sku", "Stock" },
                values: new object[] { new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), 149.00m, 41, "RZR-BW-V4", 25 });

            migrationBuilder.UpdateData(
                table: "product_variant",
                keyColumn: "Id",
                keyValue: 25,
                columns: new[] { "CreatedAt", "Price", "ProductId", "Sku", "Stock" },
                values: new object[] { new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), 99.00m, 50, "MXM3S-GRAPHITE", 35 });

            migrationBuilder.UpdateData(
                table: "product_variant",
                keyColumn: "Id",
                keyValue: 26,
                columns: new[] { "CreatedAt", "Price", "ProductId", "Sku", "Stock" },
                values: new object[] { new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), 129.00m, 51, "RZR-V2PRO", 28 });

            migrationBuilder.UpdateData(
                table: "product_variant",
                keyColumn: "Id",
                keyValue: 27,
                columns: new[] { "CreatedAt", "Price", "ProductId", "Sku", "Stock" },
                values: new object[] { new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), 349.00m, 60, "SONY-XM5-BLK", 18 });

            migrationBuilder.UpdateData(
                table: "product_variant",
                keyColumn: "Id",
                keyValue: 28,
                columns: new[] { "CreatedAt", "Price", "ProductId", "Sku", "Stock" },
                values: new object[] { new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), 249.00m, 61, "AIRPODS-PRO2", 25 });

            migrationBuilder.UpdateData(
                table: "product_variant",
                keyColumn: "Id",
                keyValue: 29,
                columns: new[] { "CreatedAt", "Price", "ProductId", "Sku" },
                values: new object[] { new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), 59.00m, 70, "ANKER-65W" });

            migrationBuilder.UpdateData(
                table: "product_variant",
                keyColumn: "Id",
                keyValue: 30,
                columns: new[] { "CreatedAt", "Price", "ProductId", "Sku", "Stock" },
                values: new object[] { new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), 19.00m, 71, "BASEUS-CABLE-15", 100 });

            migrationBuilder.UpdateData(
                table: "product_variant",
                keyColumn: "Id",
                keyValue: 31,
                columns: new[] { "CreatedAt", "Price", "ProductId", "Sku", "Stock" },
                values: new object[] { new DateTime(2025, 10, 12, 9, 34, 0, 0, DateTimeKind.Utc), 29.00m, 80, "SPIGEN-RUGGED", 50 });

            migrationBuilder.UpdateData(
                table: "product_variant",
                keyColumn: "Id",
                keyValue: 32,
                columns: new[] { "CreatedAt", "Price", "ProductId", "Sku", "Stock" },
                values: new object[] { new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 49.00m, 81, "UAG-SLEEVE", 35 });
        }
    }
}
