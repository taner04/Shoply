using Api.Common.Domain.Orders;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Common.Infrastructure.Persistence.Configurations.Orders;

public sealed class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("OrderItems");

        builder.HasKey(oi => oi.Id);

        // Foreign Key to Product (for reference only, not as navigation)

        // Product snapshot properties
        builder.Property(oi => oi.ProductName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(oi => oi.UnitPrice)
            .IsRequired()
            .HasPrecision(10, 2); // PostgreSQL: numeric(10,2)

        builder.Property(oi => oi.Quantity)
            .IsRequired()
            .HasDefaultValue(1);

        // Computed property for TotalPrice (read-only, not persisted)
        // builder.Ignore(oi => oi.TotalPrice); // Not needed, it's a computed property

        // Index for product queries
        builder.HasIndex(oi => oi.ProductId);

        // Audit properties - inherited from Entity -> Auditable
        builder.Property(oi => oi.CreatedAt)
            .IsRequired();

        builder.Property(oi => oi.CreatedBy)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(oi => oi.UpdatedAt);

        builder.Property(oi => oi.UpdatedBy)
            .HasMaxLength(256);
    }
}