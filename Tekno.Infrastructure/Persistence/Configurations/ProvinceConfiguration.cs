using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tekno.Domain.Location;

namespace Tekno.Infrastructure.Persistence.Configurations
{
    public class ProvinceConfiguration : IEntityTypeConfiguration<Province>
    {
        public void Configure(EntityTypeBuilder<Province> builder)
        {
            builder.ToTable("provinces");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Code).IsRequired();
            builder.HasIndex(p => p.Code).IsUnique();
            builder.Property(p => p.Name).IsRequired().HasMaxLength(200);
            builder.Property(p => p.Codename).HasMaxLength(200);
            builder.Property(p => p.DivisionType).HasMaxLength(100);
            builder.Property(p => p.PhoneCode);
            builder.Property(p => p.CreatedAt).HasColumnType("timestamptz").HasDefaultValueSql("NOW()");
            builder.Property(p => p.UpdatedAt).HasColumnType("timestamptz");
        }
    }
}
