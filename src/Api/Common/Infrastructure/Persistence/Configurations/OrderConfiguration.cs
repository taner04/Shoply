using Api.Common.Domain.Orders;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Common.Infrastructure.Persistence.Configurations;

public sealed class OrderConfiguration : AggregateConfiguration<Order, OrderId>
{
    protected override void PostConfigure(EntityTypeBuilder<Order> builder)
    {
        // Relationship to User
        builder.HasOne(o => o.User)
            .WithMany()
            .HasForeignKey(o => o.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

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