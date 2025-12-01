using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Tekno.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class seedata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "blog_posts",
                columns: new[] { "Id", "AuthorId", "Content", "CreatedAt", "FeaturedImageUrl", "PublishedAt", "Slug", "Status", "Summary", "Title", "UpdatedAt", "ViewCount" },
                values: new object[,]
                {
                    { 1, 1, "<h2>Thi?t k? cao c?p v?i khung Titanium</h2>\r\n<p>iPhone 15 Pro Max là chi?c iPhone ??u tiên s? d?ng khung Titanium thay vì thép không g?, giúp gi?m tr?ng l??ng ?áng k? nh?ng v?n ??m b?o ?? b?n cao. Thi?t k? vi?n m?ng h?n, c?m giác c?m n?m tho?i mái h?n so v?i th? h? tr??c.</p>\r\n\r\n<h2>Hi?u n?ng ??nh cao v?i A17 Pro</h2>\r\n<p>Chip A17 Pro ???c s?n xu?t trên ti?n trình 3nm mang l?i hi?u n?ng v??t tr?i, x? lý m?i tác v? m??t mà t? gaming ??n ch?nh s?a video 4K. GPU m?i h? tr? ray tracing, m? ra k? nguyên gaming mobile m?i.</p>\r\n\r\n<h2>Camera 48MP v?i zoom 5x</h2>\r\n<p>H? th?ng camera ba ?ng kính v?i camera chính 48MP, telephoto zoom quang 5x mang ??n ch?t l??ng ?nh tuy?t v?i trong m?i ?i?u ki?n ánh sáng. Ch? ?? ch?p ?êm Night Mode ???c c?i thi?n ?áng k?.</p>\r\n\r\n<h2>Pin trâu, s?c nhanh USB-C</h2>\r\n<p>Pin 4422mAh s? d?ng c? ngày dài, cu?i cùng Apple c?ng chuy?n sang c?ng USB-C theo chu?n châu Âu. H? tr? s?c nhanh 27W và s?c không dây MagSafe 15W.</p>\r\n\r\n<h2>K?t lu?n</h2>\r\n<p>iPhone 15 Pro Max x?ng ?áng là chi?c flagship ?áng mua nh?t n?m 2024 v?i thi?t k? cao c?p, hi?u n?ng m?nh m? và h? th?ng camera xu?t s?c. Giá 33,990,000 VND là h?p lý cho nh?ng gì Apple mang l?i.</p>", new DateTime(2025, 1, 9, 14, 30, 0, 0, DateTimeKind.Utc), "https://www.gstatic.com/webp/gallery/1.jpg", new DateTime(2025, 1, 10, 9, 0, 0, 0, DateTimeKind.Utc), "danh-gia-iphone-15-pro-max", "Published", "iPhone 15 Pro Max ?ánh d?u b??c ti?n m?i v?i chip A17 Pro, khung Titanium siêu b?n và camera 48MP v?i zoom quang 5x. Cùng khám phá chi ti?t flagship m?i nh?t t? Apple.", "?ánh giá chi ti?t iPhone 15 Pro Max: ??nh cao công ngh? t? Apple", new DateTime(2025, 1, 10, 8, 45, 0, 0, DateTimeKind.Utc), 1250 },
                    { 2, 2, "<h2>Thi?t k? và ch?t l??ng build</h2>\r\n<p>MacBook Air M2 v?i thi?t k? vuông v?n hi?n ??i, ?? m?ng ch? 11.3mm và tr?ng l??ng 1.24kg. Dell XPS 13 không kém c?nh v?i thi?t k? siêu m?ng 13.9mm, vi?n màn hình InfinityEdge ?n t??ng.</p>\r\n\r\n<h2>Hi?u n?ng và pin</h2>\r\n<p>Chip M2 8-core mang l?i hi?u n?ng v??t tr?i, ??c bi?t v?i các ?ng d?ng macOS native. Pin s? d?ng lên ??n 18 gi?. Dell XPS 13 v?i Intel Core i5/i7 th? h? 13 m?nh m? cho Windows, pin kho?ng 12 gi?.</p>\r\n\r\n<h2>Màn hình</h2>\r\n<p>MacBook Air M2: 13.6 inch Liquid Retina (2560x1664), ?? sáng 500 nits. Dell XPS 13: 13.4 inch FHD+ (1920x1200), màn hình InfinityEdge vi?n siêu m?ng.</p>\r\n\r\n<h2>Giá c?</h2>\r\n<p>MacBook Air M2 8GB/256GB: 28,990,000 VND. Dell XPS 13 i5/8GB/512GB: 25,990,000 VND. Dell có giá t?t h?n v?i SSD 512GB ngay t? ??u.</p>\r\n\r\n<h2>K?t lu?n</h2>\r\n<p>Ch?n MacBook Air M2 n?u b?n ?u tiên pin trâu, h? sinh thái Apple. Ch?n Dell XPS 13 n?u c?n Windows, giá t?t h?n và thi?t k? ??p m?t.</p>", new DateTime(2025, 1, 11, 15, 20, 0, 0, DateTimeKind.Utc), "https://www.gstatic.com/webp/gallery/2.jpg", new DateTime(2025, 1, 12, 10, 30, 0, 0, DateTimeKind.Utc), "so-sanh-macbook-air-m2-vs-dell-xps-13", "Published", "Hai chi?c laptop cao c?p ???c yêu thích nh?t hi?n nay. MacBook Air M2 v?i hi?u n?ng chip ARM xu?t s?c hay Dell XPS 13 v?i thi?t k? tinh t? và Windows 11? Cùng phân tích chi ti?t.", "So sánh MacBook Air M2 vs Dell XPS 13: Nên ch?n laptop nào?", new DateTime(2025, 1, 12, 10, 15, 0, 0, DateTimeKind.Utc), 890 },
                    { 3, 1, "<h2>Thi?t k? sang tr?ng, S Pen tích h?p</h2>\r\n<p>Thi?t k? vuông v?n m?nh m?, khung nhôm cao c?p. S Pen tích h?p trong thân máy mang l?i tr?i nghi?m ghi chú, v? tuy?t v?i - tính n?ng ??c quy?n c?a dòng Ultra.</p>\r\n\r\n<h2>Màn hình ??nh cao</h2>\r\n<p>Màn hình 6.8 inch Dynamic AMOLED 2X v?i ?? phân gi?i QHD+ (3120x1440), t?n s? quét 120Hz, ?? sáng ??nh 2600 nits - rõ nét ngay c? d??i n?ng g?t.</p>\r\n\r\n<h2>Camera 200MP chuyên nghi?p</h2>\r\n<p>Camera chính 200MP v?i OIS, camera telephoto kép (3x và 5x), camera ultra wide 12MP. Ch?t l??ng ?nh xu?t s?c, zoom 10x v?n gi? chi ti?t t?t.</p>\r\n\r\n<h2>Hi?u n?ng m?nh m? Snapdragon 8 Gen 3</h2>\r\n<p>Chip Snapdragon 8 Gen 3 for Galaxy t?i ?u riêng cho Samsung, hi?u n?ng v??t tr?i, ch?i game m??t mà, pin 5000mAh s? d?ng tho?i mái c? ngày.</p>\r\n\r\n<h2>?ánh giá</h2>\r\n<p>Galaxy S24 Ultra là l?a ch?n hàng ??u cho ai mu?n flagship Android hoàn h?o nh?t. Giá 29,990,000 VND x?ng ?áng v?i nh?ng gì Samsung mang l?i.</p>", new DateTime(2025, 1, 12, 16, 45, 0, 0, DateTimeKind.Utc), "https://www.gstatic.com/webp/gallery/3.jpg", new DateTime(2025, 1, 13, 11, 0, 0, 0, DateTimeKind.Utc), "samsung-galaxy-s24-ultra-review", "Published", "Galaxy S24 Ultra ti?p t?c kh?ng ??nh v? th? d?n ??u phân khúc flagship Android v?i camera 200MP, S Pen tích h?p và màn hình Dynamic AMOLED 2X tuy?t ??p.", "Samsung Galaxy S24 Ultra: Android flagship t?t nh?t v?i camera 200MP", new DateTime(2025, 1, 13, 10, 30, 0, 0, DateTimeKind.Utc), 1100 },
                    { 4, 2, "<h2>1. Razer BlackWidow V4 Pro - Bàn phím c? gaming cao c?p</h2>\r\n<p>Bàn phím c? v?i switch Razer Green Clicky, ?èn RGB Chroma per-key, Command Dial ti?n l?i. Giá 5,990,000 VND cho tr?i nghi?m gõ tuy?t v?i và ?? b?n cao.</p>\r\n\r\n<h2>2. Razer Viper V2 Pro - Chu?t gaming không dây siêu nh?</h2>\r\n<p>Ch? 58g nh?ng ??y ?? tính n?ng: sensor Focus Pro 30K DPI, optical switch Gen 3, pin 80 gi?. Thi?t k? ambidextrous phù h?p m?i ki?u c?m. Giá 3,990,000 VND.</p>\r\n\r\n<h2>3. LG UltraGear 27GN800 - Màn hình gaming QHD 144Hz</h2>\r\n<p>27 inch QHD (2560x1440) v?i t?n s? quét 144Hz, t?m n?n IPS Nano Color, h? tr? G-Sync. Giá 8,490,000 VND cho hình ?nh m??t mà, màu s?c chính xác.</p>\r\n\r\n<h2>4. Sony WH-1000XM5 - Tai nghe ch?ng ?n t?t nh?t</h2>\r\n<p>Tuy không ph?i tai nghe gaming chuyên d?ng nh?ng ch?t âm Hi-Res, ANC xu?t s?c phù h?p cho game single-player immersive. Giá 8,990,000 VND.</p>\r\n\r\n<h2>5. Anker 747 GaNPrime 150W - S?c nhanh ?a n?ng</h2>\r\n<p>S?c laptop gaming, ?i?n tho?i, tai nghe cùng lúc v?i 4 c?ng (3 USB-C + 1 USB-A). Công ngh? GaN nh? g?n, công su?t 150W. Giá 2,490,000 VND.</p>\r\n\r\n<h2>T?ng k?t</h2>\r\n<p>??u t? vào ph? ki?n ch?t l??ng s? nâng cao tr?i nghi?m gaming ?áng k?. T?ng chi phí setup hoàn h?o kho?ng 30 tri?u VND.</p>", new DateTime(2025, 1, 13, 17, 0, 0, 0, DateTimeKind.Utc), "https://www.gstatic.com/webp/gallery/4.jpg", new DateTime(2025, 1, 14, 14, 0, 0, 0, DateTimeKind.Utc), "top-5-phu-kien-gaming-tot-nhat-2025", "Published", "T? bàn phím c? ??n chu?t gaming, tai nghe và màn hình - nh?ng ph? ki?n gaming này s? nâng t?m tr?i nghi?m ch?i game c?a b?n lên m?t level hoàn toàn m?i.", "Top 5 ph? ki?n gaming t?t nh?t cho setup chuyên nghi?p 2025", new DateTime(2025, 1, 14, 13, 45, 0, 0, DateTimeKind.Utc), 750 },
                    { 5, 1, "<h2>Xiaomi Pad 6 - Giá tr? t?t nh?t</h2>\r\n<p>Màn hình 11 inch LCD 2.8K v?i t?n s? quét 144Hz, chip Snapdragon 870 v?n m?nh m? cho ?a nhi?m và ch?i game. Pin 8840mAh s? d?ng c? ngày. Giá ch? 8,990,000 VND.</p>\r\n\r\n<h2>iPad Air M2 - Hi?u n?ng cao c?p</h2>\r\n<p>Chip M2 m?nh m? nh? MacBook, màn hình Liquid Retina 11 inch, h? tr? Apple Pencil 2. H? sinh thái iPadOS v?i hàng tri?u app t?i ?u. Giá 16,990,000 VND.</p>\r\n\r\n<h2>So sánh chi ti?t</h2>\r\n<ul>\r\n<li><strong>Hi?u n?ng:</strong> M2 m?nh h?n rõ r?t, nh?ng Snapdragon 870 ?? dùng</li>\r\n<li><strong>Màn hình:</strong> Xiaomi có t?n s? quét 144Hz, iPad có màu s?c chính xác h?n</li>\r\n<li><strong>H? ?i?u hành:</strong> iPadOS nhi?u app h?n, MIUI Pad tùy bi?n cao</li>\r\n<li><strong>Giá:</strong> Xiaomi r? h?n g?n m?t n?a</li>\r\n</ul>\r\n\r\n<h2>K?t lu?n</h2>\r\n<p>Ch?n Xiaomi Pad 6 n?u ngân sách h?n ch?, dùng ?? xem phim, ??c sách, ch?i game nh?. Ch?n iPad Air M2 n?u c?n hi?u n?ng cao cho công vi?c, v? digital art ho?c mu?n h? sinh thái Apple.</p>", new DateTime(2025, 1, 14, 18, 30, 0, 0, DateTimeKind.Utc), "https://www.gstatic.com/webp/gallery/5.jpg", new DateTime(2025, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), "tablet-gia-re-dang-mua-2025", "Published", "So sánh hai chi?c tablet t?m trung hot nh?t: Xiaomi Pad 6 giá ch? 8,990,000 VND và iPad Air M2 giá 16,990,000 VND. ?âu là l?a ch?n phù h?p v?i b?n?", "Tablet giá r? ?áng mua 2025: Xiaomi Pad 6 vs iPad Air M2", new DateTime(2025, 1, 15, 8, 45, 0, 0, DateTimeKind.Utc), 520 }
                });

            migrationBuilder.InsertData(
                table: "blog_posts",
                columns: new[] { "Id", "AuthorId", "Content", "CreatedAt", "FeaturedImageUrl", "PublishedAt", "Slug", "Status", "Summary", "Title", "UpdatedAt" },
                values: new object[] { 6, 2, "<h2>Nhu c?u theo ngành h?c</h2>\r\n<p>N?i dung ?ang ???c c?p nh?t...</p>", new DateTime(2025, 1, 15, 9, 15, 0, 0, DateTimeKind.Utc), "https://www.gstatic.com/webp/gallery/1.jpg", new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), "bi-quyet-chon-laptop-cho-sinh-vien", "Draft", "H??ng d?n chi ti?t giúp sinh viên ch?n laptop phù h?p v?i ngành h?c và ngân sách. T? sinh viên v?n phòng ??n k? thu?t, thi?t k? - ??u có g?i ý c? th?.", "Bí quy?t ch?n laptop cho sinh viên: C?u hình nào là ???", new DateTime(2025, 1, 15, 9, 45, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.InsertData(
                table: "product_advertisements",
                columns: new[] { "Id", "CreatedAt", "EndDate", "ImageUrl", "IsActive", "Position", "Priority", "ProductId", "StartDate" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 3, 31, 23, 59, 59, 0, DateTimeKind.Utc), "https://www.gstatic.com/webp/gallery/1.jpg", true, "HomeTop", 100, 10, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 2, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 3, 31, 23, 59, 59, 0, DateTimeKind.Utc), "https://www.gstatic.com/webp/gallery/2.jpg", true, "HomeTop", 90, 11, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 6, 30, 23, 59, 59, 0, DateTimeKind.Utc), "https://www.gstatic.com/webp/gallery/3.jpg", true, "HomeTop", 85, 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 4, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 4, 30, 23, 59, 59, 0, DateTimeKind.Utc), "https://www.gstatic.com/webp/gallery/4.jpg", true, "HomeMiddle", 80, 20, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 5, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 4, 30, 23, 59, 59, 0, DateTimeKind.Utc), "https://www.gstatic.com/webp/gallery/5.jpg", true, "HomeMiddle", 75, 21, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 6, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 12, 31, 23, 59, 59, 0, DateTimeKind.Utc), "https://www.gstatic.com/webp/gallery/1.jpg", true, "HomeBottom", 70, 60, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 7, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 12, 31, 23, 59, 59, 0, DateTimeKind.Utc), "https://www.gstatic.com/webp/gallery/2.jpg", true, "HomeBottom", 65, 61, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 8, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 6, 30, 23, 59, 59, 0, DateTimeKind.Utc), "https://www.gstatic.com/webp/gallery/3.jpg", true, "CategoryTop", 90, 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 9, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 6, 30, 23, 59, 59, 0, DateTimeKind.Utc), "https://www.gstatic.com/webp/gallery/4.jpg", true, "CategoryTop", 85, 3, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 10, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 12, 31, 23, 59, 59, 0, DateTimeKind.Utc), "https://www.gstatic.com/webp/gallery/5.jpg", true, "CategoryMiddle", 80, 30, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 11, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 12, 31, 23, 59, 59, 0, DateTimeKind.Utc), "https://www.gstatic.com/webp/gallery/1.jpg", true, "CategoryMiddle", 75, 31, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 12, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 12, 31, 23, 59, 59, 0, DateTimeKind.Utc), "https://www.gstatic.com/webp/gallery/2.jpg", true, "ProductSidebar", 70, 40, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 13, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 12, 31, 23, 59, 59, 0, DateTimeKind.Utc), "https://www.gstatic.com/webp/gallery/3.jpg", true, "ProductSidebar", 65, 50, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 14, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 12, 31, 23, 59, 59, 0, DateTimeKind.Utc), "https://www.gstatic.com/webp/gallery/4.jpg", true, "ProductSidebar", 60, 70, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 15, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 3, 31, 23, 59, 59, 0, DateTimeKind.Utc), "https://www.gstatic.com/webp/gallery/5.jpg", true, "SearchTop", 85, 13, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 16, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 3, 31, 23, 59, 59, 0, DateTimeKind.Utc), "https://www.gstatic.com/webp/gallery/1.jpg", true, "SearchTop", 80, 14, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "product_advertisements",
                columns: new[] { "Id", "CreatedAt", "EndDate", "ImageUrl", "Position", "Priority", "ProductId", "StartDate" },
                values: new object[] { 17, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 12, 31, 23, 59, 59, 0, DateTimeKind.Utc), "https://www.gstatic.com/webp/gallery/2.jpg", "HomeTop", 95, 12, new DateTime(2024, 12, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.InsertData(
                table: "product_advertisements",
                columns: new[] { "Id", "CreatedAt", "EndDate", "ImageUrl", "IsActive", "Position", "Priority", "ProductId", "StartDate" },
                values: new object[] { 18, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 12, 31, 23, 59, 59, 0, DateTimeKind.Utc), "https://www.gstatic.com/webp/gallery/3.jpg", true, "CategoryTop", 88, 4, new DateTime(2024, 11, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 1,
                column: "ImageUrl",
                value: "https://cdn2.cellphones.com.vn/insecure/rs:fill:358:358/q:90/plain/https://cellphones.com.vn/media/catalog/product/g/r/group_659_40.png");

            migrationBuilder.UpdateData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 2,
                column: "ImageUrl",
                value: "https://cdn.tgdd.vn/Products/Images/44/314838/dell-xps-13-plus-9320-i5-71013325-1-750x500.jpg");

            migrationBuilder.UpdateData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 3,
                column: "ImageUrl",
                value: "https://cdn2.cellphones.com.vn/insecure/rs:fill:358:358/q:90/plain/https://cellphones.com.vn/media/catalog/product/m/a/macbook_1__1_8.png");

            migrationBuilder.UpdateData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 4,
                column: "ImageUrl",
                value: "https://store.storeimages.cdn-apple.com/8756/as-images.apple.com/is/mba13-midnight-select-202402?wid=904&hei=840&fmt=jpeg&qlt=90&.v=1708367688034");

            migrationBuilder.UpdateData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 5,
                column: "ImageUrl",
                value: "https://cdn2.cellphones.com.vn/insecure/rs:fill:358:358/q:90/plain/https://cellphones.com.vn/media/catalog/product/t/e/text_ng_n_24__3_5.png");

            migrationBuilder.UpdateData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 6,
                column: "ImageUrl",
                value: "https://dlcdnwebimgs.asus.com/gain/838fbdac-6d10-4190-8e52-d4b9463f5d23/");

            migrationBuilder.UpdateData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 7,
                column: "ImageUrl",
                value: "https://cdn2.cellphones.com.vn/insecure/rs:fill:0:358/q:90/plain/https://cellphones.com.vn/media/catalog/product/l/a/laptop-hp-spectre-x360-14-ef0030tu-6k773pa-cu-dep-1_4.png");

            migrationBuilder.UpdateData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 8,
                column: "ImageUrl",
                value: "https://www.hp.com/content/dam/sites/worldwide/personal-computers/consumer/laptops-and-2-n-1s/spectre/version-2023/HP%20Spectre%20x360%2014__Mobile@2x.png");

            migrationBuilder.UpdateData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 9,
                column: "ImageUrl",
                value: "https://cdn2.cellphones.com.vn/insecure/rs:fill:358:358/q:90/plain/https://cellphones.com.vn/media/catalog/product/g/r/group_744_2__7.png");

            migrationBuilder.UpdateData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 10,
                column: "ImageUrl",
                value: "https://mac24h.vn/images/detailed/94/ThinkPad_X1_Carbon_Gen_11.png");

            migrationBuilder.UpdateData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 12,
                column: "ImageUrl",
                value: "https://www.apple.com/v/iphone-17-pro/d/images/overview/contrast/iphone_air__fe2gdmh5u5qy_large_2x.jpg");

            migrationBuilder.UpdateData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 13,
                column: "ImageUrl",
                value: "https://baotinmobile.vn/uploads/2024/02/s24-ultra-tim.jpg");

            migrationBuilder.UpdateData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 14,
                column: "ImageUrl",
                value: "https://happyphone.vn/wp-content/uploads/2024/04/SAMSUNG-GALAXY-S24-ULTRA-12GB-512GB-Cam.jpg");

            migrationBuilder.UpdateData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 15,
                column: "ImageUrl",
                value: "https://www.didongmy.com/vnt_upload/product/10_2023/pixel8/thumbs/600_crop_google-pixel-8-pro-obsidian-thumb-didongmy-600x600.jpg");

            migrationBuilder.UpdateData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 16,
                column: "ImageUrl",
                value: "https://cdn.tgdd.vn/Products/Images/42/307188/google-pixel-8-pro-600x600.jpg");

            migrationBuilder.UpdateData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 17,
                column: "ImageUrl",
                value: "https://cdn2.cellphones.com.vn/insecure/rs:fill:0:358/q:90/plain/https://cellphones.com.vn/media/catalog/product/x/i/xiaomi-14_4.png");

            migrationBuilder.UpdateData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 18,
                column: "ImageUrl",
                value: "https://cdn.mobilecity.vn/mobilecity-vn/images/2023/10/xiaomi-14-hong.jpg.webp");

            migrationBuilder.UpdateData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 19,
                column: "ImageUrl",
                value: "https://www.duchuymobile.com/images/detailed/65/oneplus-12-trang_ucuo-lm.jpg");

            migrationBuilder.UpdateData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 20,
                column: "ImageUrl",
                value: "https://cdn2.cellphones.com.vn/x/media/catalog/product/o/n/oneplus-12_1_.jpg");

            migrationBuilder.UpdateData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 21,
                column: "ImageUrl",
                value: "https://traidepbaniphone.com/upload/product/ipadpro11-inwi-fisilver2-upscreenusen-3047.png");

            migrationBuilder.UpdateData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 22,
                column: "ImageUrl",
                value: "https://phucanhcdn.com/media/product/49293_cellular_512gb.jpg");

            migrationBuilder.UpdateData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 23,
                column: "ImageUrl",
                value: "https://product.hstatic.net/1000379731/product/mul3dutway0g35ul8rlp_bc0427d5a0594b43820baccffe69c71b.png");

            migrationBuilder.UpdateData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 24,
                column: "ImageUrl",
                value: "https://lh6.googleusercontent.com/proxy/mbdwj7VJ7KF0K0HYg2U_TBtAhgH4BBwl8w4ngxSsHSzI1psonPR0hpi1Jf7hFerv42m6zMkx5XEuXnhvggCUs5E8SiWyL7bjXC9f0iOa0i_vOotYHaCd71ywDccS");

            migrationBuilder.UpdateData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 25,
                column: "ImageUrl",
                value: "https://phukienpico.com/wp-content/uploads/2023/09/Op-lung-bao-da-xiaomi-pad-6-pro-10.jpg");

            migrationBuilder.UpdateData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 26,
                column: "ImageUrl",
                value: "https://cdn.tgdd.vn/Products/Images/522/309848/Kit/xiaomi-pad-6-note-1-1.jpg");

            migrationBuilder.UpdateData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 27,
                column: "ImageUrl",
                value: "https://product.hstatic.net/200000637319/product/1_dcfa8a17409f453cae523f6894013556_master.jpg");

            migrationBuilder.UpdateData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 28,
                column: "ImageUrl",
                value: "https://pcmarket.vn/media/lib/29-06-2022/27gn800-b4.jpg");

            migrationBuilder.UpdateData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 29,
                column: "ImageUrl",
                value: "https://cdn2.cellphones.com.vn/insecure/rs:fill:358:358/q:90/plain/https://cellphones.com.vn/media/catalog/product/g/a/gaming_8_14__1.png");

            migrationBuilder.UpdateData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 30,
                column: "ImageUrl",
                value: "https://product.hstatic.net/200000637319/product/mx-keys-mini-top-rose-us_a6b9661eb3424f1c8d79503e2cc3e0e7_master.png");

            migrationBuilder.UpdateData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 31,
                column: "ImageUrl",
                value: "https://product.hstatic.net/200000637319/product/81eeknarvil._ac_sl1500__b82e73d82da2451ca567fb128494d6aa_master.jpg");

            migrationBuilder.UpdateData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 32,
                column: "ImageUrl",
                value: "https://cdn2.cellphones.com.vn/insecure/rs:fill:0:358/q:90/plain/https://cellphones.com.vn/media/catalog/product/c/h/chuot-khong-day-logitech-mx-master-3s-for-mac_2.png");

            migrationBuilder.UpdateData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 34,
                column: "ImageUrl",
                value: "https://cdnv2.tgdd.vn/mwg-static/tgdd/Products/Images/86/357719/chuot-sac-khong-day-gaming-razer-viper-v3-pro-thumb-638967440293053901-600x600.jpg");

            migrationBuilder.UpdateData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 36,
                column: "ImageUrl",
                value: "https://cdn.tgdd.vn/Products/Images/54/313692/tai-nghe-bluetooth-chup-tai-sony-wh1000xm5-trang-1-750x500.jpg");

            migrationBuilder.UpdateData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 38,
                column: "ImageUrl",
                value: "https://photo2.tinhte.vn/data/attachment-files/2022/07/6065972_anker-GaNPrime-tinhte-4.png");

            migrationBuilder.UpdateData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 39,
                column: "ImageUrl",
                value: "https://bizweb.dktcdn.net/thumb/large/100/462/529/products/0-1-1713511388939.jpg?v=1713511395120");

            migrationBuilder.UpdateData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 40,
                column: "ImageUrl",
                value: "https://m.media-amazon.com/images/I/81osE87mFrL.jpg");

            migrationBuilder.UpdateData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 41,
                column: "ImageUrl",
                value: "https://cdn2.cellphones.com.vn/x/media/catalog/product/t/o/tomtoc-slim-tui-chong-soc-1.png");

            migrationBuilder.UpdateData(
                table: "user",
                keyColumn: "Id",
                keyValue: 1,
                column: "PhoneNumber",
                value: "0901234567");

            migrationBuilder.UpdateData(
                table: "user",
                keyColumn: "Id",
                keyValue: 2,
                column: "PhoneNumber",
                value: "0912345678");

            migrationBuilder.InsertData(
                table: "user_addresses",
                columns: new[] { "Id", "AddressLine1", "AddressLine2", "City", "Country", "CreatedAt", "IsDefault", "PhoneNumber", "PostalCode", "RecipientName", "State", "UpdatedAt", "UserId" },
                values: new object[] { 1, "123 Nguyen Hue Street", "Ben Nghe Ward", "District 1", "Vietnam", new DateTime(2025, 1, 10, 10, 0, 0, 0, DateTimeKind.Utc), true, "0912345678", "700000", "Customer User", "Ho Chi Minh City", null, 2 });

            migrationBuilder.InsertData(
                table: "user_addresses",
                columns: new[] { "Id", "AddressLine1", "AddressLine2", "City", "Country", "CreatedAt", "PhoneNumber", "PostalCode", "RecipientName", "State", "UpdatedAt", "UserId" },
                values: new object[] { 2, "456 Le Loi Boulevard", "Ben Thanh Ward", "District 1", "Vietnam", new DateTime(2025, 1, 10, 10, 0, 0, 0, DateTimeKind.Utc), "0912345678", "700000", "Customer User", "Ho Chi Minh City", null, 2 });

            migrationBuilder.InsertData(
                table: "user_carts",
                columns: new[] { "Id", "CreatedAt", "UpdatedAt", "UserId" },
                values: new object[] { 1, new DateTime(2025, 1, 12, 9, 30, 0, 0, DateTimeKind.Utc), new DateTime(2025, 1, 14, 15, 20, 0, 0, DateTimeKind.Utc), 2 });

            migrationBuilder.InsertData(
                table: "wishlists",
                columns: new[] { "Id", "AddedAt", "UserId", "VariantId" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 1, 10, 9, 0, 0, 0, DateTimeKind.Utc), 2, 2 },
                    { 2, new DateTime(2025, 1, 11, 14, 30, 0, 0, DateTimeKind.Utc), 2, 27 },
                    { 3, new DateTime(2025, 1, 12, 16, 45, 0, 0, DateTimeKind.Utc), 2, 31 }
                });

            migrationBuilder.InsertData(
                table: "blog_post_products",
                columns: new[] { "Id", "BlogPostId", "ProductId" },
                values: new object[,]
                {
                    { 1, 1, 10 },
                    { 2, 1, 61 },
                    { 3, 1, 80 },
                    { 4, 2, 2 },
                    { 5, 2, 1 },
                    { 6, 2, 81 },
                    { 7, 3, 11 },
                    { 8, 3, 70 },
                    { 9, 3, 80 },
                    { 10, 4, 41 },
                    { 11, 4, 51 },
                    { 12, 4, 31 },
                    { 13, 4, 60 },
                    { 14, 4, 70 },
                    { 15, 5, 22 },
                    { 16, 5, 20 },
                    { 17, 5, 21 }
                });

            migrationBuilder.InsertData(
                table: "blog_post_tags",
                columns: new[] { "Id", "BlogPostId", "Tag" },
                values: new object[,]
                {
                    { 1, 1, "review" },
                    { 2, 1, "iphone" },
                    { 3, 1, "apple" },
                    { 4, 1, "flagship" },
                    { 5, 1, "smartphone" },
                    { 6, 2, "comparison" },
                    { 7, 2, "laptop" },
                    { 8, 2, "macbook" },
                    { 9, 2, "dell" },
                    { 10, 3, "review" },
                    { 11, 3, "samsung" },
                    { 12, 3, "android" },
                    { 13, 3, "flagship" },
                    { 14, 3, "camera" },
                    { 15, 4, "gaming" },
                    { 16, 4, "peripherals" },
                    { 17, 4, "accessories" },
                    { 18, 4, "top-list" },
                    { 19, 5, "comparison" },
                    { 20, 5, "tablet" },
                    { 21, 5, "budget" },
                    { 22, 5, "xiaomi" },
                    { 23, 5, "ipad" },
                    { 24, 6, "guide" },
                    { 25, 6, "student" },
                    { 26, 6, "laptop" }
                });

            migrationBuilder.InsertData(
                table: "cart_items",
                columns: new[] { "Id", "AddedAt", "CartId", "Price", "Quantity", "VariantId" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 1, 12, 9, 30, 0, 0, DateTimeKind.Utc), 1, 33990000m, 1, 11 },
                    { 2, new DateTime(2025, 1, 13, 14, 15, 0, 0, DateTimeKind.Utc), 1, 6490000m, 1, 42 },
                    { 3, new DateTime(2025, 1, 14, 15, 20, 0, 0, DateTimeKind.Utc), 1, 490000m, 1, 46 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "blog_post_products",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "blog_post_products",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "blog_post_products",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "blog_post_products",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "blog_post_products",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "blog_post_products",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "blog_post_products",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "blog_post_products",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "blog_post_products",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "blog_post_products",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "blog_post_products",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "blog_post_products",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "blog_post_products",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "blog_post_products",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "blog_post_products",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "blog_post_products",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "blog_post_products",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "blog_post_tags",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "blog_post_tags",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "blog_post_tags",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "blog_post_tags",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "blog_post_tags",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "blog_post_tags",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "blog_post_tags",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "blog_post_tags",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "blog_post_tags",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "blog_post_tags",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "blog_post_tags",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "blog_post_tags",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "blog_post_tags",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "blog_post_tags",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "blog_post_tags",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "blog_post_tags",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "blog_post_tags",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "blog_post_tags",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "blog_post_tags",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "blog_post_tags",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "blog_post_tags",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "blog_post_tags",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "blog_post_tags",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "blog_post_tags",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "blog_post_tags",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "blog_post_tags",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "cart_items",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "cart_items",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "cart_items",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "product_advertisements",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "product_advertisements",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "product_advertisements",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "product_advertisements",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "product_advertisements",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "product_advertisements",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "product_advertisements",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "product_advertisements",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "product_advertisements",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "product_advertisements",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "product_advertisements",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "product_advertisements",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "product_advertisements",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "product_advertisements",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "product_advertisements",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "product_advertisements",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "product_advertisements",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "product_advertisements",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "user_addresses",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "user_addresses",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "wishlists",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "wishlists",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "wishlists",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "blog_posts",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "blog_posts",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "blog_posts",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "blog_posts",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "blog_posts",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "blog_posts",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "user_carts",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.UpdateData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 1,
                column: "ImageUrl",
                value: "https://i.dell.com/is/image/DellContent/content/dam/ss2/product-images/dell-client-products/notebooks/xps-notebooks/13-9315/media-gallery/notebook-xps-9315-nt-blue-gallery-4.psd?fmt=png-alpha&pscan=auto&scl=1&hei=804&wid=1180&qlt=100,1&resMode=sharp2&size=1180,804&chrss=full");

            migrationBuilder.UpdateData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 2,
                column: "ImageUrl",
                value: "https://i.dell.com/is/image/DellContent/content/dam/ss2/product-images/dell-client-products/notebooks/xps-notebooks/13-9315/media-gallery/notebook-xps-9315-nt-blue-gallery-2.psd?fmt=png-alpha&pscan=auto&scl=1&hei=804&wid=1534&qlt=100,1&resMode=sharp2&size=1534,804&chrss=full");

            migrationBuilder.UpdateData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 3,
                column: "ImageUrl",
                value: "https://store.storeimages.cdn-apple.com/8756/as-images.apple.com/is/mba13-midnight-select-202402?wid=904&hei=840&fmt=jpeg&qlt=90&.v=1708367688034");

            migrationBuilder.UpdateData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 4,
                column: "ImageUrl",
                value: "https://store.storeimages.cdn-apple.com/8756/as-images.apple.com/is/macbook-air-midnight-config-202402?wid=820&hei=498&fmt=jpeg&qlt=90&.v=1708367398169");

            migrationBuilder.UpdateData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 5,
                column: "ImageUrl",
                value: "https://dlcdnwebimgs.asus.com/gain/8E4B3A6D-5F51-465F-8B7E-BD8BE8C77C52/w717/h525");

            migrationBuilder.UpdateData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 6,
                column: "ImageUrl",
                value: "https://dlcdnwebimgs.asus.com/gain/82CB6B3D-6C1D-4A7E-9A0C-1D9F3E8B8C8D/w717/h525");

            migrationBuilder.UpdateData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 7,
                column: "ImageUrl",
                value: "https://ssl-product-images.www8-hp.com/digmedialib/prodimg/lowres/c08111766.png");

            migrationBuilder.UpdateData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 8,
                column: "ImageUrl",
                value: "https://ssl-product-images.www8-hp.com/digmedialib/prodimg/lowres/c08111768.png");

            migrationBuilder.UpdateData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 9,
                column: "ImageUrl",
                value: "https://p3-ofp.static.pub/fes/cms/2023/03/13/exh3cw4xp3hqh5w22bkv5gxkd00ozq528090.png");

            migrationBuilder.UpdateData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 10,
                column: "ImageUrl",
                value: "https://p1-ofp.static.pub/fes/cms/2023/03/13/ulhzrk7v2chz1bmz5aatsjluwrv24c287126.png");

            migrationBuilder.UpdateData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 12,
                column: "ImageUrl",
                value: "https://store.storeimages.cdn-apple.com/8756/as-images.apple.com/is/iphone-15-pro-max-bluetitanium-select-202309_GEO_US?wid=470&hei=556&fmt=png-alpha&.v=1693009279096");

            migrationBuilder.UpdateData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 13,
                column: "ImageUrl",
                value: "https://images.samsung.com/is/image/samsung/p6pim/vn/2401/gallery/vn-galaxy-s24-s928-sm-s928bztgxxv-thumb-539572408");

            migrationBuilder.UpdateData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 14,
                column: "ImageUrl",
                value: "https://images.samsung.com/is/image/samsung/p6pim/vn/2401/gallery/vn-galaxy-s24-s928-sm-s928bztgxxv-539572407");

            migrationBuilder.UpdateData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 15,
                column: "ImageUrl",
                value: "https://lh3.googleusercontent.com/VkQhN5T5M3j3TZ5h1Q0-wM7K9j7vK2jqj6iqZE5n6Hx1Q2E5Q7R8T9U0V1W2X3Y4Z5");

            migrationBuilder.UpdateData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 16,
                column: "ImageUrl",
                value: "https://lh3.googleusercontent.com/0wCDsVFj6f1ck0jRqjD3Q7C8tB7vE8kI0hH9tG8sF7rE6qD5cC4bB3aA2zZ1yY0xX");

            migrationBuilder.UpdateData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 17,
                column: "ImageUrl",
                value: "https://i02.appmifile.com/mi-com-product/fly-birds/xiaomi-14/pc/2d8b8c0c0a2a93fe0f4f4c8c0c0a2a93.png");

            migrationBuilder.UpdateData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 18,
                column: "ImageUrl",
                value: "https://i02.appmifile.com/mi-com-product/fly-birds/xiaomi-14/pc/9d7f8e9f0e1e92ed1e3e2c9d0d1e92ed.png");

            migrationBuilder.UpdateData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 19,
                column: "ImageUrl",
                value: "https://oasis.opstatics.com/content/dam/oasis/page/2024/global/product/12/image/kv-img.png");

            migrationBuilder.UpdateData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 20,
                column: "ImageUrl",
                value: "https://oasis.opstatics.com/content/dam/oasis/page/2024/global/product/12/image/gallery-img-1.png");

            migrationBuilder.UpdateData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 21,
                column: "ImageUrl",
                value: "https://store.storeimages.cdn-apple.com/8756/as-images.apple.com/is/ipad-pro-11-select-wifi-spacegray-202210?wid=470&hei=556&fmt=png-alpha&.v=1664411207213");

            migrationBuilder.UpdateData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 22,
                column: "ImageUrl",
                value: "https://store.storeimages.cdn-apple.com/8756/as-images.apple.com/is/ipad-pro-11-finish-select-202210-space-gray-wifi?wid=5120&hei=2880&fmt=p-jpg&qlt=80&.v=1664411443597");

            migrationBuilder.UpdateData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 23,
                column: "ImageUrl",
                value: "https://images.samsung.com/is/image/samsung/p6pim/vn/sm-x710nzaaxxv/gallery/vn-galaxy-tab-s9-5g-x710-sm-x710nzaaxxv-thumb-537168467");

            migrationBuilder.UpdateData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 24,
                column: "ImageUrl",
                value: "https://images.samsung.com/is/image/samsung/p6pim/vn/sm-x710nzaaxxv/gallery/vn-galaxy-tab-s9-5g-x710-sm-x710nzaaxxv-537168466");

            migrationBuilder.UpdateData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 25,
                column: "ImageUrl",
                value: "https://i02.appmifile.com/mi-com-product/fly-birds/xiaomi-pad-6/pc/pad6-kv.png");

            migrationBuilder.UpdateData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 26,
                column: "ImageUrl",
                value: "https://i02.appmifile.com/mi-com-product/fly-birds/xiaomi-pad-6/pc/pad6-gallery-1.png");

            migrationBuilder.UpdateData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 27,
                column: "ImageUrl",
                value: "https://i.dell.com/is/image/DellContent/content/dam/ss2/product-images/dell-electronics-and-accessories/dell-monitors/u-series/u2723de/media-gallery/monitor-u2723de-gallery-1.psd?fmt=pjpg&pscan=auto&scl=1&wid=3337&hei=2503&qlt=100,1&resMode=sharp2&size=3337,2503&chrss=full&imwidth=5000");

            migrationBuilder.UpdateData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 28,
                column: "ImageUrl",
                value: "https://www.lg.com/vn/images/man-hinh-may-tinh/md07577031/gallery/D-01.jpg");

            migrationBuilder.UpdateData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 29,
                column: "ImageUrl",
                value: "https://resource.logitech.com/w_800,c_limit,q_auto,f_auto,dpr_1.0/d_transparent.gif/content/dam/logitech/en/products/keyboards/mx-keys/gallery/mx-keys-gallery-graphite-01.png?v=1");

            migrationBuilder.UpdateData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 30,
                column: "ImageUrl",
                value: "https://resource.logitech.com/w_800,c_limit,q_auto,f_auto,dpr_1.0/d_transparent.gif/content/dam/logitech/en/products/keyboards/mx-keys/gallery/mx-keys-gallery-graphite-02.png?v=1");

            migrationBuilder.UpdateData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 31,
                column: "ImageUrl",
                value: "https://assets2.razerzone.com/images/pnx.assets/618d0f3581cf6ac3177c4ae2af69e0fa/razer-blackwidow-v4-pro-gallery-hero-1500x1000.png");

            migrationBuilder.UpdateData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 32,
                column: "ImageUrl",
                value: "https://resource.logitech.com/w_800,c_lpad,ar_1:1,q_auto,f_auto,dpr_1.0/d_transparent.gif/content/dam/logitech/en/products/mice/mx-master-3s/gallery/mx-master-3s-mouse-top-view-graphite.png?v=1");

            migrationBuilder.UpdateData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 34,
                column: "ImageUrl",
                value: "https://assets2.razerzone.com/images/pnx.assets/82f433e3c3e59ce939a70eba8c26a21b/razer-viper-v2-pro-gallery-hero-1500x1000.png");

            migrationBuilder.UpdateData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 36,
                column: "ImageUrl",
                value: "https://www.sony.com.vn/image/c5fb8db9f5be1ab2e6f55bcb59a2c3b7?fmt=pjpeg&wid=330&bgcolor=FFFFFF&bgc=FFFFFF");

            migrationBuilder.UpdateData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 38,
                column: "ImageUrl",
                value: "https://m.media-amazon.com/images/I/61ue5fO88JL._AC_SL1500_.jpg");

            migrationBuilder.UpdateData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 39,
                column: "ImageUrl",
                value: "https://m.media-amazon.com/images/I/61DQwwGO9dL._AC_SL1500_.jpg");

            migrationBuilder.UpdateData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 40,
                column: "ImageUrl",
                value: "https://m.media-amazon.com/images/I/71GxKxT3GCL._AC_SL1500_.jpg");

            migrationBuilder.UpdateData(
                table: "product_image",
                keyColumn: "Id",
                keyValue: 41,
                column: "ImageUrl",
                value: "https://m.media-amazon.com/images/I/71Vy5eF8qpL._AC_SL1500_.jpg");

            migrationBuilder.UpdateData(
                table: "user",
                keyColumn: "Id",
                keyValue: 1,
                column: "PhoneNumber",
                value: null);

            migrationBuilder.UpdateData(
                table: "user",
                keyColumn: "Id",
                keyValue: 2,
                column: "PhoneNumber",
                value: null);
        }
    }
}
