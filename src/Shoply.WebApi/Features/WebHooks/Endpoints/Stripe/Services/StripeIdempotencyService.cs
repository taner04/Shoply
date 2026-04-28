using System.Collections.Concurrent;
using System.Text.Json;
using Hangfire;
using Shoply.WebApi.Features.WebHooks.Models;
using Stripe;

namespace Shoply.WebApi.Features.WebHooks.Endpoints.Stripe.Services;
internal sealed class StripeIdempotencyService(
    IBackgroundJobClient backgroundJobClient,
    IServiceScopeFactory scopeFactory)
{
    private static readonly ConcurrentDictionary<string, SemaphoreSlim> EventLocks = new();

    public async ValueTask AddEventIdempotentlyAsync(Event @event)
    {
        var semaphore = EventLocks.GetOrAdd(@event.Id, _ => new SemaphoreSlim(1, 1));
        await semaphore.WaitAsync();
        try
        {
            using var scope = scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ShoplyDbContext>();

            if (await dbContext.WebHookEvents.FirstOrDefaultAsync(w => w.EventId == @event.Id) is null)
            {
                var newEvent = new WebHookEvent(WebHookEventType.Stripe, @event.Id, JsonSerializer.Serialize(@event));
                dbContext.WebHookEvents.Add(newEvent);
                await dbContext.SaveChangesAsync();

                backgroundJobClient.Enqueue<StripeEventProcessor>(processor =>
                    processor.ProcessEventAsync(newEvent.Id, CancellationToken.None));
            }
        }
        finally
        {
            semaphore.Release();
            if (semaphore.CurrentCount == 1)
            {
                EventLocks.TryRemove(@event.Id, out _);
            }
        }
    }
}