using Shoply.WebApi.Common.Attributes;
using Stripe;
using Stripe.Checkout;

namespace Shoply.WebApi.Features.Orders.WebHooks.Stripe.EventStrategies.CheckoutSessionEvents;

[ServiceInjection<CheckoutSessionPaymentSucceededStrategy, IStripeEventStrategy>(ServiceLifetime.Scoped)]
public sealed partial class CheckoutSessionPaymentSucceededStrategy(
    ShoplyDbContext context,
    ILogger<CheckoutSessionPaymentSucceededStrategy> logger) : IStripeEventStrategy
{
    public string EventType => EventTypes.CheckoutSessionAsyncPaymentSucceeded;

    public async Task HandleNotification(Event stripeEvent, Order order, CancellationToken cancellationToken)
    {
        var session = (Session)stripeEvent.Data.Object;

        order.Payment.SetStripePaymentIntentId(session.PaymentIntentId);
        order.Payment.MarkProcessing();
        order.MarkPaid();

        await context.SaveChangesAsync(cancellationToken);

        LogOrderAndPaymentMarkedAsSucceeded(logger, order.Id, order.Payment?.Id ?? PaymentId.From(Guid.Empty),
            stripeEvent.Id);
    }

    [LoggerMessage(LogLevel.Information,
        "Order {orderId} and Payment {paymentId} marked as succeeded from Stripe event {eventId}")]
    static partial void LogOrderAndPaymentMarkedAsSucceeded(
        ILogger<CheckoutSessionPaymentSucceededStrategy> logger,
        OrderId orderId,
        PaymentId paymentId,
        string eventId);
}