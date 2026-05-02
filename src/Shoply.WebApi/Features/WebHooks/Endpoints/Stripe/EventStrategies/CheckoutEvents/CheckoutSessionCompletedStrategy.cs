using Shoply.WebApi.Features.Orders.Enums;
using Shoply.WebApi.Features.Orders.Exceptions;
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
        if (!@event.PaymentStatus.Equals("unpaid", StringComparison.OrdinalIgnoreCase))
        {
            if (order.Status != OrderStatus.Delivered)
            {
                throw new InvalidOrderPaymentStatusException(order.Status, OrderStatus.Processing);
            }

            if (order.Payment.Status is not PaymentStatus.Pending)
            {
                throw new InvalidPaymentStatusTransitionException(order.Payment.Status, PaymentStatus.Paid);
            }

            order.Payment.Status = PaymentStatus.Paid;
            order.Status = OrderStatus.Processing;

            LogCheckoutSessionCompletedAndPaid(order.Id);
        }
        else
        {
            if (order.Status != OrderStatus.Delivered)
            {
                throw new InvalidOrderPaymentStatusException(order.Status, OrderStatus.Cancelled);
            }

            order.Status = OrderStatus.Cancelled;

            if (order.Payment.Status is not PaymentStatus.Paid)
            {
                throw new InvalidPaymentStatusTransitionException(order.Payment.Status, PaymentStatus.Canceled);
            }

            order.Payment.Status = PaymentStatus.Canceled;

            LogCheckoutSessionCompletedButUnpaid(order.Id);
        }

        await Context.SaveChangesAsync(cancellationToken);
    }

    [LoggerMessage(LogLevel.Information, "Checkout session for order {OrderId} completed and marked as paid")]
    private partial void LogCheckoutSessionCompletedAndPaid(OrderId orderId);

    [LoggerMessage(LogLevel.Warning, "Checkout session for order {OrderId} completed with unpaid status")]
    private partial void LogCheckoutSessionCompletedButUnpaid(OrderId orderId);
}