using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tekno.Domain.Location;

namespace Tekno.Infrastructure.Persistence.Configurations
{
    public class DistrictConfiguration : IEntityTypeConfiguration<District>
    {
        public void Configure(EntityTypeBuilder<District> builder)
        {
            builder.ToTable("districts");
            builder.HasKey(d => d.Id);
            builder.Property(d => d.Code).IsRequired();
            builder.HasIndex(d => d.Code).IsUnique();
            builder.Property(d => d.Name).IsRequired().HasMaxLength(200);
            builder.Property(d => d.Codename).HasMaxLength(200);
            builder.Property(d => d.DivisionType).HasMaxLength(100);
            builder.Property(d => d.ProvinceCode).IsRequired();
            builder.Property(d => d.CreatedAt).HasColumnType("timestamptz").HasDefaultValueSql("NOW()");
            builder.Property(d => d.UpdatedAt).HasColumnType("timestamptz");

            // Relationship: District -> Province
            builder.HasOne(d => d.Province)
                .WithMany(p => p.Districts)
                .HasForeignKey(d => d.ProvinceCode)
                .HasPrincipalKey(p => p.Code)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(d => d.ProvinceCode);
        }
    }
}
