using System.Collections.Concurrent;
using System.Text.Json;
using Shoply.WebApi.Common.Attributes;
using Shoply.WebApi.Features.WebHooks.Models;
using Stripe;

namespace Shoply.WebApi.Features.WebHooks.Endpoints.Stripe.EndpointFilter.Idempotency;

[ServiceInjection(ServiceLifetime.Singleton)]
internal sealed partial class IdempotencyService(
    ILogger<IdempotencyService> logger,
    IServiceScopeFactory  scopeFactory)
{
    private static readonly ConcurrentDictionary<string, SemaphoreSlim> EventLocks = new();

    public async ValueTask<object?> ProcessEventIdempotentlyAsync(
        Event stripeEvent,
        Func<ValueTask<object?>> processAsync)
    {
        var semaphore = EventLocks.GetOrAdd(stripeEvent.Id, _ => new SemaphoreSlim(1, 1));
        await semaphore.WaitAsync();
        try
        {
            using var scope = scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ShoplyDbContext>();
            ArgumentNullException.ThrowIfNull(dbContext);

            var stripeWebHookEvent = await dbContext.WebHookEvents.FirstOrDefaultAsync(w => w.EventId == stripeEvent.Id);
            if (stripeWebHookEvent is not null)
            {
                LogDuplicateStripeEventEventidOfTypeEventtypeDetected(stripeEvent.Id, stripeEvent.Type);
                stripeWebHookEvent.IncrementRetryCount();
                await dbContext.SaveChangesAsync();
                return null;
            }

            var result = await processAsync();

            LogProcessedStripeEventEventidOfTypeEventtype(stripeEvent.Id, stripeEvent.Type);
            stripeWebHookEvent = new WebHookEvent(WebHookEventType.Stripe, stripeEvent.Id, JsonSerializer.Serialize(stripeEvent));
            dbContext.WebHookEvents.Add(stripeWebHookEvent);
            await dbContext.SaveChangesAsync();

            return result;
        }
        finally
        {
            semaphore.Release();
            EventLocks.TryRemove(stripeEvent.Id, out _);
        }
    }

    [LoggerMessage(LogLevel.Information, "Processed Stripe event {EventId} of type {EventType}")]
    private partial void LogProcessedStripeEventEventidOfTypeEventtype(string eventId, string eventType);

    [LoggerMessage(LogLevel.Information, "Duplicate Stripe event {EventId} of type {EventType} detected.")]
    private partial void LogDuplicateStripeEventEventidOfTypeEventtypeDetected(string eventId, string eventType);
}