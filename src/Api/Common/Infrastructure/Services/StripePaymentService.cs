using Api.Common.Attributes;
using Api.Common.Composition.Options;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;
using OrderId = Api.Features.Orders.Models.OrderId;

namespace Api.Common.Infrastructure.Services;

public readonly record struct RefundParameters(
    long TotalAmountInCents,
    string PaymentIntentId,
    string Reason);

public readonly record struct CheckoutSessionParameters(Order Order);

[ServiceInjection(ServiceLifetime.Scoped)]
public sealed partial class StripePaymentProvider(
    IOptions<StripeConfig> stripeConfigOptions,
    ILogger<StripePaymentProvider> logger,
    SessionService sessionService,
    RefundService refundService)
{
    private readonly StripeConfig _stripeConfigOptions = stripeConfigOptions.Value;

    public async Task<string> CreateCheckoutSessionAsync(
        Order order,
        string userEmail,
        CancellationToken cancellationToken)
    {
        var sessionLineItemOptions = new List<SessionLineItemOptions>();

        foreach (var orderItem in order.OrderItems)
        {
            sessionLineItemOptions.Add(new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    UnitAmountDecimal = orderItem.UnitPrice * 100, // Convert to cents
                    Currency = "eur",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = orderItem.ProductName,
                        Description = orderItem.ProductDescription
                    }
                },
                Quantity = orderItem.Quantity
            });
        }

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
            PaymentIntentData = new SessionPaymentIntentDataOptions
            {
                Metadata = new Dictionary<string, string>
                {
                    ["OrderId"] = order.Id.ToString(),
                    ["UserId"] = order.UserId.ToString()
                }
            },
            Metadata = new Dictionary<string, string>
            {
                ["OrderId"] = order.Id.ToString(),
                ["UserId"] = order.UserId.ToString()
            },
            LineItems = sessionLineItemOptions
        };

        var requestOptions = new RequestOptions
        {
            IdempotencyKey = order.IdempotencyKey
        };

        try
        {
            var session = await sessionService.CreateAsync(options, requestOptions, cancellationToken);
            LogCreateSessionSuccess(order.Id, order.TotalPrice());
            return session.Url ?? throw new InvalidOperationException("Stripe session URL missing.");
        }
        catch (Exception ex)
        {
            LogCreateSessionException(order.Id, ex);
            throw;
        }
    }

    public async Task<bool> RefundPaymentAsync(
        RefundParameters parameters,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(parameters.PaymentIntentId))
        {
            throw new ArgumentException("PaymentIntentId is required.", nameof(parameters));
        }

        if (parameters.TotalAmountInCents <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(parameters), "Refund amount must be > 0.");
        }

        var refundOptions = new RefundCreateOptions
        {
            PaymentIntent = parameters.PaymentIntentId,
            Amount = parameters.TotalAmountInCents,
            Reason = parameters.Reason,

            Metadata = new Dictionary<string, string>
            {
                ["PaymentIntentId"] = parameters.PaymentIntentId
            }
        };

        try
        {
            var refund = await refundService.CreateAsync(refundOptions, cancellationToken: cancellationToken);
            LogRefundSuccess(parameters.PaymentIntentId, parameters.TotalAmountInCents);

            return true;
        }
        catch (Exception ex)
        {
            LogRefundException(parameters.PaymentIntentId, parameters.TotalAmountInCents, ex);
            logger.LogError(ex, "Failed to refund Stripe payment.");
            throw;
        }
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