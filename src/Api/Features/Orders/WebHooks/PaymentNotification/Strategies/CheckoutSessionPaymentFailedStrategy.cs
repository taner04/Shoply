using Api.Common.Attributes;
using Api.Common.Infrastructure.Persistence;
using Api.Features.Orders.Models;
using Stripe;

namespace Api.Features.Orders.WebHooks.PaymentNotification.Strategies;

[ServiceInjection<CheckoutSessionPaymentFailedStrategy, IPaymentNotificationStrategy>(ServiceLifetime.Scoped)]
public sealed class CheckoutSessionPaymentFailedStrategy(
    ApplicationDbContext context,
    ILogger<CheckoutSessionPaymentFailedStrategy> logger) : IPaymentNotificationStrategy
{
    public string EventType => EventTypes.CheckoutSessionAsyncPaymentFailed;

    public async Task HandleNotification(Event stripeEvent, Order order, CancellationToken cancellationToken)
    {
        order.MarkFailed();
        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Order {orderId} marked as failed from Stripe event {eventId}", order.Id, stripeEvent.Id);
    }
}