using Stripe;

namespace Shoply.WebApi.Features.WebHooks.Endpoints.Stripe.EventStrategies.RefundEvents;

public sealed partial class RefundFailedStrategy(
    ShoplyDbContext context,
    ILogger<RefundFailedStrategy> logger) : StripeEventStrategy<Refund>(context)
{
    public override string EventType => EventTypes.RefundFailed;
    protected override Task HandleEventAsync(Refund @event, Order order, CancellationToken cancellationToken)
    {
        LogHandlingRefundFailedEventForOrderOrderidWithRefundAmountRefundamount(order.Id, @event.Amount);
        return Task.CompletedTask;
    }

    [LoggerMessage(LogLevel.Warning, "Handling refund failed event for order {OrderId} with refund amount {RefundAmount}")]
    private partial void LogHandlingRefundFailedEventForOrderOrderidWithRefundAmountRefundamount(OrderId orderId, long refundAmount);
}