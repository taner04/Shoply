using Shoply.WebApi.Common.Attributes;
using Shoply.WebApi.Common.Infrastructure.Services.Emails;
using Stripe;

namespace Shoply.WebApi.Features.Orders.WebHooks.Stripe.EventStrategies.CheckoutEvents;

[ServiceInjection<CheckoutSessionCompletedStrategy, IStripeEventStrategy>(ServiceLifetime.Scoped)]
public sealed partial class CheckoutSessionCompletedStrategy(
    IEmailService emailService,
    ILogger<CheckoutSessionCompletedStrategy> logger)
    : IStripeEventStrategy
{
    public string EventType => EventTypes.CheckoutSessionCompleted;
    public Task HandleNotification(IHasMetadata metadata, Order order, CancellationToken cancellationToken) => throw new NotImplementedException();
}
