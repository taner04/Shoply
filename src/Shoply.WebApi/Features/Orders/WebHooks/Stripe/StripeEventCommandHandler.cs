using Microsoft.Extensions.Options;
using Shoply.WebApi.Common.Composition.Options;
using Shoply.WebApi.Features.Orders.WebHooks.Stripe.EventStrategies;
using Stripe;
using Stripe.Checkout;

namespace Shoply.WebApi.Features.Orders.WebHooks.Stripe;

public sealed partial class StripeEventCommandHandler(
    IHttpContextAccessor httpContextAccessor,
    IOptions<StripeConfig> stripeConfig,
    ILogger<StripeEventCommandHandler> logger,
    ShoplyDbContext context,
    IEnumerable<IStripeEventStrategy> strategies) : ICommandHandler<StripeEventCommand>
{
    public async ValueTask<Unit> Handle(StripeEventCommand _, CancellationToken cancellationToken)
    {
        try
        {
            var stripeEvent = await ReconstructStripeEventAsync(cancellationToken);
            if (stripeEvent is null)
            {
                LogFailedToReconstructStripeEvent(logger);
                return Unit.Value;
            }

            var strategy = GetStrategy(stripeEvent.Type);
            if (strategy is null)
            {
                return Unit.Value;
            }

            if (stripeEvent.Data.Object is not Session session ||
                !session.TryExtractOrderAndUserId(out var orderId, out var userId))
            {
                LogMissingMetadata(logger, stripeEvent.Id);
                return Unit.Value;
            }

            var order = await context.Orders
                .Include(o => o.Payment)
                .FirstOrDefaultAsync(o => o.Id == orderId, cancellationToken);

            if (order is null)
            {
                LogOrderNotFound(logger, orderId, stripeEvent.Id);
                return Unit.Value;
            }

            await strategy.HandleNotification(stripeEvent, order, cancellationToken);

            LogStripeEventProcessedSuccessfully(logger, stripeEvent.Id, orderId);
        }
        catch (StripeException ex)
        {
            LogConstructEventFailed(logger, ex.StripeError?.Code ?? "Unkown");
        }

        return Unit.Value;
    }

    private async Task<Event?> ReconstructStripeEventAsync(CancellationToken cancellationToken)
    {
        var httpRequest = httpContextAccessor.HttpContext!.Request;
        using var reader = new StreamReader(httpRequest.Body);

        return EventUtility.ConstructEvent(
            await reader.ReadToEndAsync(cancellationToken),
            httpRequest.Headers["Stripe-Signature"],
            stripeConfig.Value.WebhookSecret
        );
    }

    private IStripeEventStrategy? GetStrategy(string eventType)
    {
        if (strategies.FirstOrDefault(s => s.EventType.Equals(eventType, StringComparison.OrdinalIgnoreCase)) is
            { } strategy)
        {
            return strategy;
        }

        LogNoStrategyFoundForStripeEvent(logger, eventType);
        return null;
    }

    [LoggerMessage(LogLevel.Error,
        "Failed to construct Stripe event from webhook payload. Stripe error code: {stripeErrorCode}")]
    static partial void LogConstructEventFailed(ILogger<StripeEventCommandHandler> logger, string stripeErrorCode);

    [LoggerMessage(LogLevel.Warning, "No strategy found for Stripe event type: {eventType}")]
    static partial void LogNoStrategyFoundForStripeEvent(ILogger<StripeEventCommandHandler> logger, string eventType);

    [LoggerMessage(LogLevel.Warning, "Failed to reconstruct Stripe event from webhook payload")]
    static partial void LogFailedToReconstructStripeEvent(ILogger<StripeEventCommandHandler> logger);

    [LoggerMessage(LogLevel.Warning, "Stripe event {eventId} missing OrderId or UserId in metadata")]
    static partial void LogMissingMetadata(ILogger<StripeEventCommandHandler> logger, string eventId);

    [LoggerMessage(LogLevel.Error, "Order with ID {orderId} not found for Stripe event {eventId}")]
    static partial void LogOrderNotFound(ILogger<StripeEventCommandHandler> logger, OrderId orderId, string eventId);

    [LoggerMessage(LogLevel.Information, "Successfully processed Stripe event {eventId} for order {orderId}")]
    static partial void LogStripeEventProcessedSuccessfully(
        ILogger<StripeEventCommandHandler> logger,
        string eventId,
        OrderId orderId);
}