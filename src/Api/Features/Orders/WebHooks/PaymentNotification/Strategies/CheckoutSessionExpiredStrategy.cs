using Api.Common.Attributes;
using Api.Common.Infrastructure.Persistence;
using Api.Features.Orders.Models;
using Stripe;

namespace Api.Features.Orders.WebHooks.PaymentNotification.Strategies;

[ServiceInjection<CheckoutSessionExpiredStrategy, IPaymentNotificationStrategy>(ServiceLifetime.Scoped)]
public sealed class CheckoutSessionExpiredStrategy(
    ApplicationDbContext context,
    ILogger<CheckoutSessionExpiredStrategy> logger) : IPaymentNotificationStrategy
{
    public string EventType => EventTypes.CheckoutSessionExpired;

    public async Task HandleNotification(Event stripeEvent, Order order, CancellationToken cancellationToken)
    {
        order.MarkFailed();
        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Order {orderId} marked as failed from Stripe event {eventId} because checkout session expired", order.Id,
            stripeEvent.Id);
    }
}