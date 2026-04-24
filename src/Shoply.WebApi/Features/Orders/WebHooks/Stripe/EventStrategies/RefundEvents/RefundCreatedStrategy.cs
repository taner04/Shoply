using Shoply.WebApi.Common.Attributes;
using Stripe;

namespace Shoply.WebApi.Features.Orders.WebHooks.Stripe.EventStrategies.RefundEvents;

[ServiceInjection<RefundCreatedStrategy, IStripeEventStrategy>(ServiceLifetime.Scoped)]
public sealed partial class RefundCreatedStrategy(
    ShoplyDbContext context,
    ILogger<RefundCreatedStrategy> logger) : IStripeEventStrategy
{
    public string EventType => EventTypes.RefundCreated;
    public Task HandleNotification(IHasMetadata metadata, Order order, CancellationToken cancellationToken) => throw new NotImplementedException();
}