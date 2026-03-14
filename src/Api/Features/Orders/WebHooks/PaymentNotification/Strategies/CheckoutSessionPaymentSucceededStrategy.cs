using Api.Common.Attributes;
using Api.Common.Infrastructure.Persistence;
using Api.Features.Orders.Models;
using Stripe;

namespace Api.Features.Orders.WebHooks.PaymentNotification.Strategies;

[ServiceInjection<CheckoutSessionPaymentSucceededStrategy, IPaymentNotificationStrategy>(ServiceLifetime.Scoped)]
public sealed class CheckoutSessionPaymentSucceededStrategy(
    ApplicationDbContext context,
    Logger<CheckoutSessionPaymentSucceededStrategy> logger) : IPaymentNotificationStrategy
{
    public string EventType => EventTypes.CheckoutSessionAsyncPaymentSucceeded;

    public async Task HandleNotification(Event stripeEvent, Order order, CancellationToken cancellationToken)
    {
        order.MarkPaid();
        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Order {orderId} marked as paid from async payment success {eventId}", order.Id,
            stripeEvent.Id);
    }
}