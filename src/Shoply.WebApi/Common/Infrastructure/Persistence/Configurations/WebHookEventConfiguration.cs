using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Shoply.WebApi.Features.WebHooks.Models;

namespace Shoply.WebApi.Common.Infrastructure.Persistence.Configurations;

internal sealed class WebHookEventConfiguration : EntityConfiguration<WebHookEvent, WebHookEventId>
{
    protected override void PostConfigure(EntityTypeBuilder<WebHookEvent> builder)
    {
        builder.Property(e => e.EventType)
            .IsRequired()
            .HasConversion<EnumToStringConverter<WebHookEventType>>();

        builder.Property(e => e.Status)
            .IsRequired()
            .HasConversion<EnumToStringConverter<WebHookEventStatus>>();

        builder.Property(e => e.EventId)
            .IsRequired();

        builder.HasIndex(e => e.EventId)
            .IsUnique();

        builder.Property(e => e.Payload)
            .IsRequired();

        builder.Property(e => e.RetryCount)
            .IsRequired();
    }
}