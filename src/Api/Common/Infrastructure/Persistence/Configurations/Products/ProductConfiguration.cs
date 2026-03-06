using Api.Common.Domain.Products;
using Api.Common.Infrastructure.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Common.Infrastructure.Persistence.Configurations.Products;

public sealed class ProductConfiguration : AggregateConfiguration<Product, ProductId>
{
    protected override void PostConfigure(EntityTypeBuilder<Product> builder)
    {
        // Required properties
        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(ProductRules.NameMaxLength);

        builder.Property(p => p.Price)
            .IsRequired()
            .HasPrecision(10, 2); // PostgreSQL: numeric(10,2) = 12345678.90

        builder.Property(p => p.Quantity)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(p => p.ImageUrl)
            .IsRequired()
            .HasMaxLength(2048); // URL max length

        // Optional property
        builder.Property(p => p.Description)
            .HasMaxLength(ProductRules.MaxDescriptionMaxLength);

        // Index for frequent access
        builder.HasIndex(p => p.Name);
    }
}