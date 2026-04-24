using Shoply.WebApi.Common.Attributes;
using Stripe;

namespace Shoply.WebApi.Features.Orders.WebHooks.Stripe.EventStrategies.RefundEvents;

[ServiceInjection<RefundUpdatedStrategy, IStripeEventStrategy>(ServiceLifetime.Scoped)]
public sealed partial class RefundUpdatedStrategy(
    ShoplyDbContext context,
    ILogger<RefundUpdatedStrategy> logger) : IStripeEventStrategy
{
    public string EventType => EventTypes.RefundUpdated;
    public Task HandleNotification(IHasMetadata metadata, Order order, CancellationToken cancellationToken) => throw new NotImplementedException();
}