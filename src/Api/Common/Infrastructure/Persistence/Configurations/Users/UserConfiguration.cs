using Api.Common.Domain.Baskets;
using Api.Common.Domain.Orders;
using Api.Common.Domain.Users;
using Api.Common.Infrastructure.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Common.Infrastructure.Persistence.Configurations.Users;

public sealed class UserConfiguration : AggregateConfiguration<User, UserId>
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
    }
}