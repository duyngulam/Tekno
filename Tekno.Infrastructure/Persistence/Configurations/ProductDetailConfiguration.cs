using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tekno.Domain.Catalog;

namespace Tekno.Infrastructure.Persistence.Configurations
{
    public class ProductDetailConfiguration : IEntityTypeConfiguration<ProductDetail>
    {
        public void Configure(EntityTypeBuilder<ProductDetail> builder)
        {
            builder.ToTable("product_detail");

            builder.HasKey(pd => pd.ProductId);

            builder.Property(pd => pd.Specs).HasColumnType("jsonb");

            builder.HasOne(pd => pd.Product)
                   .WithOne(p => p.Detail)
                   .HasForeignKey<ProductDetail>(pd => pd.ProductId);
        }
    }
}
