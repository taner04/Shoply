using Shoply.WebApi.Common.Attributes;
using Shoply.WebApi.Features.WebHooks.Endpoints.Stripe.EventObjects.V1;
using Stripe;

namespace Shoply.WebApi.Features.WebHooks.Endpoints.Stripe.EventStrategies.CheckoutEvents;

[ServiceInjection<CheckoutSessionAsyncPaymentFailedStrategy, IStripeEventStrategy>(ServiceLifetime.Scoped)]
public sealed class CheckoutSessionAsyncPaymentFailedStrategy(
    ShoplyDbContext context,
    ILogger<CheckoutSessionAsyncPaymentFailedStrategy> logger) : IStripeEventStrategy
{
    public string EventType => EventTypes.CheckoutSessionAsyncPaymentFailed;

    public Task HandleNotification(
        StripeEventObjectV1 stripEventObjectV1,
        Order order,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Checkout session completed");
        return Task.CompletedTask;
    }
}