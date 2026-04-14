using Stripe;

namespace Shoply.WebApi.Features.Orders.WebHooks.Stripe.EventStrategies;

public interface IStripeEventStrategy
{
    string EventType { get; }

    Task HandleNotification(Event stripeEvent, Order order, CancellationToken cancellationToken);
}