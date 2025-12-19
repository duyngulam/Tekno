using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using Tekno.Domain.Blog;

namespace Tekno.Infrastructure.Persistence.Configurations
{
    public class BlogPostConfiguration : IEntityTypeConfiguration<BlogPost>
    {
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

            // ProductIds as JSON column
            builder.Property(b => b.ProductIds)
                .IsRequired()
                .HasColumnType("text")
                .HasDefaultValue("[]");

            // Relationships
            builder.HasMany(b => b.Tags)
                .WithOne(t => t.BlogPost)
                .HasForeignKey(t => t.BlogPostId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(b => b.Status);
            builder.HasIndex(b => b.PublishedAt);
            builder.HasIndex(b => b.CreatedAt);

            // ========== SEED BLOG POST DATA ==========
            builder.HasData(
                new
                {
                    Id = 1,
                    Title = "Danh gia chi tiet iPhone 15 Pro Max: Dinh cao cong nghe tu Apple",
                    Slug = "danh-gia-iphone-15-pro-max",
                    Summary = "iPhone 15 Pro Max danh dau buoc tien moi voi chip A17 Pro, khung Titanium sieu ben va camera 48MP voi zoom quang 5x.",
                    Content = "<h2>Thiet ke cao cap voi khung Titanium</h2><p>iPhone 15 Pro Max la chiec iPhone dau tien su dung khung Titanium thay vi thep khong gi, giup giam trong luong dang ke nhung van dam bao do ben cao.</p><h2>Hieu nang dinh cao voi A17 Pro</h2><p>Chip A17 Pro duoc san xuat tren tien trinh 3nm mang lai hieu nang vuot troi.</p><h2>Camera 48MP voi zoom 5x</h2><p>He thong camera ba ong kinh voi camera chinh 48MP, telephoto zoom quang 5x.</p>",
                    FeaturedImageUrl = "https://www.gstatic.com/webp/gallery/1.jpg",
                    AuthorId = 1,
                    Status = BlogPostStatus.Published,
                    ViewCount = 1250,
                    ProductIds = "[10,61,80]",
                    PublishedAt = new DateTime(2025, 1, 10, 9, 0, 0, DateTimeKind.Utc),
                    CreatedAt = new DateTime(2025, 1, 9, 14, 30, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2025, 1, 10, 8, 45, 0, DateTimeKind.Utc)
                },
                new
                {
                    Id = 2,
                    Title = "So sanh MacBook Air M2 vs Dell XPS 13: Nen chon laptop nao?",
                    Slug = "so-sanh-macbook-air-m2-vs-dell-xps-13",
                    Summary = "Hai chiec laptop cao cap duoc yeu thich nhat hien nay. Cung phan tich chi tiet.",
                    Content = "<h2>Thiet ke va chat luong build</h2><p>MacBook Air M2 voi thiet ke vuong vuc hien dai, do mong chi 11.3mm va trong luong 1.24kg.</p>",
                    FeaturedImageUrl = "https://www.gstatic.com/webp/gallery/2.jpg",
                    AuthorId = 2,
                    Status = BlogPostStatus.Published,
                    ViewCount = 890,
                    ProductIds = "[1,2,81]",
                    PublishedAt = new DateTime(2025, 1, 12, 10, 30, 0, DateTimeKind.Utc),
                    CreatedAt = new DateTime(2025, 1, 11, 15, 20, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2025, 1, 12, 10, 15, 0, DateTimeKind.Utc)
                },
                new
                {
                    Id = 3,
                    Title = "Samsung Galaxy S24 Ultra: Android flagship tot nhat voi camera 200MP",
                    Slug = "samsung-galaxy-s24-ultra-review",
                    Summary = "Galaxy S24 Ultra tiep tuc khang dinh vi the dan dau phan khuc flagship Android.",
                    Content = "<h2>Thiet ke sang trong, S Pen tich hop</h2><p>Thiet ke vuong vuc manh me, khung nhom cao cap.</p>",
                    FeaturedImageUrl = "https://www.gstatic.com/webp/gallery/3.jpg",
                    AuthorId = 1,
                    Status = BlogPostStatus.Published,
                    ViewCount = 1100,
                    ProductIds = "[11,70,80]",
                    PublishedAt = new DateTime(2025, 1, 13, 11, 0, 0, DateTimeKind.Utc),
                    CreatedAt = new DateTime(2025, 1, 12, 16, 45, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2025, 1, 13, 10, 30, 0, DateTimeKind.Utc)
                },
                new
                {
                    Id = 4,
                    Title = "Top 5 phu kien gaming tot nhat cho setup chuyen nghiep 2025",
                    Slug = "top-5-phu-kien-gaming-tot-nhat-2025",
                    Summary = "Tu ban phim co den chuot gaming, tai nghe va man hinh.",
                    Content = "<h2>1. Razer BlackWidow V4 Pro</h2><p>Ban phim co voi switch Razer Green Clicky.</p>",
                    FeaturedImageUrl = "https://www.gstatic.com/webp/gallery/4.jpg",
                    AuthorId = 2,
                    Status = BlogPostStatus.Published,
                    ViewCount = 750,
                    ProductIds = "[31,41,51,60,70]",
                    PublishedAt = new DateTime(2025, 1, 14, 14, 0, 0, DateTimeKind.Utc),
                    CreatedAt = new DateTime(2025, 1, 13, 17, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2025, 1, 14, 13, 45, 0, DateTimeKind.Utc)
                },
                new
                {
                    Id = 5,
                    Title = "Tablet gia re dang mua 2025: Xiaomi Pad 6 vs iPad Air M2",
                    Slug = "tablet-gia-re-dang-mua-2025",
                    Summary = "So sanh hai chiec tablet tam trung hot nhat.",
                    Content = "<h2>Xiaomi Pad 6 - Gia tri tot nhat</h2><p>Man hinh 11 inch LCD 2.8K voi tan so quet 144Hz.</p>",
                    FeaturedImageUrl = "https://www.gstatic.com/webp/gallery/5.jpg",
                    AuthorId = 1,
                    Status = BlogPostStatus.Published,
                    ViewCount = 680,
                    ProductIds = "[20,21,22]",
                    PublishedAt = new DateTime(2025, 1, 15, 9, 30, 0, DateTimeKind.Utc),
                    CreatedAt = new DateTime(2025, 1, 14, 18, 15, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2025, 1, 15, 9, 0, 0, DateTimeKind.Utc)
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

            // ========== SEED TAG DATA ========= =
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
                new { Id = 23, BlogPostId = 5, Tag = "ipad" }
            );
        }
    }
}