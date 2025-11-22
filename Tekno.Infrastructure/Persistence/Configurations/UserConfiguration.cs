using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tekno.Domain.Auth;

namespace Tekno.Infrastructure.Persistence.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("user");

            builder.HasKey(u => u.Id);
            
            builder.Property(u => u.Fullname)
                .HasMaxLength(100);

            builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(255);

            builder.HasIndex(u => u.Email).IsUnique();

            builder.Property(u => u.PhoneNumber)
                .HasMaxLength(20);

            builder.Property(u => u.PasswordHash)
                .IsRequired();

            builder.Property(u => u.CreatedAt)
                .HasColumnType("timestamptz")
                .HasDefaultValueSql("NOW()");

            builder.Property(u => u.UpdatedAt)
                .HasColumnType("timestamptz");

            builder.HasOne(u => u.Role)
                .WithMany()
                .HasForeignKey(u => u.RoleId);

            builder.HasMany(u => u.Addresses)
                .WithOne(a => a.User)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Seed data
            builder.HasData(
                new
                {
                    Id = 1,
                    Fullname = "Admin User",
                    Email = "admin@tekno.com",
                    PasswordHash = "$2a$11$W/ZYaZwxFhbSWpJNtPMAfetjQIsqJ1rYdiP2GQoF1.Hr7aqFmtaya",
                    RoleId = 1,
                    CreatedAt = new System.DateTime(2025, 1, 1, 0, 0, 0, System.DateTimeKind.Utc)
                },
                new
                {
                    Id = 2,
                    Fullname = "Customer User",
                    Email = "customer@tekno.com",
                    PasswordHash = "$2a$11$ZKxnFd0g1qcrtOgFJrbYiOOnKrtsA6flk4msMC0Uf/qcmqYzoUlSq",
                    RoleId = 2,
                    CreatedAt = new System.DateTime(2025, 1, 1, 0, 0, 0, System.DateTimeKind.Utc)
                }
            );
        }
    }

    public class UserAddressConfiguration : IEntityTypeConfiguration<UserAddress>
    {
        public void Configure(EntityTypeBuilder<UserAddress> builder)
        {
            builder.ToTable("user_addresses");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.UserId)
                .IsRequired();

            builder.Property(a => a.RecipientName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(a => a.PhoneNumber)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(a => a.AddressLine1)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(a => a.AddressLine2)
                .HasMaxLength(200);

            builder.Property(a => a.City)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(a => a.State)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(a => a.PostalCode)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(a => a.Country)
                .IsRequired()
                .HasMaxLength(100)
                .HasDefaultValue("Vietnam");

            builder.Property(a => a.IsDefault)
                .HasDefaultValue(false);

            builder.Property(a => a.CreatedAt)
                .HasColumnType("timestamptz")
                .HasDefaultValueSql("NOW()");

            builder.Property(a => a.UpdatedAt)
                .HasColumnType("timestamptz");

            builder.HasOne(a => a.User)
                .WithMany(u => u.Addresses)
                .HasForeignKey(a => a.UserId);

            builder.HasIndex(a => a.UserId);
        }
    }
}
