using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tekno.Domain.Promotion;

namespace Tekno.Infrastructure.Persistence.Configurations
{
    public class PromotionConfiguration : IEntityTypeConfiguration<Domain.Promotion.Promotion>
    {
        public void Configure(EntityTypeBuilder<Domain.Promotion.Promotion> builder)
        {
            builder.ToTable("Promotions");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(p => p.Description)
                .HasMaxLength(1000);

            builder.Property(p => p.Type)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(p => p.Value)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(p => p.StartDate)
                .IsRequired();

            builder.Property(p => p.EndDate)
                .IsRequired();

            builder.Property(p => p.Status)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(p => p.Priority)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(p => p.StackableWithCoupons)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(p => p.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("NOW()");

            builder.Property(p => p.UpdatedAt)
                .IsRequired()
                .HasDefaultValueSql("NOW()");

            // Relationships
            builder.HasMany(p => p.ApplicableCategories)
                .WithOne(pc => pc.Promotion)
                .HasForeignKey(pc => pc.PromotionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.ApplicableProducts)
                .WithOne(pp => pp.Promotion)
                .HasForeignKey(pp => pp.PromotionId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(p => p.Status);
            builder.HasIndex(p => new { p.StartDate, p.EndDate });
            builder.HasIndex(p => p.Priority);
        }
    }

    public class PromotionCategoryConfiguration : IEntityTypeConfiguration<PromotionCategory>
    {
        public void Configure(EntityTypeBuilder<PromotionCategory> builder)
        {
            builder.ToTable("PromotionCategories");

            builder.HasKey(pc => new { pc.PromotionId, pc.CategoryId });

            builder.HasOne(pc => pc.Promotion)
                .WithMany(p => p.ApplicableCategories)
                .HasForeignKey(pc => pc.PromotionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class PromotionProductConfiguration : IEntityTypeConfiguration<PromotionProduct>
    {
        public void Configure(EntityTypeBuilder<PromotionProduct> builder)
        {
            builder.ToTable("PromotionProducts");

            builder.HasKey(pp => new { pp.PromotionId, pp.ProductId });

            builder.HasOne(pp => pp.Promotion)
                .WithMany(p => p.ApplicableProducts)
                .HasForeignKey(pp => pp.PromotionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
