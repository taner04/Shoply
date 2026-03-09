using Api.Common.Domain;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Common.Infrastructure.Persistence.Configurations.Common;

public abstract class AggregateConfiguration<TEntity, TId> : IEntityTypeConfiguration<TEntity>
    where TEntity : AggregateRoot<TId>
    where TId : struct
{
    public void Configure(EntityTypeBuilder<TEntity> builder)
    {
        // IEntity configuration
        builder.ToTable(typeof(TEntity).Name + "s");
        builder.HasKey(t => t.Id);

        // IAuditable configuration
        builder.Property(t => t.CreatedAt)
            .IsRequired();

        builder.Property(t => t.CreatedBy)
            .IsRequired()
            .HasMaxLength(Auditable.MaxCreatedByLength);

        builder.Property(t => t.UpdatedAt);

        builder.Property(t => t.UpdatedBy)
            .HasMaxLength(Auditable.MaxUpdatedByLength);

        PostConfigure(builder);
    }

    /// <summary>
    ///     Hook method for additional configurations in derived classes.
    ///     Executed after the base configuration to add entity-specific settings.
    /// </summary>
    /// <param name="builder">The <see cref="EntityTypeBuilder{TEntity}" /> for further configurations.</param>
    protected abstract void PostConfigure(
        EntityTypeBuilder<TEntity> builder);
}