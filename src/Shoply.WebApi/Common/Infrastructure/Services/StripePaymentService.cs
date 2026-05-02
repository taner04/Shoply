using Microsoft.Extensions.Options;
using Shoply.WebApi.Common.Composition.Options;
using Stripe;
using Stripe.Checkout;

namespace Shoply.WebApi.Common.Infrastructure.Services;

public sealed partial class StripePaymentProvider(
    IOptions<StripeConfig> stripeConfigOptions,
    ILogger<StripePaymentProvider> logger,
    SessionService sessionService,
    RefundService refundService)
{
    private readonly StripeConfig _stripeConfigOptions = stripeConfigOptions.Value;

    public async Task<Session> CreateCheckoutSessionAsync(
        Order order,
        string userEmail,
        CancellationToken cancellationToken)
    {
        var sessionLineItemOptions = order.OrderItems.Select(orderItem => new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    UnitAmountDecimal = (long)(orderItem.TotalPrice * 100),
                    Currency = "eur",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = orderItem.ProductName,
                        Description = orderItem.ProductDescription
                    }
                },
                Quantity = orderItem.Quantity
            })
            .ToList();

        var options = new SessionCreateOptions
        {
            PaymentMethodTypes = ["card", "paypal"],
            Mode = "payment",
            SuccessUrl = _stripeConfigOptions.SuccessUrl,
            CancelUrl = _stripeConfigOptions.CancelUrl,
            CustomerEmail = userEmail,
            ExpiresAt = DateTime.UtcNow.AddMinutes(30),
            AllowPromotionCodes = true,
            BillingAddressCollection = "required",
            PhoneNumberCollection = new SessionPhoneNumberCollectionOptions
            {
                Enabled = true
            },
            SubmitType = "pay",
            Locale = "auto",
            PaymentIntentData = new SessionPaymentIntentDataOptions { Metadata = ToMetadata(order) },
            Metadata = ToMetadata(order),
            LineItems = sessionLineItemOptions
        };

        var requestOptions = new RequestOptions
        {
            IdempotencyKey = order.IdempotencyKey.ToString()
        };

        return await sessionService.CreateAsync(options, requestOptions, cancellationToken);
    }

    public async Task RefundOrderAsync(
        Order order,
        CancellationToken cancellationToken)
    {
        var refundOptions = new RefundCreateOptions
        {
            PaymentIntent = order.Payment.PaymentIntentId,
            Amount = (long)(order.Payment.Amount * 100),
            Reason = "Customer cancelled the pending order",
            Metadata = ToMetadata(order)
        };

        await refundService.CreateAsync(refundOptions, cancellationToken: cancellationToken);
    }

    private static Dictionary<string, string> ToMetadata(Order order)
    {
        return new Dictionary<string, string>
        {
            ["OrderId"] = order.Id.Value.ToString(),
            ["UserId"] = order.UserId.Value.ToString(),
            ["StripePaymentIntentId"] = order.Payment.PaymentIntentId,
            ["IdempotencyKey"] = order.IdempotencyKey.ToString()
        };
    }

    [LoggerMessage(0, LogLevel.Information,
        "Successfully created a checkout session for order {orderId} with amount {amount} cents")]
    private partial void LogCreateSessionSuccess(OrderId orderId, decimal amount);

    [LoggerMessage(1, LogLevel.Error,
        "Failed to create a checkout session for order {orderId}")]
    private partial void LogCreateSessionException(OrderId orderId, Exception exception);

    [LoggerMessage(2, LogLevel.Information,
        "Successfully refunded payment for paymentIntentId {paymentIntentId} with amount {amount} cents")]
    private partial void LogRefundSuccess(string paymentIntentId, decimal amount);

    [LoggerMessage(3, LogLevel.Error,
        "Failed to refund payment for paymentIntentId {paymentIntentId} with amount {amount} cents")]
    private partial void LogRefundException(string paymentIntentId, decimal amount, Exception exception);
}