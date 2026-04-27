using Shoply.WebApi.Common.Attributes;
using Shoply.WebApi.Features.WebHooks.Endpoints.Stripe.EventObjects.V1;
using Stripe;

namespace Shoply.WebApi.Features.WebHooks.Endpoints.Stripe.EventStrategies.CheckoutEvents;

[ServiceInjection<CheckoutSessionExpiredStrategy, IStripeEventStrategy>(ServiceLifetime.Scoped)]
public sealed class CheckoutSessionExpiredStrategy(
    ShoplyDbContext context,
    ILogger<CheckoutSessionExpiredStrategy> logger) : IStripeEventStrategy
{
    public string EventType => EventTypes.CheckoutSessionExpired;

    public Task HandleNotification(
        StripeEventObjectV1 stripEventObjectV1,
        Order order,
        CancellationToken cancellationToken) => throw new NotImplementedException();
}