using System.Net;

namespace Shoply.WebApi.Features.Orders.Exceptions;

public sealed class PaymentRefundExceedsBalanceException(decimal requestedAmount, decimal availableBalance)
    : ShoplyException(
        "Refund amount exceeds available balance.",
        $"Cannot refund {requestedAmount}. Only {availableBalance} available for refund.",
        "Payment.Refund.ExceedsBalance",
        HttpStatusCode.BadRequest);