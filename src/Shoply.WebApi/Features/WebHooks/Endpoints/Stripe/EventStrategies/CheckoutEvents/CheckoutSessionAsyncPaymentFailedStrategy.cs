using Stripe;
using Stripe.Checkout;

namespace Shoply.WebApi.Features.WebHooks.Endpoints.Stripe.EventStrategies.CheckoutEvents;

public sealed partial class CheckoutSessionAsyncPaymentFailedStrategy(
    ShoplyDbContext context,
    ILogger<CheckoutSessionAsyncPaymentFailedStrategy> logger) : StripeEventStrategy<Session>(context)
{
    public override string EventType => EventTypes.CheckoutSessionAsyncPaymentFailed;

    protected override Task HandleEventAsync(
        Session @event,
        Order order,
        CancellationToken cancellationToken)
    {
        LogPaymentFailedForOrderid(order.Id);
        return Task.CompletedTask;
    }

    [LoggerMessage(LogLevel.Warning, "Payment Failed for {OrderId}")]
    private partial void LogPaymentFailedForOrderid(OrderId orderId);
}