using Api.Common.Attributes;
using Stripe;

namespace Api.Features.Orders.WebHooks.Stripe.EventStrategies.RefundEvents;

[ServiceInjection<RefundCreatedStrategy, IStripeEventStrategy>(ServiceLifetime.Scoped)]
public sealed partial class RefundCreatedStrategy(
    ApplicationDbContext context,
    ILogger<RefundCreatedStrategy> logger) : IStripeEventStrategy
{
    public string EventType => EventTypes.RefundCreated;
    
    public async Task HandleNotification(Event stripeEvent, Order order, CancellationToken cancellationToken)
    {
        var refund = (Refund)stripeEvent.Data.Object;
        
        var payment = order.Payment;
        if (payment.StripePaymentIntentId != refund.PaymentIntentId)
        {
            LogRefundPaymentIntentMismatch(logger, refund.Id, refund.PaymentIntentId ?? "null", stripeEvent.Id);
            return;
        }

        if (refund.Status == "succeeded")
        {
            var refundAmount = refund.Amount / 100m; // Convert from cents
            payment.MarkRefunded(refundAmount);
            await context.SaveChangesAsync(cancellationToken);

            LogRefundProcessed(logger, refund.Id, payment.Id.Value.ToString());
        }
    }

    [LoggerMessage(LogLevel.Information, "Refund {refundId} processed for order with PaymentIntentId {paymentIntentId}.")]
    static partial void LogRefundProcessed(ILogger<RefundCreatedStrategy> logger, string refundId,  string paymentIntentId);

    [LoggerMessage(LogLevel.Warning, "Refund {refundId} PaymentIntentId {paymentIntentId} does not match order payment from event {eventId}")]
    static partial void LogRefundPaymentIntentMismatch(ILogger<RefundCreatedStrategy> logger, string refundId, string paymentIntentId, string eventId);
}