using Shoply.WebApi.Common.Attributes;
using Shoply.WebApi.Common.Infrastructure.Services.Emails;
using Shoply.WebApi.Features.WebHooks.Endpoints.Stripe.EventObjects.V1;
using Stripe;

namespace Shoply.WebApi.Features.WebHooks.Endpoints.Stripe.EventStrategies.CheckoutEvents;

[ServiceInjection<CheckoutSessionCompletedStrategy, IStripeEventStrategy>(ServiceLifetime.Scoped)]
public sealed class CheckoutSessionCompletedStrategy(
    IEmailService emailService,
    ILogger<CheckoutSessionCompletedStrategy> logger)
    : IStripeEventStrategy
{
    public string EventType => EventTypes.CheckoutSessionCompleted;

    public Task HandleNotification(
        StripeEventObjectV1 stripEventObjectV1,
        Order order,
        CancellationToken cancellationToken) => throw new NotImplementedException();
}