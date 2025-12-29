using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaymentEntity = Tekno.Domain.Payment.Payment;

namespace Tekno.Infrastructure.Persistence.Configurations
{
    public class PaymentConfiguration : IEntityTypeConfiguration<PaymentEntity>
    {
        public void Configure(EntityTypeBuilder<PaymentEntity> builder)
        {
            builder.ToTable("payment");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.TransactionId)
                .IsRequired()
                .HasMaxLength(255);

            builder.HasIndex(p => p.TransactionId)
                .IsUnique();

            builder.Property(p => p.Gateway)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(p => p.Method)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(p => p.Status)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(p => p.Amount)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(p => p.Currency)
                .IsRequired()
                .HasMaxLength(3)
                .HasDefaultValue("VND");

            builder.Property(p => p.GatewayResponse)
                .HasColumnType("text");

            builder.Property(p => p.ErrorMessage)
                .HasMaxLength(1000);

            builder.Property(p => p.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("NOW()");

            // Relationship with Order (Many payments per Order)
            builder.HasOne(p => p.Order)
                .WithMany(o => o.Payments)
                .HasForeignKey(p => p.OrderId)
                // allow cascade delete so dependent payments are removed when order is removed
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes for common queries
            builder.HasIndex(p => p.OrderId);
            builder.HasIndex(p => p.UserId);
            builder.HasIndex(p => p.Status);
            builder.HasIndex(p => p.CreatedAt);
        }
    }
}
