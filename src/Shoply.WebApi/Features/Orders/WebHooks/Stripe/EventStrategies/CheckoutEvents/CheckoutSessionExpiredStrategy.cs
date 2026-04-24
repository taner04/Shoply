using Shoply.WebApi.Common.Attributes;
using Stripe;

namespace Shoply.WebApi.Features.Orders.WebHooks.Stripe.EventStrategies.CheckoutEvents;

[ServiceInjection<CheckoutSessionExpiredStrategy, IStripeEventStrategy>(ServiceLifetime.Scoped)]
public sealed partial class CheckoutSessionExpiredStrategy(
    ShoplyDbContext context,
    ILogger<CheckoutSessionExpiredStrategy> logger) : IStripeEventStrategy
{
    public string EventType => EventTypes.CheckoutSessionExpired;
    public Task HandleNotification(IHasMetadata metadata, Order order, CancellationToken cancellationToken) => throw new NotImplementedException();
}