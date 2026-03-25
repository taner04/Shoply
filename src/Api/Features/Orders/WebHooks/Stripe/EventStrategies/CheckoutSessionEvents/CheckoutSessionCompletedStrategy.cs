using Api.Common.Attributes;
using Stripe;

namespace Api.Features.Orders.WebHooks.Stripe.EventStrategies.CheckoutSessionEvents;

[ServiceInjection<CheckoutSessionCompletedStrategy, IStripeEventStrategy>(ServiceLifetime.Scoped)]
public sealed partial class CheckoutSessionCompletedStrategy(ILogger<CheckoutSessionCompletedStrategy> logger)
    : IStripeEventStrategy
{
    public string EventType => EventTypes.CheckoutSessionCompleted;

    public Task HandleNotification(Event stripeEvent, Order order, CancellationToken cancellationToken)
    {
        // No action needed, we will wait for async payment success or failure events to mark the order as paid or failed.
        LogReceivedCheckoutSessionCompleted(logger, stripeEvent.Id, order.Id);
        return Task.CompletedTask;
    }

    [LoggerMessage(LogLevel.Information, "Received checkout session completed event {eventId} for order {orderId}, waiting for async payment result")]
    static partial void LogReceivedCheckoutSessionCompleted(ILogger<CheckoutSessionCompletedStrategy> logger, string eventId, OrderId orderId);
}