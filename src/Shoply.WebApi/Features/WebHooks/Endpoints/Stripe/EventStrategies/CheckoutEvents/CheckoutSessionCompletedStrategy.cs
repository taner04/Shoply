using Stripe;
using Stripe.Checkout;

namespace Shoply.WebApi.Features.WebHooks.Endpoints.Stripe.EventStrategies.CheckoutEvents;

public sealed partial class CheckoutSessionCompletedStrategy(
    ShoplyDbContext context,
    ILogger<CheckoutSessionCompletedStrategy> logger)
    : StripeEventStrategy<Session>(context)
{
    public override string EventType => EventTypes.CheckoutSessionCompleted;

    protected override async Task HandleEventAsync(
        Session @event,
        Order order,
        CancellationToken cancellationToken)
    {
        if(!@event.PaymentStatus.Equals("unpaid", StringComparison.OrdinalIgnoreCase))
        {
            order.MarkProcessing();
            order.Payment.MarkPaid();
            
            LogCheckoutSessionForOrderOrderidCompletedAndMarkedAsPaid(order.Id);
        }
        else
        {
            order.MarkCancelled();
            order.Payment.MarkCanceled();
            
            LogCheckoutSessionForOrderOrderidCompletedWithUnpaidStatus(order.Id);
        }
        
        await Context.SaveChangesAsync(cancellationToken);
    }

    [LoggerMessage(LogLevel.Information, "Checkout session for order {OrderId} completed and marked as paid")]
    private partial void LogCheckoutSessionForOrderOrderidCompletedAndMarkedAsPaid(OrderId orderId);
    
    [LoggerMessage(LogLevel.Warning, "Checkout session for order {OrderId} completed with unpaid status")]
    private partial void LogCheckoutSessionForOrderOrderidCompletedWithUnpaidStatus(OrderId orderId);
}