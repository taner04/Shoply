using Stripe;

namespace Shoply.WebApi.Features.WebHooks.Endpoints.Stripe.EventStrategies;

public interface IStripeEventStrategy
{
    string EventType { get; }

    Task HandleEventAsync(EventData eventData, CancellationToken cancellationToken);
}