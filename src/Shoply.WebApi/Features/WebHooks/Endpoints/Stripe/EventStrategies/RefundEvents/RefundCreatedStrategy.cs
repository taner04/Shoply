using Stripe;

namespace Shoply.WebApi.Features.WebHooks.Endpoints.Stripe.EventStrategies.RefundEvents;

public sealed partial class RefundCreatedStrategy(
    ShoplyDbContext context,
    ILogger<RefundCreatedStrategy> logger) : StripeEventStrategy<Refund>(context)
{
    public override string EventType => EventTypes.RefundCreated;

    protected override Task HandleEventAsync(
        Refund @event,
        Order order,
        CancellationToken cancellationToken)
    {
        LogRefundCreatedForOrder(order.Id, @event.Amount);
        return Task.CompletedTask;
    }

    [LoggerMessage(LogLevel.Information,
        "Handling refund created event for order {OrderId} with refund amount {RefundAmount}")]
    private partial void LogRefundCreatedForOrder(
        OrderId orderId,
        long refundAmount);
}