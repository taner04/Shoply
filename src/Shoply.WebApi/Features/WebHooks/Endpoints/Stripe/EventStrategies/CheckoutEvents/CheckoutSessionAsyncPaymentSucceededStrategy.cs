using Stripe;
using Stripe.Checkout;

namespace Shoply.WebApi.Features.WebHooks.Endpoints.Stripe.EventStrategies.CheckoutEvents;

public sealed partial class CheckoutSessionAsyncPaymentSucceededStrategy(
    ShoplyDbContext context,
    ILogger<CheckoutSessionAsyncPaymentSucceededStrategy> logger) : StripeEventStrategy<Session>(context)
{
    public override string EventType => EventTypes.CheckoutSessionAsyncPaymentSucceeded;

    protected override Task HandleEventAsync(
        Session @event,
        Order order,
        CancellationToken cancellationToken)
    {
        LogOrderOrderidPaid(order.Id);
        return Task.CompletedTask;
    }

    [LoggerMessage(LogLevel.Information, "Order {OrderId} paid")]
    private partial void LogOrderOrderidPaid(OrderId orderId);
}