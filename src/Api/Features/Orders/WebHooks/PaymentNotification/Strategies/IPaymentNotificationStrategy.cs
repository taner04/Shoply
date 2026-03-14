using Api.Features.Orders.Models;
using Stripe;

namespace Api.Features.Orders.WebHooks.PaymentNotification.Strategies;

public interface IPaymentNotificationStrategy
{
    string EventType { get; }

    Task HandleNotification(Event stripeEvent, Order order, CancellationToken cancellationToken);
}