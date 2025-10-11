using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tekno.Domain.Catalog;

namespace Tekno.Infrastructure.Persistence.Configurations
{
    public class ProductImageConfiguration : IEntityTypeConfiguration<ProductImage>
    {
        public void Configure(EntityTypeBuilder<ProductImage> builder)
        {
            builder.ToTable("product_image");
            builder.HasKey(pi => pi.Id);

            builder.Property(pi => pi.ImageUrl).IsRequired();
            builder.Property(pi => pi.IsPrimary).HasDefaultValue(false);
            builder.Property(pi => pi.SortOrder).HasDefaultValue(0);

            builder.HasOne(pi => pi.Product)
                   .WithMany(p => p.Images)
                   .HasForeignKey(pi => pi.ProductId);
        }
    }
}
