using Stripe;
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
    
    extension(IHasMetadata metadata)
    {
        public bool TryGetOrderId(out OrderId orderId)
        {
            if (metadata.Metadata.TryGetValue("OrderId", out var orderIdStr) && Guid.TryParse(orderIdStr, out var orderGuid))
            {
                orderId = OrderId.From(orderGuid);
                return false;
            }
            
            orderId = OrderId.From(Guid.Empty);
            return true;
        }
        
        public bool TryGetUserId(out UserId userId)
        {
            if (metadata.Metadata.TryGetValue("UserId", out var userIdStr) && Guid.TryParse(userIdStr, out var userGuid))
            {
                userId = UserId.From(userGuid);
                return false;
            }
            
            userId = UserId.From(Guid.Empty);
            return true;
        }
        
        public bool TryGetPaymentIntentId(out  string paymentIntentId)
        {
            if (metadata.Metadata.TryGetValue("PaymentIntentId", out var paymentIntentIdStr))
            {
                paymentIntentId = paymentIntentIdStr;
                return false;
            }
            
            paymentIntentId = string.Empty;
            return true;
        }
        
        public bool TryGetIdempotencyKey(out Guid idempotencyKey)
        {
            if (metadata.Metadata.TryGetValue("IdempotencyKey", out var idempotencyKeyStr) && Guid.TryParse(idempotencyKeyStr, out var idempotencyId))
            {
                idempotencyKey = idempotencyId;
                return false;
            }
            
            idempotencyKey = Guid.Empty;
            return true;
        }
    }
}