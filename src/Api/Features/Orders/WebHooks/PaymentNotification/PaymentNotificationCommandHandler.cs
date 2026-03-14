using System.Collections.ObjectModel;
using Api.Common.Infrastructure.Persistence;
using Api.Common.Shared.Exceptions;
using Api.Features.Orders.Models;
using Api.Features.Orders.WebHooks.PaymentNotification.Strategies;
using Mediator;
using Stripe.Checkout;
using OrderId = Api.Features.Orders.Models.OrderId;
using UserId = Api.Features.Users.Models.UserId;

namespace Api.Features.Orders.WebHooks.PaymentNotification;

public sealed class PaymentNotificationCommandHandler(
    ApplicationDbContext context,
    IEnumerable<IPaymentNotificationStrategy> strategies,
    ILogger<PaymentNotificationCommandHandler> logger) : ICommandHandler<PaymentNotificationCommand>
{
    private ReadOnlyDictionary<string, IPaymentNotificationStrategy> StrategiesMap =>
        strategies.ToDictionary(s => s.EventType, s => s).AsReadOnly();

    public async ValueTask<Unit> Handle(PaymentNotificationCommand command, CancellationToken cancellationToken)
    {
        if (!StrategiesMap.TryGetValue(command.StripeEvent.Type, out var strategy))
        {
            logger.LogWarning("No strategy found for Stripe event type: {EventType}", command.StripeEvent.Type);
            return Unit.Value;
        }

        var session = (Session)command.StripeEvent.Data.Object;
        if (!TryExtractOrderAndUserId(session, out var orderId, out var userId))
        {
            logger.LogWarning("Stripe event {eventId} missing OrderId or UserId in metadata", command.StripeEvent.Id);
            throw new InvalidOperationException(
                $"Stripe event {command.StripeEvent.Id} missing OrderId or UserId in metadata");
        }

        logger.LogInformation("Processing Stripe event {eventId} for order {orderId} and user {userId}",
            command.StripeEvent.Id, orderId, userId);
        var order = await context.Orders.FirstOrDefaultAsync(o => o.Id == orderId, cancellationToken) ??
                    throw new EntityNotFoundException<Order>(orderId);
        await strategy.HandleNotification(command.StripeEvent, order, cancellationToken);

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
}