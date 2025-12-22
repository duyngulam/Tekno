using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using Tekno.Domain.Auth;

namespace Tekno.Infrastructure.Persistence.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        private static readonly DateTime SeedTime = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        
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

            // ========== SEED USER DATA ==========
            builder.HasData(
                // Admin user
                new
                {
                    Id = 1,
                    Fullname = "Admin User",
                    Email = "admin@tekno.com",
                    PhoneNumber = "0901234567",
                    PasswordHash = "$2a$11$W/ZYaZwxFhbSWpJNtPMAfetjQIsqJ1rYdiP2GQoF1.Hr7aqFmtaya", // password: "password"
                    RoleId = 1,
                    CreatedAt = SeedTime
                },
                
                // Customer user
                new
                {
                    Id = 2,
                    Fullname = "Customer User",
                    Email = "customer@tekno.com",
                    PhoneNumber = "0912345678",
                    PasswordHash = "$2a$11$ZKxnFd0g1qcrtOgFJrbYiOOnKrtsA6flk4msMC0Uf/qcmqYzoUlSq", // password: "password"
                    RoleId = 2,
                    CreatedAt = SeedTime
                }
            );
        }
    }

    public class UserAddressConfiguration : IEntityTypeConfiguration<UserAddress>
    {
        private static readonly DateTime SeedTime = new DateTime(2025, 1, 10, 10, 0, 0, DateTimeKind.Utc);
        
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

            builder.Property(a => a.AddressLine)
                .IsRequired()
                .HasMaxLength(200)
                .HasColumnName("address_line");

            builder.Property(a => a.ProvinceCode)
                .IsRequired()
                .HasColumnName("province_code");

            builder.Property(a => a.ProvinceName)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("province_name");

            builder.Property(a => a.DistrictCode)
                .IsRequired()
                .HasColumnName("district_code");

            builder.Property(a => a.DistrictName)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("district_name");

            builder.Property(a => a.WardCode)
                .IsRequired()
                .HasColumnName("ward_code");

            builder.Property(a => a.WardName)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("ward_name");

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

            // ========== SEED ADDRESS DATA (Customer User Only) ==========
            builder.HasData(
                // Primary address for customer user (default)
                new
                {
                    Id = 1,
                    UserId = 2, // Customer user
                    RecipientName = "Customer User",
                    PhoneNumber = "0912345678",
                    AddressLine = "123 Nguyễn Huệ",
                    ProvinceCode = 79,
                    ProvinceName = "Thành phố Hồ Chí Minh",
                    DistrictCode = 760,
                    DistrictName = "Quận 1",
                    WardCode = 26734,
                    WardName = "Phường Bến Nghé",
                    IsDefault = true,
                    CreatedAt = SeedTime
                },
                
                // Secondary address for customer user
                new
                {
                    Id = 2,
                    UserId = 2, // Customer user
                    RecipientName = "Customer User",
                    PhoneNumber = "0912345678",
                    AddressLine = "456 Võ Văn Tần",
                    ProvinceCode = 79,
                    ProvinceName = "Thành phố Hồ Chí Minh",
                    DistrictCode = 769,
                    DistrictName = "Quận 3",
                    WardCode = 27031,
                    WardName = "Phường 6",
                    IsDefault = false,
                    CreatedAt = SeedTime
                }
            );
        }
    }
}
