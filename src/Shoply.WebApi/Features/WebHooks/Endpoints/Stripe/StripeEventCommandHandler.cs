using Microsoft.Extensions.Options;
using Shoply.WebApi.Common.Composition.Options;
using Shoply.WebApi.Features.WebHooks.Endpoints.Stripe.EventStrategies;
using Stripe;

namespace Shoply.WebApi.Features.WebHooks.Endpoints.Stripe;

public sealed partial class StripeEventCommandHandler(
    IHttpContextAccessor httpContextAccessor,
    IOptions<StripeConfig> stripeConfig,
    ILogger<StripeEventCommandHandler> logger,
    ShoplyDbContext context,
    IEnumerable<IStripeEventStrategy> strategies) : ICommandHandler<StripeEventCommand>
{
    public async ValueTask<Unit> Handle(StripeEventCommand command, CancellationToken cancellationToken)
    {
        var strategy = GetStrategy(command.EventType);
        if (strategy is null)
        {
            LogNoStrategyFoundForStripeEvent(logger, command.EventType);
            return Unit.Value;
        }

        if (!command.EventObjectV1.Data.Object.TryGetMetadata(out var orderId, out var userId, out var paymentIntentId,
                out var idempotencyKey))
        {
            LogMissingPaymentIntentId(logger, "", command.EventType);
            return Unit.Value;
        }

        var order = await context.Orders
            .Include(o => o.Payment)
            .Include(o => o.User)
            .FirstOrDefaultAsync(o => o.Payment.PaymentIntentId == paymentIntentId &&
                                      o.UserId == userId &&
                                      o.Id == orderId &&
                                      o.IdempotencyKey == idempotencyKey, cancellationToken);
        if (order is null)
        {
            LogOrderNotFound(logger, OrderId.From(Guid.Empty), command.EventType);
            return Unit.Value;
        }

        await strategy.HandleNotification(command.EventObjectV1, order, cancellationToken);

        LogStripeEventProcessedSuccessfully(logger, command.EventType, order.Id);

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

    [LoggerMessage(LogLevel.Warning, "No strategy found for Stripe event type: {eventType}")]
    static partial void LogNoStrategyFoundForStripeEvent(ILogger<StripeEventCommandHandler> logger, string eventType);

    [LoggerMessage(LogLevel.Warning, "Stripe event {eventId} of type {eventType} missing PaymentIntentId")]
    static partial void LogMissingPaymentIntentId(
        ILogger<StripeEventCommandHandler> logger,
        string eventId,
        string eventType);

    [LoggerMessage(LogLevel.Error, "Order with ID {orderId} not found for Stripe event {eventId}")]
    static partial void LogOrderNotFound(ILogger<StripeEventCommandHandler> logger, OrderId orderId, string eventId);

    [LoggerMessage(LogLevel.Information, "Successfully processed Stripe event {eventId} for order {orderId}")]
    static partial void LogStripeEventProcessedSuccessfully(
        ILogger<StripeEventCommandHandler> logger,
        string eventId,
        OrderId orderId);
}