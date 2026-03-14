using Api.Features.Orders.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderId = Api.Features.Orders.Models.OrderId;

namespace Api.Common.Infrastructure.Persistence.Configurations;

public sealed class OrderConfiguration : EntityConfiguration<Order, OrderId>
{
    protected override void PostConfigure(EntityTypeBuilder<Order> builder)
    {
        // Relationship to User
        builder.HasOne(o => o.User)
            .WithMany()
            .HasForeignKey(o => o.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(o => o.IdempotencyKey)
            .IsRequired();

        builder.Property(o => o.PaymentStatus)
            .IsRequired()
            .HasConversion<string>();

        // OrderItems as owned collection (value objects)
        builder.OwnsMany(o => o.OrderItems, nav =>
        {
            nav.ToTable("OrderItems");
            nav.WithOwner().HasForeignKey("OrderId");
            nav.HasKey("OrderId", "ProductId");
        });

        // Index for user queries
        builder.HasIndex(o => o.UserId);
    }
}