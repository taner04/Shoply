using Api.Common.Attributes;
using Stripe;

namespace Api.Features.Orders.WebHooks.Stripe.EventStrategies.RefundEvents;

[ServiceInjection<RefundFailedStrategy, IStripeEventStrategy>(ServiceLifetime.Scoped)]
public sealed partial class RefundFailedStrategy(
    ApplicationDbContext context,
    ILogger<RefundFailedStrategy> logger) : IStripeEventStrategy
{
    public string EventType => EventTypes.RefundFailed;
    
    public async Task HandleNotification(Event stripeEvent, Order order, CancellationToken cancellationToken)
    {
        var refund = (Refund)stripeEvent.Data.Object;
        
        var payment = order.Payment;
        if (payment.StripePaymentIntentId != refund.PaymentIntentId)
        {
            LogRefundPaymentIntentMismatch(logger, refund.Id, refund.PaymentIntentId ?? "null", stripeEvent.Id);
            return;
        }

        if (payment.Status is not PaymentStatus.Failed)
        {
            var failureReason = $"Refund failed: {refund.FailureReason ?? "Unknown reason"}";
            payment.MarkFailed(failureReason);
            await context.SaveChangesAsync(cancellationToken);

            LogRefundFailed(logger, order.Id, payment.Id, refund.FailureReason ?? "Unknown", stripeEvent.Id);
        }
    }

    [LoggerMessage(LogLevel.Warning, "Refund failed for order {orderId} payment {paymentId}: {failureReason} from Stripe event {eventId}")]
    static partial void LogRefundFailed(ILogger<RefundFailedStrategy> logger, OrderId orderId, PaymentId paymentId, string failureReason, string eventId);

    [LoggerMessage(LogLevel.Warning, "Refund {refundId} PaymentIntentId {paymentIntentId} does not match order payment from event {eventId}")]
    static partial void LogRefundPaymentIntentMismatch(ILogger<RefundFailedStrategy> logger, string refundId, string paymentIntentId, string eventId);
}