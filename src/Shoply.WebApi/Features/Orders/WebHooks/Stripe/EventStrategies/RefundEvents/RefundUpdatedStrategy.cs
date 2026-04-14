using Shoply.WebApi.Common.Attributes;
using Stripe;

namespace Shoply.WebApi.Features.Orders.WebHooks.Stripe.EventStrategies.RefundEvents;

[ServiceInjection<RefundUpdatedStrategy, IStripeEventStrategy>(ServiceLifetime.Scoped)]
public sealed partial class RefundUpdatedStrategy(
    ShoplyDbContext context,
    ILogger<RefundUpdatedStrategy> logger) : IStripeEventStrategy
{
    public string EventType => EventTypes.RefundUpdated;

    public async Task HandleNotification(Event stripeEvent, Order order, CancellationToken cancellationToken)
    {
        var refund = (Refund)stripeEvent.Data.Object;

        var payment = order.Payment;
        if (payment.StripePaymentIntentId != refund.PaymentIntentId)
        {
            LogRefundPaymentIntentMismatch(logger, refund.Id, refund.PaymentIntentId ?? "null", stripeEvent.Id);
            return;
        }

        switch (refund.Status)
        {
            case "succeeded":
            {
                var refundAmount = refund.Amount / 100m; // Convert from cents
                if (payment.Status is PaymentStatus.Succeeded or PaymentStatus.PartiallyRefunded)
                {
                    payment.MarkRefunded(refundAmount);
                    await context.SaveChangesAsync(cancellationToken);
                    LogRefundSucceeded(logger, order.Id, payment.Id, refundAmount, stripeEvent.Id);
                }

                break;
            }
            case "failed":
            {
                if (payment.Status is not PaymentStatus.Failed)
                {
                    payment.MarkFailed($"Refund failed: {refund.FailureReason ?? "Unknown reason"}");
                    await context.SaveChangesAsync(cancellationToken);
                    LogRefundFailed(logger, order.Id, payment.Id, stripeEvent.Id);
                }

                break;
            }
        }
    }

    [LoggerMessage(LogLevel.Information,
        "Refund succeeded for order {orderId} payment {paymentId}, amount: {amount} from Stripe event {eventId}")]
    static partial void LogRefundSucceeded(
        ILogger<RefundUpdatedStrategy> logger,
        OrderId orderId,
        PaymentId paymentId,
        decimal amount,
        string eventId);

    [LoggerMessage(LogLevel.Warning,
        "Refund failed for order {orderId} payment {paymentId} from Stripe event {eventId}")]
    static partial void LogRefundFailed(
        ILogger<RefundUpdatedStrategy> logger,
        OrderId orderId,
        PaymentId paymentId,
        string eventId);

    [LoggerMessage(LogLevel.Warning,
        "Refund {refundId} PaymentIntentId {paymentIntentId} does not match order payment from event {eventId}")]
    static partial void LogRefundPaymentIntentMismatch(
        ILogger<RefundUpdatedStrategy> logger,
        string refundId,
        string paymentIntentId,
        string eventId);
}