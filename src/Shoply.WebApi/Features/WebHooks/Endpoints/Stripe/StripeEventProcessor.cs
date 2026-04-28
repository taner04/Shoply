using Hangfire;
using Shoply.WebApi.Features.WebHooks.Endpoints.Stripe.EventStrategies;
using Shoply.WebApi.Features.WebHooks.Models;
using Stripe;

namespace Shoply.WebApi.Features.WebHooks.Endpoints.Stripe;

[AutomaticRetry(Attempts = 5)]
public sealed partial class StripeEventProcessor(
    ShoplyDbContext context,
    IEnumerable<IStripeEventStrategy> strategies,
    ILogger<StripeEventProcessor> logger)
{
    public async Task ProcessEventAsync(WebHookEventId eventId, CancellationToken cancellationToken)
    {
        if (await context.WebHookEvents.FirstOrDefaultAsync(e => e.Id == eventId, cancellationToken) is not
            { } webHookEvent)
        {
            LogWebhookEventEventidNotFound(eventId);
            return;
        }

        webHookEvent.IncrementRetryCount();
        var stripeEvent = EventUtility.ParseEvent(webHookEvent.Payload);
        if (stripeEvent is null)
        {
            LogFailedToDeserializeWebhookEventEventid(eventId);
            return;
        }

        var strategy = strategies.FirstOrDefault(s => s.EventType == stripeEvent.Type);
        if (strategy is null)
        {
            LogFailedToFindStrategyStrategy(stripeEvent.Type);
            return;
        }

        try
        {
            await strategy.HandleEventAsync(stripeEvent.Data, cancellationToken);
            webHookEvent.MarkHandled();
        }
        catch (Exception e)
        {
            webHookEvent.MarkFailed();
            LogFailedToHandleWebhookEventEventid(eventId, e);
            throw;
        }
        finally
        {
            await context.SaveChangesAsync(cancellationToken);
            LogWebhookEventEventidHandled(eventId);
        }
    }

    [LoggerMessage(LogLevel.Warning, "Failed to deserialize webhook event {eventId}")]
    private partial void LogFailedToDeserializeWebhookEventEventid(WebHookEventId eventId);

    [LoggerMessage(LogLevel.Warning, "Failed to find strategy {strategy}")]
    private partial void LogFailedToFindStrategyStrategy(string strategy);

    [LoggerMessage(LogLevel.Information, "Webhook event {eventId} handled")]
    private partial void LogWebhookEventEventidHandled(WebHookEventId eventId);

    [LoggerMessage(LogLevel.Error, "Failed to handle webhook event {eventId}")]
    private partial void LogFailedToHandleWebhookEventEventid(WebHookEventId eventId, Exception exception);

    [LoggerMessage(LogLevel.Warning, "Webhook event {eventId} not found")]
    private partial void LogWebhookEventEventidNotFound(WebHookEventId eventId);
}