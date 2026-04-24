using Stripe;

namespace Shoply.WebApi.Features.Orders.WebHooks.Stripe.EventStrategies;

public interface IStripeEventStrategy
{
    string EventType { get; }

    Task HandleNotification(IHasMetadata metadata, Order order, CancellationToken cancellationToken);
}