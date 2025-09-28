using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tekno.Domain.Auth;
using Tekno.Infrastructure.Auth;

namespace Tekno.Infrastructure.Persistence.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(u => u.Id);

            builder.Property(u => u.Username)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(u => u.PasswordHash)
                .IsRequired();

            builder.HasOne(u => u.Role)
                   .WithMany()
                   .HasForeignKey(u => u.RoleId);
            builder.HasData(
                 new
                 {
                     Id = 1,
                     Username = "admin",
                     Email = "admin@tekno.com",
                     PasswordHash = "$2a$11$W/ZYaZwxFhbSWpJNtPMAfetjQIsqJ1rYdiP2GQoF1.Hr7aqFmtaya",
                     RoleId = 1
                 },
                 new
                 {
                     Id = 2,
                     Username = "customer",
                     Email = "customer@tekno.com",
                     PasswordHash = "$2a$11$ZKxnFd0g1qcrtOgFJrbYiOOnKrtsA6flk4msMC0Uf/qcmqYzoUlSq",
                     RoleId = 2
                 }
                 );
        }
    }
}
