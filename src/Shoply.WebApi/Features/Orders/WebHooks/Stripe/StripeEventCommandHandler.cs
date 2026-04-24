using Microsoft.Extensions.Options;
using Shoply.WebApi.Common.Composition.Options;
using Shoply.WebApi.Features.Orders.WebHooks.Stripe.EventStrategies;
using Stripe;

namespace Shoply.WebApi.Features.Orders.WebHooks.Stripe;

public sealed partial class StripeEventCommandHandler(
    IHttpContextAccessor httpContextAccessor,
    IOptions<StripeConfig> stripeConfig,
    ILogger<StripeEventCommandHandler> logger,
    ShoplyDbContext context,
    IEnumerable<IStripeEventStrategy> strategies) : ICommandHandler<StripeEventCommand>
{
    public async ValueTask<Unit> Handle(StripeEventCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var strategy = GetStrategy(command.EventType);
            if (strategy is null)
            {
                LogNoStrategyFoundForStripeEvent(logger, command.EventType);
                return Unit.Value;
            }
            
            if(!command.Metadata.TryGetPaymentIntentId(out var paymentIntentId) || 
               !command.Metadata.TryGetOrderId(out var orderId) || 
               !command.Metadata.TryGetUserId(out var userId) || 
               !command.Metadata.TryGetIdempotencyKey(out var idempotencyKey))
            {
                LogMissingPaymentIntentId(logger,"",  command.EventType);
                return Unit.Value;
            }
            
            var order = await context.Orders
                .Include(o => o.Payment)
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.Payment.PaymentIntentId == paymentIntentId &&
                                                o.UserId == userId &&
                                                o.Id == orderId &&
                                                o.IdempotencyKey == idempotencyKey, cancellationToken: cancellationToken);
            if(order is null)
            {
                LogOrderNotFound(logger, OrderId.From(Guid.Empty), command.EventType);
                return Unit.Value;
            }
            
            await strategy.HandleNotification(command.Metadata, order, cancellationToken);

            LogStripeEventProcessedSuccessfully(logger, command.EventType, order.Id);
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

    [LoggerMessage(LogLevel.Warning, "Stripe event {eventId} of type {eventType} missing PaymentIntentId")]
    static partial void LogMissingPaymentIntentId(
        ILogger<StripeEventCommandHandler> logger,
        string eventId,
        string eventType);

    [LoggerMessage(LogLevel.Error, "Order with ID {orderId} not found for Stripe event {eventId}")]
    static partial void LogOrderNotFound(ILogger<StripeEventCommandHandler> logger, OrderId orderId, string eventId);

    [LoggerMessage(LogLevel.Error,
        "Order with PaymentIntentId {paymentIntentId} not found for Stripe event {eventId}")]
    static partial void LogOrderNotFoundByPaymentIntent(
        ILogger<StripeEventCommandHandler> logger,
        string paymentIntentId,
        string eventId);

    [LoggerMessage(LogLevel.Warning,
        "Unsupported Stripe data object for event {eventId} type {eventType}: {dataObjectType}")]
    static partial void LogUnsupportedStripeDataObject(
        ILogger<StripeEventCommandHandler> logger,
        string eventId,
        string eventType,
        string dataObjectType);

    [LoggerMessage(LogLevel.Information, "Successfully processed Stripe event {eventId} for order {orderId}")]
    static partial void LogStripeEventProcessedSuccessfully(
        ILogger<StripeEventCommandHandler> logger,
        string eventId,
        OrderId orderId);
}