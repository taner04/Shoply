using System.Net;

namespace Shoply.WebApi.Features.Orders.Exceptions;

public sealed class InvalidPaymentStatusTransitionException(PaymentStatus currentStatus, PaymentStatus attemptedStatus)
    : ApiException(
        $"Cannot transition from {currentStatus} to {attemptedStatus}.",
        $"The payment status transition from '{currentStatus}' to '{attemptedStatus}' is not allowed.",
        "Payment.Status.Invalid",
        HttpStatusCode.BadRequest);

public sealed class PaymentRefundExceedsBalanceException(decimal requestedAmount, decimal availableBalance)
    : ApiException(
        "Refund amount exceeds available balance.",
        $"Cannot refund {requestedAmount}. Only {availableBalance} available for refund.",
        "Payment.Refund.ExceedsBalance",
        HttpStatusCode.BadRequest);

public sealed class PaymentIntentIdAlreadySetException()
    : ApiException(
        "Stripe Payment Intent ID is already set.",
        "The payment intent ID cannot be changed once it has been set.",
        "Payment.IntentId.AlreadySet",
        HttpStatusCode.BadRequest);