using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserId = Api.Features.Users.Models.UserId;

namespace Api.Common.Infrastructure.Persistence.Configurations;

public sealed class UserConfiguration : EntityConfiguration<User, UserId>
{
    protected override void PostConfigure(EntityTypeBuilder<User> builder)
    {
        // Required properties
        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(u => u.Auth0Id)
            .IsRequired()
            .HasMaxLength(256);

        // Index for email queries
        builder.HasIndex(u => u.Email)
            .IsUnique();

        builder.HasIndex(u => u.Auth0Id)
            .IsUnique();

        // Basket is owned by User
        builder.OwnsOne(u => u.Basket, nav =>
        {
            nav.ToTable("Baskets");
            nav.OwnsMany(b => b.BasketItems, items =>
            {
                items.ToTable("BasketItems");
                items.WithOwner().HasForeignKey("BasketId");
            });
        });
    }
}