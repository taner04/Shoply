using Api.Common.Domain.Baskets;
using Api.Common.Infrastructure.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Common.Infrastructure.Persistence.Configurations.Baskets;

public sealed class BasketConfiguration : AggregateConfiguration<Basket, BasketId>
{
    protected override void PostConfigure(EntityTypeBuilder<Basket> builder)
    {
        // Relationship to User
        builder.HasOne(b => b.User)
            .WithOne(u => u.Basket)
            .HasForeignKey<Basket>(b => b.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        // BasketItems - 1:* Relationship
        builder.HasMany<BasketItem>()
            .WithOne()
            .HasForeignKey("BasketId")
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        // Index for user queries
        builder.HasIndex(b => b.UserId)
            .IsUnique(); // One basket per user
    }
}