using Api.Common.Attributes;
using Api.Features.Orders.Models;
using Stripe;

namespace Api.Features.Orders.WebHooks.PaymentNotification.Strategies;

[ServiceInjection<CheckoutSessionCompletedStrategy, IPaymentNotificationStrategy>(ServiceLifetime.Scoped)]
public class CheckoutSessionCompletedStrategy(ILogger<CheckoutSessionCompletedStrategy> logger)
    : IPaymentNotificationStrategy
{
    public string EventType => EventTypes.CheckoutSessionCompleted;

    public Task HandleNotification(Event stripeEvent, Order order, CancellationToken cancellationToken)
    {
        // No action needed, we will wait for async payment success or failure events to mark the order as paid or failed.
        logger.LogInformation(
            "Received checkout session completed event {eventId} for order {orderId}, waiting for async payment result",
            stripeEvent.Id, order.Id);
        return Task.CompletedTask;
    }
}