using Shoply.WebApi.Common.Attributes;
using Shoply.WebApi.Features.WebHooks.Endpoints.Stripe.EventObjects.V1;
using Stripe;

namespace Shoply.WebApi.Features.WebHooks.Endpoints.Stripe.EventStrategies.RefundEvents;

[ServiceInjection<RefundFailedStrategy, IStripeEventStrategy>(ServiceLifetime.Scoped)]
public sealed class RefundFailedStrategy(
    ShoplyDbContext context,
    ILogger<RefundFailedStrategy> logger) : IStripeEventStrategy
{
    public string EventType => EventTypes.RefundFailed;

    public Task HandleNotification(
        StripeEventObjectV1 stripEventObjectV1,
        Order order,
        CancellationToken cancellationToken) => throw new NotImplementedException();
}