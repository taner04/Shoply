using Stripe;

namespace Shoply.WebApi.Features.WebHooks.Endpoints.Stripe.EventStrategies.RefundEvents;

public sealed partial class RefundUpdatedStrategy(
    ShoplyDbContext context,
    ILogger<RefundUpdatedStrategy> logger) : StripeEventStrategy<Refund>(context)
{
    public override string EventType => EventTypes.RefundUpdated;

    protected override async Task HandleEventAsync(Refund @event, Order order, CancellationToken cancellationToken)
    {
        if (@event.Status.Equals("succeeded", StringComparison.OrdinalIgnoreCase))
        {
            order.Payment.MarkRefunded(@event.Amount);
            await Context.SaveChangesAsync(cancellationToken);

            LogRefundStatusChangedForOrder(order.Id, @event.Status,
                @event.Amount);
        }
        else
        {
            LogRefundUpdatedForOrder(order.Id, @event.Status, @event.Amount);
        }
    }

    [LoggerMessage(LogLevel.Information,
        "Refund for order {OrderId} updated with status {RefundStatus} and amount {RefundAmount}, marked as refunded")]
    private partial void LogRefundStatusChangedForOrder(
        OrderId orderId,
        string refundStatus,
        long refundAmount);

    [LoggerMessage(LogLevel.Warning,
        "Refund for order {OrderId} updated with status {RefundStatus} and amount {RefundAmount}")]
    private partial void LogRefundUpdatedForOrder(
        OrderId orderId,
        string refundStatus,
        long refundAmount);
}