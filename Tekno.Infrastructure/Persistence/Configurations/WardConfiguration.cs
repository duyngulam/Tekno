using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tekno.Domain.Location;

namespace Tekno.Infrastructure.Persistence.Configurations
{
    public class WardConfiguration : IEntityTypeConfiguration<Ward>
    {
        public void Configure(EntityTypeBuilder<Ward> builder)
        {
            builder.ToTable("wards");
            builder.HasKey(w => w.Id);
            builder.Property(w => w.Code).IsRequired();
            builder.HasIndex(w => w.Code).IsUnique();
            builder.Property(w => w.Name).IsRequired().HasMaxLength(200);
            builder.Property(w => w.Codename).HasMaxLength(200);
            builder.Property(w => w.DivisionType).HasMaxLength(100);
            builder.Property(w => w.DistrictCode).IsRequired();
            builder.Property(w => w.CreatedAt).HasColumnType("timestamptz").HasDefaultValueSql("NOW()");
            builder.Property(w => w.UpdatedAt).HasColumnType("timestamptz");

            // Relationship: Ward -> District
            builder.HasOne(w => w.District)
                .WithMany(d => d.Wards)
                .HasForeignKey(w => w.DistrictCode)
                .HasPrincipalKey(d => d.Code)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(w => w.DistrictCode);
        }
    }
}
