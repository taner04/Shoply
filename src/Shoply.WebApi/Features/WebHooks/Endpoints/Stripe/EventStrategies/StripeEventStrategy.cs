using Stripe;

namespace Shoply.WebApi.Features.WebHooks.Endpoints.Stripe.EventStrategies;

public abstract class StripeEventStrategy<TEvent>(ShoplyDbContext context) : IStripeEventStrategy
{
    protected ShoplyDbContext Context => context;

    public abstract string EventType { get; }

    public async Task HandleEventAsync(EventData eventData, CancellationToken cancellationToken)
    {
        if (eventData.Object is not TEvent @event)
        {
            throw new InvalidOperationException($"Event data object is not of type {typeof(TEvent).Name}");
        }

        var metadata = GetEventMetadata(eventData);

        var order = await context.Orders
            .Include(o => o.Payment)
            .Include(o => o.User)
            .FirstOrDefaultAsync(o => o.Payment.PaymentIntentId == metadata.PaymentIntentId &&
                                      o.UserId == metadata.UserId &&
                                      o.Id == metadata.OrderId &&
                                      o.IdempotencyKey == metadata.IdempotencyKey, cancellationToken);
        if (order is null)
        {
            return;
        }

        await HandleEventAsync(@event, order, cancellationToken);
    }

    protected abstract Task HandleEventAsync(
        TEvent @event,
        Order order,
        CancellationToken cancellationToken);

    private static EventMetadata GetEventMetadata(EventData eventData)
    {
        if (eventData.Object is not IHasMetadata { } obj)
        {
            throw new InvalidOperationException($"Event data object is not of type {nameof(IHasObject)}");
        }

        if (!obj.Metadata.TryGetValue("orderId", out var orderId) || !OrderId.TryParse(orderId, out var parsedOrderId))
        {
            throw new InvalidOperationException("Event data object metadata does not contain a valid orderId");
        }

        if (!obj.Metadata.TryGetValue("userId", out var userId) || !UserId.TryParse(userId, out var parsedUserId))
        {
            throw new InvalidOperationException("Event data object metadata does not contain a valid userId");
        }

        if (!obj.Metadata.TryGetValue("idempotencyKey", out var idempotencyKey) ||
            !Guid.TryParse(idempotencyKey, out var parsedIdempotencyKey))
        {
            throw new InvalidOperationException("Event data object metadata does not contain a valid idempotencyKey");
        }

        if (!obj.Metadata.TryGetValue("paymentIntentId", out var paymentIntentId))
        {
            throw new InvalidOperationException("Event data object metadata does not contain a valid paymentIntentId");
        }

        return new EventMetadata(parsedOrderId, parsedUserId, paymentIntentId, parsedIdempotencyKey);
    }

    private record EventMetadata(OrderId OrderId, UserId UserId, string PaymentIntentId, Guid IdempotencyKey);
}