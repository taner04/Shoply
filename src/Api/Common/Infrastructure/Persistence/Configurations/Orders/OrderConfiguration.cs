using Api.Common.Domain.Orders;
using Api.Common.Infrastructure.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Common.Infrastructure.Persistence.Configurations.Orders;

public sealed class OrderConfiguration : AggregateConfiguration<Order, OrderId>
{
    protected override void PostConfigure(EntityTypeBuilder<Order> builder)
    {
        // Relationship to User (configured in UserConfiguration)
        builder.HasOne(o => o.User)
            .WithMany()
            .HasForeignKey(o => o.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        // OrderItems - 1:* Relationship
        builder.HasMany<OrderItem>()
            .WithOne()
            .HasForeignKey("OrderId")
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        // Index for user queries
        builder.HasIndex(o => o.UserId);
    }
}