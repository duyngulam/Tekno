using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tekno.Domain.Review;

namespace Tekno.Infrastructure.Persistence.Configurations
{
    public class ProductReviewConfiguration : IEntityTypeConfiguration<ProductReview>
    {
        public void Configure(EntityTypeBuilder<ProductReview> builder)
        {
            builder.ToTable("product_reviews");
            builder.HasKey(r => r.Id);

            builder.Property(r => r.ProductId)
                .IsRequired();

            builder.Property(r => r.UserId)
                .IsRequired();

            builder.Property(r => r.OrderId)
                .IsRequired(false);

            builder.Property(r => r.VariantId)
                .IsRequired(false);

            builder.Property(r => r.Rating)
                .IsRequired();

            builder.Property(r => r.Comment)
                .HasMaxLength(2000)
                .IsRequired();

            builder.Property(r => r.Status)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired()
                .HasDefaultValue(ReviewStatus.Pending);

            builder.Property(r => r.IsVerifiedPurchase)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(r => r.HelpfulCount)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(r => r.NotHelpfulCount)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(r => r.CreatedAt)
                .HasColumnType("timestamptz")
                .HasDefaultValueSql("NOW()");

            builder.Property(r => r.UpdatedAt)
                .HasColumnType("timestamptz")
                .IsRequired(false);

            builder.Property(r => r.ApprovedAt)
                .HasColumnType("timestamptz")
                .IsRequired(false);

            builder.Property(r => r.ApprovedByUserId)
                .IsRequired(false);

            // Indexes
            builder.HasIndex(r => r.ProductId);
            builder.HasIndex(r => r.UserId);
            builder.HasIndex(r => new { r.UserId, r.ProductId }).IsUnique(); // One review per user per product
            builder.HasIndex(r => r.Status);
        }
    }

    public class ReviewHelpfulnessConfiguration : IEntityTypeConfiguration<ReviewHelpfulness>
    {
        public void Configure(EntityTypeBuilder<ReviewHelpfulness> builder)
        {
            builder.ToTable("review_helpfulness");
            builder.HasKey(v => v.Id);

            builder.Property(v => v.ReviewId)
                .IsRequired();

            builder.Property(v => v.UserId)
                .IsRequired();

            builder.Property(v => v.IsHelpful)
                .IsRequired();

            builder.Property(v => v.VotedAt)
                .HasColumnType("timestamptz")
                .HasDefaultValueSql("NOW()");

            builder.HasOne(v => v.Review)
                .WithMany()
                .HasForeignKey(v => v.ReviewId)
                .OnDelete(DeleteBehavior.Cascade);

            // One vote per user per review
            builder.HasIndex(v => new { v.ReviewId, v.UserId }).IsUnique();
        }
    }
}
