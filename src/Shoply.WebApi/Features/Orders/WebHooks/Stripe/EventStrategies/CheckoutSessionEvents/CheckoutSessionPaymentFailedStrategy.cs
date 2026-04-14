using Shoply.WebApi.Common.Attributes;
using Stripe;
using Stripe.Checkout;

namespace Shoply.WebApi.Features.Orders.WebHooks.Stripe.EventStrategies.CheckoutSessionEvents;

[ServiceInjection<CheckoutSessionPaymentFailedStrategy, IStripeEventStrategy>(ServiceLifetime.Scoped)]
public sealed partial class CheckoutSessionPaymentFailedStrategy(
    ShoplyDbContext context,
    ILogger<CheckoutSessionPaymentFailedStrategy> logger) : IStripeEventStrategy
{
    public string EventType => EventTypes.CheckoutSessionAsyncPaymentFailed;

    public async Task HandleNotification(Event stripeEvent, Order order, CancellationToken cancellationToken)
    {
        var session = (Session)stripeEvent.Data.Object;

        if (string.IsNullOrEmpty(order.Payment.StripePaymentIntentId) && !string.IsNullOrEmpty(session.PaymentIntentId))
        {
            order.Payment.SetStripePaymentIntentId(session.PaymentIntentId);
        }

        order.Payment.MarkFailed("Payment Failed");
        order.MarkFailed();

        await context.SaveChangesAsync(cancellationToken);

        LogOrderAndPaymentMarkedAsFailed(logger, order.Id, order.Payment?.Id ?? PaymentId.From(Guid.Empty),
            "Payment failed", stripeEvent.Id);
    }

    [LoggerMessage(LogLevel.Information,
        "Order {orderId} and Payment {paymentId} marked as failed from Stripe event {eventId}: {failureReason}")]
    static partial void LogOrderAndPaymentMarkedAsFailed(
        ILogger<CheckoutSessionPaymentFailedStrategy> logger,
        OrderId orderId,
        PaymentId paymentId,
        string failureReason,
        string eventId);
}