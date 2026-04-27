using Shoply.WebApi.Common.Attributes;
using Shoply.WebApi.Features.WebHooks.Endpoints.Stripe.EventObjects.V1;
using Stripe;

namespace Shoply.WebApi.Features.WebHooks.Endpoints.Stripe.EventStrategies.CheckoutEvents;

[ServiceInjection<CheckoutSessionAsyncPaymentSucceededStrategy, IStripeEventStrategy>(ServiceLifetime.Scoped)]
public sealed class CheckoutSessionAsyncPaymentSucceededStrategy(
    ShoplyDbContext context,
    ILogger<CheckoutSessionAsyncPaymentSucceededStrategy> logger) : IStripeEventStrategy
{
    public string EventType => EventTypes.CheckoutSessionAsyncPaymentSucceeded;

    public Task HandleNotification(
        StripeEventObjectV1 stripEventObjectV1,
        Order order,
        CancellationToken cancellationToken) => throw new NotImplementedException();
}