using Shoply.WebApi.Common.Attributes;
using Stripe;

namespace Shoply.WebApi.Features.Orders.WebHooks.Stripe.EventStrategies.RefundEvents;

[ServiceInjection<RefundFailedStrategy, IStripeEventStrategy>(ServiceLifetime.Scoped)]
public sealed partial class RefundFailedStrategy(
    ShoplyDbContext context,
    ILogger<RefundFailedStrategy> logger) : IStripeEventStrategy
{
    public string EventType => EventTypes.RefundFailed;
    public Task HandleNotification(IHasMetadata metadata, Order order, CancellationToken cancellationToken) => throw new NotImplementedException();
}