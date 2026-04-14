using Stripe.Checkout;

namespace Shoply.WebApi.Features.Orders.WebHooks.Stripe;

public static class StripeExtensions
{
    extension(Session session)
    {
        public bool TryExtractOrderAndUserId(out OrderId orderId, out UserId userId)
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
}