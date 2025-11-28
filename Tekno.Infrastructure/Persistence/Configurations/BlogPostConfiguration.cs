using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using Tekno.Domain.Blog;

namespace Tekno.Infrastructure.Persistence.Configurations
{
    public class BlogPostConfiguration : IEntityTypeConfiguration<BlogPost>
    {
        private static readonly DateTime SeedTime = new DateTime(2025, 1, 15, 10, 0, 0, DateTimeKind.Utc);
        
        public void Configure(EntityTypeBuilder<BlogPost> builder)
        {
            builder.ToTable("blog_posts");

            builder.HasKey(b => b.Id);

            builder.Property(b => b.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(b => b.Slug)
                .IsRequired()
                .HasMaxLength(200);

            builder.HasIndex(b => b.Slug)
                .IsUnique();

            builder.Property(b => b.Summary)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(b => b.Content)
                .IsRequired()
                .HasColumnType("text");

            builder.Property(b => b.FeaturedImageUrl)
                .HasMaxLength(500);

            builder.Property(b => b.AuthorId)
                .IsRequired();

            builder.Property(b => b.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.Property(b => b.ViewCount)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(b => b.PublishedAt)
                .HasColumnType("timestamptz");

            builder.Property(b => b.CreatedAt)
                .IsRequired()
                .HasColumnType("timestamptz")
                .HasDefaultValueSql("NOW()");

            builder.Property(b => b.UpdatedAt)
                .HasColumnType("timestamptz");

            // Relationships
            builder.HasMany(b => b.Tags)
                .WithOne(t => t.BlogPost)
                .HasForeignKey(t => t.BlogPostId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(b => b.RelatedProducts)
                .WithOne(p => p.BlogPost)
                .HasForeignKey(p => p.BlogPostId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(b => b.Status);
            builder.HasIndex(b => b.PublishedAt);
            builder.HasIndex(b => b.CreatedAt);

            // ========== SEED BLOG POST DATA ==========
            builder.HasData(
                // Blog 1: iPhone 15 Pro Max Review
                new
                {
                    Id = 1,
                    Title = "?ánh giá chi ti?t iPhone 15 Pro Max: ??nh cao công ngh? t? Apple",
                    Slug = "danh-gia-iphone-15-pro-max",
                    Summary = "iPhone 15 Pro Max ?ánh d?u b??c ti?n m?i v?i chip A17 Pro, khung Titanium siêu b?n và camera 48MP v?i zoom quang 5x. Cùng khám phá chi ti?t flagship m?i nh?t t? Apple.",
                    Content = @"<h2>Thi?t k? cao c?p v?i khung Titanium</h2>
<p>iPhone 15 Pro Max là chi?c iPhone ??u tiên s? d?ng khung Titanium thay vì thép không g?, giúp gi?m tr?ng l??ng ?áng k? nh?ng v?n ??m b?o ?? b?n cao. Thi?t k? vi?n m?ng h?n, c?m giác c?m n?m tho?i mái h?n so v?i th? h? tr??c.</p>

<h2>Hi?u n?ng ??nh cao v?i A17 Pro</h2>
<p>Chip A17 Pro ???c s?n xu?t trên ti?n trình 3nm mang l?i hi?u n?ng v??t tr?i, x? lý m?i tác v? m??t mà t? gaming ??n ch?nh s?a video 4K. GPU m?i h? tr? ray tracing, m? ra k? nguyên gaming mobile m?i.</p>

<h2>Camera 48MP v?i zoom 5x</h2>
<p>H? th?ng camera ba ?ng kính v?i camera chính 48MP, telephoto zoom quang 5x mang ??n ch?t l??ng ?nh tuy?t v?i trong m?i ?i?u ki?n ánh sáng. Ch? ?? ch?p ?êm Night Mode ???c c?i thi?n ?áng k?.</p>

<h2>Pin trâu, s?c nhanh USB-C</h2>
<p>Pin 4422mAh s? d?ng c? ngày dài, cu?i cùng Apple c?ng chuy?n sang c?ng USB-C theo chu?n châu Âu. H? tr? s?c nhanh 27W và s?c không dây MagSafe 15W.</p>

<h2>K?t lu?n</h2>
<p>iPhone 15 Pro Max x?ng ?áng là chi?c flagship ?áng mua nh?t n?m 2024 v?i thi?t k? cao c?p, hi?u n?ng m?nh m? và h? th?ng camera xu?t s?c. Giá 33,990,000 VND là h?p lý cho nh?ng gì Apple mang l?i.</p>",
                    FeaturedImageUrl = "https://www.gstatic.com/webp/gallery/1.jpg",
                    AuthorId = 1,
                    Status = BlogPostStatus.Published,
                    ViewCount = 1250,
                    PublishedAt = new DateTime(2025, 1, 10, 9, 0, 0, DateTimeKind.Utc),
                    CreatedAt = new DateTime(2025, 1, 9, 14, 30, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2025, 1, 10, 8, 45, 0, DateTimeKind.Utc)
                },

                // Blog 2: Laptop comparison
                new
                {
                    Id = 2,
                    Title = "So sánh MacBook Air M2 vs Dell XPS 13: Nên ch?n laptop nào?",
                    Slug = "so-sanh-macbook-air-m2-vs-dell-xps-13",
                    Summary = "Hai chi?c laptop cao c?p ???c yêu thích nh?t hi?n nay. MacBook Air M2 v?i hi?u n?ng chip ARM xu?t s?c hay Dell XPS 13 v?i thi?t k? tinh t? và Windows 11? Cùng phân tích chi ti?t.",
                    Content = @"<h2>Thi?t k? và ch?t l??ng build</h2>
<p>MacBook Air M2 v?i thi?t k? vuông v?n hi?n ??i, ?? m?ng ch? 11.3mm và tr?ng l??ng 1.24kg. Dell XPS 13 không kém c?nh v?i thi?t k? siêu m?ng 13.9mm, vi?n màn hình InfinityEdge ?n t??ng.</p>

<h2>Hi?u n?ng và pin</h2>
<p>Chip M2 8-core mang l?i hi?u n?ng v??t tr?i, ??c bi?t v?i các ?ng d?ng macOS native. Pin s? d?ng lên ??n 18 gi?. Dell XPS 13 v?i Intel Core i5/i7 th? h? 13 m?nh m? cho Windows, pin kho?ng 12 gi?.</p>

<h2>Màn hình</h2>
<p>MacBook Air M2: 13.6 inch Liquid Retina (2560x1664), ?? sáng 500 nits. Dell XPS 13: 13.4 inch FHD+ (1920x1200), màn hình InfinityEdge vi?n siêu m?ng.</p>

<h2>Giá c?</h2>
<p>MacBook Air M2 8GB/256GB: 28,990,000 VND. Dell XPS 13 i5/8GB/512GB: 25,990,000 VND. Dell có giá t?t h?n v?i SSD 512GB ngay t? ??u.</p>

<h2>K?t lu?n</h2>
<p>Ch?n MacBook Air M2 n?u b?n ?u tiên pin trâu, h? sinh thái Apple. Ch?n Dell XPS 13 n?u c?n Windows, giá t?t h?n và thi?t k? ??p m?t.</p>",
                    FeaturedImageUrl = "https://www.gstatic.com/webp/gallery/2.jpg",
                    AuthorId = 2,
                    Status = BlogPostStatus.Published,
                    ViewCount = 890,
                    PublishedAt = new DateTime(2025, 1, 12, 10, 30, 0, DateTimeKind.Utc),
                    CreatedAt = new DateTime(2025, 1, 11, 15, 20, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2025, 1, 12, 10, 15, 0, DateTimeKind.Utc)
                },

                // Blog 3: Samsung Galaxy S24 Ultra
                new
                {
                    Id = 3,
                    Title = "Samsung Galaxy S24 Ultra: Android flagship t?t nh?t v?i camera 200MP",
                    Slug = "samsung-galaxy-s24-ultra-review",
                    Summary = "Galaxy S24 Ultra ti?p t?c kh?ng ??nh v? th? d?n ??u phân khúc flagship Android v?i camera 200MP, S Pen tích h?p và màn hình Dynamic AMOLED 2X tuy?t ??p.",
                    Content = @"<h2>Thi?t k? sang tr?ng, S Pen tích h?p</h2>
<p>Thi?t k? vuông v?n m?nh m?, khung nhôm cao c?p. S Pen tích h?p trong thân máy mang l?i tr?i nghi?m ghi chú, v? tuy?t v?i - tính n?ng ??c quy?n c?a dòng Ultra.</p>

<h2>Màn hình ??nh cao</h2>
<p>Màn hình 6.8 inch Dynamic AMOLED 2X v?i ?? phân gi?i QHD+ (3120x1440), t?n s? quét 120Hz, ?? sáng ??nh 2600 nits - rõ nét ngay c? d??i n?ng g?t.</p>

<h2>Camera 200MP chuyên nghi?p</h2>
<p>Camera chính 200MP v?i OIS, camera telephoto kép (3x và 5x), camera ultra wide 12MP. Ch?t l??ng ?nh xu?t s?c, zoom 10x v?n gi? chi ti?t t?t.</p>

<h2>Hi?u n?ng m?nh m? Snapdragon 8 Gen 3</h2>
<p>Chip Snapdragon 8 Gen 3 for Galaxy t?i ?u riêng cho Samsung, hi?u n?ng v??t tr?i, ch?i game m??t mà, pin 5000mAh s? d?ng tho?i mái c? ngày.</p>

<h2>?ánh giá</h2>
<p>Galaxy S24 Ultra là l?a ch?n hàng ??u cho ai mu?n flagship Android hoàn h?o nh?t. Giá 29,990,000 VND x?ng ?áng v?i nh?ng gì Samsung mang l?i.</p>",
                    FeaturedImageUrl = "https://www.gstatic.com/webp/gallery/3.jpg",
                    AuthorId = 1,
                    Status = BlogPostStatus.Published,
                    ViewCount = 1100,
                    PublishedAt = new DateTime(2025, 1, 13, 11, 0, 0, DateTimeKind.Utc),
                    CreatedAt = new DateTime(2025, 1, 12, 16, 45, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2025, 1, 13, 10, 30, 0, DateTimeKind.Utc)
                },

                // Blog 4: Gaming peripherals
                new
                {
                    Id = 4,
                    Title = "Top 5 ph? ki?n gaming t?t nh?t cho setup chuyên nghi?p 2025",
                    Slug = "top-5-phu-kien-gaming-tot-nhat-2025",
                    Summary = "T? bàn phím c? ??n chu?t gaming, tai nghe và màn hình - nh?ng ph? ki?n gaming này s? nâng t?m tr?i nghi?m ch?i game c?a b?n lên m?t level hoàn toàn m?i.",
                    Content = @"<h2>1. Razer BlackWidow V4 Pro - Bàn phím c? gaming cao c?p</h2>
<p>Bàn phím c? v?i switch Razer Green Clicky, ?èn RGB Chroma per-key, Command Dial ti?n l?i. Giá 5,990,000 VND cho tr?i nghi?m gõ tuy?t v?i và ?? b?n cao.</p>

<h2>2. Razer Viper V2 Pro - Chu?t gaming không dây siêu nh?</h2>
<p>Ch? 58g nh?ng ??y ?? tính n?ng: sensor Focus Pro 30K DPI, optical switch Gen 3, pin 80 gi?. Thi?t k? ambidextrous phù h?p m?i ki?u c?m. Giá 3,990,000 VND.</p>

<h2>3. LG UltraGear 27GN800 - Màn hình gaming QHD 144Hz</h2>
<p>27 inch QHD (2560x1440) v?i t?n s? quét 144Hz, t?m n?n IPS Nano Color, h? tr? G-Sync. Giá 8,490,000 VND cho hình ?nh m??t mà, màu s?c chính xác.</p>

<h2>4. Sony WH-1000XM5 - Tai nghe ch?ng ?n t?t nh?t</h2>
<p>Tuy không ph?i tai nghe gaming chuyên d?ng nh?ng ch?t âm Hi-Res, ANC xu?t s?c phù h?p cho game single-player immersive. Giá 8,990,000 VND.</p>

<h2>5. Anker 747 GaNPrime 150W - S?c nhanh ?a n?ng</h2>
<p>S?c laptop gaming, ?i?n tho?i, tai nghe cùng lúc v?i 4 c?ng (3 USB-C + 1 USB-A). Công ngh? GaN nh? g?n, công su?t 150W. Giá 2,490,000 VND.</p>

<h2>T?ng k?t</h2>
<p>??u t? vào ph? ki?n ch?t l??ng s? nâng cao tr?i nghi?m gaming ?áng k?. T?ng chi phí setup hoàn h?o kho?ng 30 tri?u VND.</p>",
                    FeaturedImageUrl = "https://www.gstatic.com/webp/gallery/4.jpg",
                    AuthorId = 2,
                    Status = BlogPostStatus.Published,
                    ViewCount = 750,
                    PublishedAt = new DateTime(2025, 1, 14, 14, 0, 0, DateTimeKind.Utc),
                    CreatedAt = new DateTime(2025, 1, 13, 17, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2025, 1, 14, 13, 45, 0, DateTimeKind.Utc)
                },

                // Blog 5: Budget tablets
                new
                {
                    Id = 5,
                    Title = "Tablet giá r? ?áng mua 2025: Xiaomi Pad 6 vs iPad Air M2",
                    Slug = "tablet-gia-re-dang-mua-2025",
                    Summary = "So sánh hai chi?c tablet t?m trung hot nh?t: Xiaomi Pad 6 giá ch? 8,990,000 VND và iPad Air M2 giá 16,990,000 VND. ?âu là l?a ch?n phù h?p v?i b?n?",
                    Content = @"<h2>Xiaomi Pad 6 - Giá tr? t?t nh?t</h2>
<p>Màn hình 11 inch LCD 2.8K v?i t?n s? quét 144Hz, chip Snapdragon 870 v?n m?nh m? cho ?a nhi?m và ch?i game. Pin 8840mAh s? d?ng c? ngày. Giá ch? 8,990,000 VND.</p>

<h2>iPad Air M2 - Hi?u n?ng cao c?p</h2>
<p>Chip M2 m?nh m? nh? MacBook, màn hình Liquid Retina 11 inch, h? tr? Apple Pencil 2. H? sinh thái iPadOS v?i hàng tri?u app t?i ?u. Giá 16,990,000 VND.</p>

<h2>So sánh chi ti?t</h2>
<ul>
<li><strong>Hi?u n?ng:</strong> M2 m?nh h?n rõ r?t, nh?ng Snapdragon 870 ?? dùng</li>
<li><strong>Màn hình:</strong> Xiaomi có t?n s? quét 144Hz, iPad có màu s?c chính xác h?n</li>
<li><strong>H? ?i?u hành:</strong> iPadOS nhi?u app h?n, MIUI Pad tùy bi?n cao</li>
<li><strong>Giá:</strong> Xiaomi r? h?n g?n m?t n?a</li>
</ul>

<h2>K?t lu?n</h2>
<p>Ch?n Xiaomi Pad 6 n?u ngân sách h?n ch?, dùng ?? xem phim, ??c sách, ch?i game nh?. Ch?n iPad Air M2 n?u c?n hi?u n?ng cao cho công vi?c, v? digital art ho?c mu?n h? sinh thái Apple.</p>",
                    FeaturedImageUrl = "https://www.gstatic.com/webp/gallery/5.jpg",
                    AuthorId = 1,
                    Status = BlogPostStatus.Published,
                    ViewCount = 520,
                    PublishedAt = new DateTime(2025, 1, 15, 9, 0, 0, DateTimeKind.Utc),
                    CreatedAt = new DateTime(2025, 1, 14, 18, 30, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2025, 1, 15, 8, 45, 0, DateTimeKind.Utc)
                },

                // Blog 6: Draft article
                new
                {
                    Id = 6,
                    Title = "Bí quy?t ch?n laptop cho sinh viên: C?u hình nào là ???",
                    Slug = "bi-quyet-chon-laptop-cho-sinh-vien",
                    Summary = "H??ng d?n chi ti?t giúp sinh viên ch?n laptop phù h?p v?i ngành h?c và ngân sách. T? sinh viên v?n phòng ??n k? thu?t, thi?t k? - ??u có g?i ý c? th?.",
                    Content = @"<h2>Nhu c?u theo ngành h?c</h2>
<p>N?i dung ?ang ???c c?p nh?t...</p>",
                    FeaturedImageUrl = "https://www.gstatic.com/webp/gallery/1.jpg",
                    AuthorId = 2,
                    Status = BlogPostStatus.Draft,
                    ViewCount = 0,
                    PublishedAt = new DateTime(2025, 1, 15, 10, 0, 0, DateTimeKind.Utc),
                    CreatedAt = new DateTime(2025, 1, 15, 9, 15, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2025, 1, 15, 9, 45, 0, DateTimeKind.Utc)
                }
            );
        }
    }

    public class BlogPostTagConfiguration : IEntityTypeConfiguration<BlogPostTag>
    {
        public void Configure(EntityTypeBuilder<BlogPostTag> builder)
        {
            builder.ToTable("blog_post_tags");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.BlogPostId)
                .IsRequired();

            builder.Property(t => t.Tag)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(t => t.Tag);
            builder.HasIndex(t => new { t.BlogPostId, t.Tag });

            // ========== SEED TAG DATA ==========
            builder.HasData(
                // Tags for Blog 1 (iPhone 15 Pro Max)
                new { Id = 1, BlogPostId = 1, Tag = "review" },
                new { Id = 2, BlogPostId = 1, Tag = "iphone" },
                new { Id = 3, BlogPostId = 1, Tag = "apple" },
                new { Id = 4, BlogPostId = 1, Tag = "flagship" },
                new { Id = 5, BlogPostId = 1, Tag = "smartphone" },

                // Tags for Blog 2 (MacBook vs Dell)
                new { Id = 6, BlogPostId = 2, Tag = "comparison" },
                new { Id = 7, BlogPostId = 2, Tag = "laptop" },
                new { Id = 8, BlogPostId = 2, Tag = "macbook" },
                new { Id = 9, BlogPostId = 2, Tag = "dell" },

                // Tags for Blog 3 (Samsung S24 Ultra)
                new { Id = 10, BlogPostId = 3, Tag = "review" },
                new { Id = 11, BlogPostId = 3, Tag = "samsung" },
                new { Id = 12, BlogPostId = 3, Tag = "android" },
                new { Id = 13, BlogPostId = 3, Tag = "flagship" },
                new { Id = 14, BlogPostId = 3, Tag = "camera" },

                // Tags for Blog 4 (Gaming peripherals)
                new { Id = 15, BlogPostId = 4, Tag = "gaming" },
                new { Id = 16, BlogPostId = 4, Tag = "peripherals" },
                new { Id = 17, BlogPostId = 4, Tag = "accessories" },
                new { Id = 18, BlogPostId = 4, Tag = "top-list" },

                // Tags for Blog 5 (Tablets)
                new { Id = 19, BlogPostId = 5, Tag = "comparison" },
                new { Id = 20, BlogPostId = 5, Tag = "tablet" },
                new { Id = 21, BlogPostId = 5, Tag = "budget" },
                new { Id = 22, BlogPostId = 5, Tag = "xiaomi" },
                new { Id = 23, BlogPostId = 5, Tag = "ipad" },

                // Tags for Blog 6 (Draft)
                new { Id = 24, BlogPostId = 6, Tag = "guide" },
                new { Id = 25, BlogPostId = 6, Tag = "student" },
                new { Id = 26, BlogPostId = 6, Tag = "laptop" }
            );
        }
    }

    public class BlogPostProductConfiguration : IEntityTypeConfiguration<BlogPostProduct>
    {
        public void Configure(EntityTypeBuilder<BlogPostProduct> builder)
        {
            builder.ToTable("blog_post_products");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.BlogPostId)
                .IsRequired();

            builder.Property(p => p.ProductId)
                .IsRequired();

            builder.HasIndex(p => p.BlogPostId);
            builder.HasIndex(p => p.ProductId);
            builder.HasIndex(p => new { p.BlogPostId, p.ProductId })
                .IsUnique();

            // ========== SEED RELATED PRODUCTS DATA ==========
            builder.HasData(
                // Blog 1 related products (iPhone review)
                new { Id = 1, BlogPostId = 1, ProductId = 10 }, // iPhone 15 Pro Max
                new { Id = 2, BlogPostId = 1, ProductId = 61 }, // AirPods Pro 2
                new { Id = 3, BlogPostId = 1, ProductId = 80 }, // Spigen Case

                // Blog 2 related products (MacBook vs Dell)
                new { Id = 4, BlogPostId = 2, ProductId = 2 },  // MacBook Air M2
                new { Id = 5, BlogPostId = 2, ProductId = 1 },  // Dell XPS 13
                new { Id = 6, BlogPostId = 2, ProductId = 81 }, // Laptop Sleeve

                // Blog 3 related products (Samsung S24)
                new { Id = 7, BlogPostId = 3, ProductId = 11 }, // Samsung S24 Ultra
                new { Id = 8, BlogPostId = 3, ProductId = 70 }, // Anker Charger
                new { Id = 9, BlogPostId = 3, ProductId = 80 }, // Phone Case

                // Blog 4 related products (Gaming gear)
                new { Id = 10, BlogPostId = 4, ProductId = 41 }, // Razer BlackWidow
                new { Id = 11, BlogPostId = 4, ProductId = 51 }, // Razer Viper
                new { Id = 12, BlogPostId = 4, ProductId = 31 }, // LG Monitor
                new { Id = 13, BlogPostId = 4, ProductId = 60 }, // Sony Headphones
                new { Id = 14, BlogPostId = 4, ProductId = 70 }, // Anker Charger

                // Blog 5 related products (Tablets)
                new { Id = 15, BlogPostId = 5, ProductId = 22 }, // Xiaomi Pad 6
                new { Id = 16, BlogPostId = 5, ProductId = 20 }, // iPad Pro
                new { Id = 17, BlogPostId = 5, ProductId = 21 }  // Galaxy Tab S9
            );
        }
    }
}
