using Shoply.WebApi.Common.Attributes;
using Shoply.WebApi.Features.WebHooks.Endpoints.Stripe.EventObjects.V1;
using Stripe;

namespace Shoply.WebApi.Features.WebHooks.Endpoints.Stripe.EventStrategies.RefundEvents;

[ServiceInjection<RefundCreatedStrategy, IStripeEventStrategy>(ServiceLifetime.Scoped)]
public sealed class RefundCreatedStrategy(
    ShoplyDbContext context,
    ILogger<RefundCreatedStrategy> logger) : IStripeEventStrategy
{
    public string EventType => EventTypes.RefundCreated;

    public Task HandleNotification(
        StripeEventObjectV1 stripEventObjectV1,
        Order order,
        CancellationToken cancellationToken) => throw new NotImplementedException();
}