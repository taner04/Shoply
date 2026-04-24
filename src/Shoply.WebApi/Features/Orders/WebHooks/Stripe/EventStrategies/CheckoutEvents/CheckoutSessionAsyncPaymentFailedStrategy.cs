using Shoply.WebApi.Common.Attributes;
using Stripe;
using Stripe.Checkout;

namespace Shoply.WebApi.Features.Orders.WebHooks.Stripe.EventStrategies.CheckoutEvents;

[ServiceInjection<CheckoutSessionAsyncPaymentFailedStrategy, IStripeEventStrategy>(ServiceLifetime.Scoped)]
public sealed partial class CheckoutSessionAsyncPaymentFailedStrategy(
    ShoplyDbContext context,
    ILogger<CheckoutSessionAsyncPaymentFailedStrategy> logger) : IStripeEventStrategy
{
    public string EventType => EventTypes.CheckoutSessionAsyncPaymentFailed;
    public Task HandleNotification(IHasMetadata metadata, Order order, CancellationToken cancellationToken) => throw new NotImplementedException();
}