using Shoply.WebApi.Features.WebHooks.Endpoints.Stripe.EventObjects.V1;

namespace Shoply.WebApi.Features.WebHooks.Endpoints.Stripe.EventStrategies;

public interface IStripeEventStrategy
{
    string EventType { get; }

    Task HandleNotification(StripeEventObjectV1 stripEventObjectV1, Order order, CancellationToken cancellationToken);
}