using System.Collections.ObjectModel;
using Api.Common.Composition.Options;
using Api.Features.Orders.WebHooks.Stripe.EventStrategies;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;

namespace Api.Features.Orders.WebHooks.Stripe;

public sealed partial class StripeEventCommandHandler(
    IHttpContextAccessor httpContextAccessor,
    IOptions<StripeConfig> stripeConfig,
    ILogger<StripeEventCommandHandler> logger, 
    ApplicationDbContext context, 
    IEnumerable<IStripeEventStrategy> strategies) : ICommandHandler<StripeEventCommand>
{
    private ReadOnlyDictionary<string, IStripeEventStrategy> StrategiesMap => strategies.ToDictionary(s => s.EventType, s => s).AsReadOnly();
    
    public async ValueTask<Unit> Handle(StripeEventCommand _, CancellationToken cancellationToken)
    {
        var httpRequest = httpContextAccessor.HttpContext!.Request;
        var json = await new StreamReader(httpRequest.Body).ReadToEndAsync(cancellationToken);
        
        var stripeEvent = EventUtility.ConstructEvent(
            json,
            httpRequest.Headers["Stripe-Signature"],
            stripeConfig.Value.WebhookSecret
        );
        
        if (!StrategiesMap.TryGetValue(stripeEvent.Type, out var strategy))
        {
            LogNoStrategyFoundForStripeEvent(logger, stripeEvent.Type);
        }
        else
        {
            var session = (Session)stripeEvent.Data.Object;
            if (!TryExtractOrderAndUserId(session, out var orderId, out var userId))
            {
                LogStripeEventEventIdMissing(logger, stripeEvent.Id);
                throw new InvalidOperationException(
                    $"Stripe event {stripeEvent.Id} missing OrderId or UserId in metadata");
            }

            LogProcessingStripeEvent(logger, stripeEvent.Id, orderId, userId);
            var order = await context.Orders
                .Include(o => o.Payment)
                .FirstOrDefaultAsync(o => o.Id == orderId, cancellationToken) 
                ?? throw new EntityNotFoundException<Order>(orderId);
        
            await strategy.HandleNotification(stripeEvent, order, cancellationToken);
        }
        
        return Unit.Value;
    }
    
    private static bool TryExtractOrderAndUserId(Session session, out OrderId orderId, out UserId userId)
    {
        if (session.Metadata == null ||
            !session.Metadata.TryGetValue("OrderId", out var orderIdStr) ||
            !session.Metadata.TryGetValue("UserId", out var userIdStr) ||
            !Guid.TryParse(orderIdStr, out var orderGuid) ||
            !Guid.TryParse(userIdStr, out var userGuid))
        {
            orderId = OrderId.From(Guid.Empty);
            userId = UserId.From(Guid.Empty);
            return false;
        }

        orderId = OrderId.From(orderGuid);
        userId = UserId.From(userGuid);
        return true;
    }
    
    [LoggerMessage(LogLevel.Warning, "No strategy found for Stripe event type: {EventType}")]
    static partial void LogNoStrategyFoundForStripeEvent(ILogger<StripeEventCommandHandler> logger, string EventType);

    [LoggerMessage(LogLevel.Warning, "Stripe event {eventId} missing OrderId or UserId in metadata")]
    static partial void LogStripeEventEventIdMissing(ILogger<StripeEventCommandHandler> logger, string eventId);

    [LoggerMessage(LogLevel.Information, "Processing Stripe event {eventId} for order {orderId} and user {userId}")]
    static partial void LogProcessingStripeEvent(ILogger<StripeEventCommandHandler> logger, string eventId, OrderId orderId, UserId userId);
}