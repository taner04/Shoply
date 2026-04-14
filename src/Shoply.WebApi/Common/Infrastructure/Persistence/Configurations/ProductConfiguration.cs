using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shoply.WebApi.Common.Infrastructure.Persistence.Configurations;

public sealed class ProductConfiguration : EntityConfiguration<Product, ProductId>
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
            .IsRequired()
            .HasMaxLength(ProductRules.MaxDescriptionMaxLength);

        // Unique case-insensitive index for product names
        builder.HasIndex(p => p.Name)
            .IsUnique()
            .UseCollation("POSIX");
    }
}