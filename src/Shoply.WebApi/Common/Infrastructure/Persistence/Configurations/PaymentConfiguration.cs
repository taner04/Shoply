using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shoply.WebApi.Common.Infrastructure.Persistence.Configurations;

public sealed class PaymentConfiguration : EntityConfiguration<Payment, PaymentId>
{
    protected override void PostConfigure(EntityTypeBuilder<Payment> builder)
    {
        builder.Property(p => p.Amount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(p => p.PaymentIntentId)
            .IsRequired(false)
            .HasMaxLength(255);

        builder.Property(p => p.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(p => p.RefundedAmount)
            .HasPrecision(18, 2)
            .HasDefaultValue(0)
            .IsRequired();

        // Index for Stripe PaymentIntentId lookups (only on non-null values)
        builder.HasIndex(p => p.PaymentIntentId)
            .IsUnique()
            .HasFilter("\"PaymentIntentId\" IS NOT NULL");

        // Index for OrderId lookups
        builder.HasIndex(p => p.OrderId)
            .IsUnique();
    }
}