using Stripe;
using Stripe.Checkout;

namespace Shoply.WebApi.Features.WebHooks.Endpoints.Stripe.EventStrategies.CheckoutEvents;

public sealed partial class CheckoutSessionExpiredStrategy(
    ShoplyDbContext context,
    ILogger<CheckoutSessionExpiredStrategy> logger) : StripeEventStrategy<Session>(context)
{
    public override string EventType => EventTypes.CheckoutSessionExpired;

    protected override Task HandleEventAsync(
        Session @event,
        Order order,
        CancellationToken cancellationToken)
    {
        LogMarkedOrderOrderidAsCancelledDueToCheckoutSessionExpiration(order.Id);
        return Task.CompletedTask;
    }

    [LoggerMessage(LogLevel.Warning, "Marked order {orderId} as cancelled due to checkout session expiration")]
    private partial void LogMarkedOrderOrderidAsCancelledDueToCheckoutSessionExpiration(OrderId orderId);
}