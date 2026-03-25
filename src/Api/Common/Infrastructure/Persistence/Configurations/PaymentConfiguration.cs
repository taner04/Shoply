using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Api.Features.Orders.Models;
using PaymentId = Api.Features.Orders.Models.PaymentId;

namespace Api.Common.Infrastructure.Persistence.Configurations;

public sealed class PaymentConfiguration : EntityConfiguration<Payment, PaymentId>
{
    protected override void PostConfigure(EntityTypeBuilder<Payment> builder)
    {
        builder.Property(p => p.Amount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(p => p.StripePaymentIntentId)
            .IsRequired(false)
            .HasMaxLength(255);

        builder.Property(p => p.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(p => p.FailureReason)
            .HasMaxLength(500);

        builder.Property(p => p.RefundedAmount)
            .HasPrecision(18, 2)
            .HasDefaultValue(0)
            .IsRequired();

        // Index for Stripe PaymentIntentId lookups (only on non-null values)
        builder.HasIndex(p => p.StripePaymentIntentId)
            .IsUnique()
            .HasFilter("\"StripePaymentIntentId\" IS NOT NULL");

        // Index for OrderId lookups
        builder.HasIndex(p => p.OrderId)
            .IsUnique();
    }
}
