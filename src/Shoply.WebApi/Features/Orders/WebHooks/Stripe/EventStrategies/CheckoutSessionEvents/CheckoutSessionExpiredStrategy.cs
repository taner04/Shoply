using Shoply.WebApi.Common.Attributes;
using Stripe;

namespace Shoply.WebApi.Features.Orders.WebHooks.Stripe.EventStrategies.CheckoutSessionEvents;

[ServiceInjection<CheckoutSessionExpiredStrategy, IStripeEventStrategy>(ServiceLifetime.Scoped)]
public sealed partial class CheckoutSessionExpiredStrategy(
    ShoplyDbContext context,
    ILogger<CheckoutSessionExpiredStrategy> logger) : IStripeEventStrategy
{
    public string EventType => EventTypes.CheckoutSessionExpired;

    public async Task HandleNotification(Event stripeEvent, Order order, CancellationToken cancellationToken)
    {
        order.Payment.MarkFailed("Checkout session expired");
        order.MarkFailed();

        await context.SaveChangesAsync(cancellationToken);

        LogOrderAndPaymentMarkedAsFailed(logger, order.Id, order.Payment?.Id ?? PaymentId.From(Guid.Empty),
            stripeEvent.Id);
    }

    [LoggerMessage(LogLevel.Information,
        "Order {orderId} and Payment {paymentId} marked as failed from Stripe event {eventId} because checkout session expired")]
    static partial void LogOrderAndPaymentMarkedAsFailed(
        ILogger<CheckoutSessionExpiredStrategy> logger,
        OrderId orderId,
        PaymentId paymentId,
        string eventId);
}