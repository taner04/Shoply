using Shoply.WebApi.Common.Attributes;
using Stripe;

namespace Shoply.WebApi.Features.Orders.WebHooks.Stripe.EventStrategies.CheckoutEvents;

[ServiceInjection<CheckoutSessionAsyncPaymentSucceededStrategy, IStripeEventStrategy>(ServiceLifetime.Scoped)]
public sealed partial class CheckoutSessionAsyncPaymentSucceededStrategy(
    ShoplyDbContext context,
    ILogger<CheckoutSessionAsyncPaymentSucceededStrategy> logger) : IStripeEventStrategy
{
    public string EventType => EventTypes.CheckoutSessionAsyncPaymentSucceeded;
    public Task HandleNotification(IHasMetadata metadata, Order order, CancellationToken cancellationToken) => throw new NotImplementedException();
}