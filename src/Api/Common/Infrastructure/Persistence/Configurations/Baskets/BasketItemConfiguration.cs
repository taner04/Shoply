using Api.Common.Domain.Baskets;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Common.Infrastructure.Persistence.Configurations.Baskets;

public sealed class BasketItemConfiguration : IEntityTypeConfiguration<BasketItem>
{
    public void Configure(EntityTypeBuilder<BasketItem> builder)
    {
        builder.ToTable("BasketItems");

        builder.HasKey(bi => bi.Id);

        // Foreign Key to Product

        // Quantity
        builder.Property(bi => bi.Quantity)
            .IsRequired()
            .HasDefaultValue(1);

        // Relationship to Product
        builder.HasOne(bi => bi.Product)
            .WithMany()
            .HasForeignKey(bi => bi.ProductId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict); // Prevent product deletion if in basket

        // Composite index for basket + product
        builder.HasIndex(bi => new { bi.ProductId })
            .HasDatabaseName("idx_basketitem_productid");

        // Audit properties - inherited from Entity -> Auditable
        builder.Property(bi => bi.CreatedAt)
            .IsRequired();

        builder.Property(bi => bi.CreatedBy)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(bi => bi.UpdatedAt);

        builder.Property(bi => bi.UpdatedBy)
            .HasMaxLength(256);
    }
}