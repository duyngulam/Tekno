using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tekno.Domain.Catalog;

namespace Tekno.Infrastructure.Persistence.Configurations
{
    public class ProductAdvertisementConfiguration : IEntityTypeConfiguration<ProductAdvertisement>
    {
        public void Configure(EntityTypeBuilder<ProductAdvertisement> builder)
        {
            builder.ToTable("product_advertisements");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.ProductId)
                .IsRequired();

            builder.Property(a => a.ImageUrl)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(a => a.Position)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValue("HomeTop");

            builder.Property(a => a.Priority)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(a => a.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(a => a.StartDate)
                .HasColumnType("timestamptz");

            builder.Property(a => a.EndDate)
                .HasColumnType("timestamptz");

            builder.Property(a => a.CreatedAt)
                .HasColumnType("timestamptz")
                .HasDefaultValueSql("NOW()");

            builder.HasOne(a => a.Product)
                .WithMany()
                .HasForeignKey(a => a.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(a => a.Position);
            builder.HasIndex(a => a.IsActive);
            builder.HasIndex(a => new { a.IsActive, a.Position, a.Priority });
        }
    }
}
