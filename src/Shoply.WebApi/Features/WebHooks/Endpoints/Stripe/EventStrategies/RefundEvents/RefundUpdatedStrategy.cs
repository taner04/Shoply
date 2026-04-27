using Shoply.WebApi.Common.Attributes;
using Shoply.WebApi.Features.WebHooks.Endpoints.Stripe.EventObjects.V1;
using Stripe;

namespace Shoply.WebApi.Features.WebHooks.Endpoints.Stripe.EventStrategies.RefundEvents;

[ServiceInjection<RefundUpdatedStrategy, IStripeEventStrategy>(ServiceLifetime.Scoped)]
public sealed class RefundUpdatedStrategy(
    ShoplyDbContext context,
    ILogger<RefundUpdatedStrategy> logger) : IStripeEventStrategy
{
    public string EventType => EventTypes.RefundUpdated;

    public Task HandleNotification(
        StripeEventObjectV1 stripEventObjectV1,
        Order order,
        CancellationToken cancellationToken) => throw new NotImplementedException();
}