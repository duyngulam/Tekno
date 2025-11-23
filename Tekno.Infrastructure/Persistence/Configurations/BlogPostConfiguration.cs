using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
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
        }
    }
}
