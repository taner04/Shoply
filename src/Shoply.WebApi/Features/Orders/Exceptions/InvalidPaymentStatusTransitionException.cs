using System.Net;

namespace Shoply.WebApi.Features.Orders.Exceptions;

public sealed class InvalidPaymentStatusTransitionException(PaymentStatus currentStatus, PaymentStatus attemptedStatus)
    : ShoplyException(
        $"Cannot transition from {currentStatus} to {attemptedStatus}.",
        $"The payment status transition from '{currentStatus}' to '{attemptedStatus}' is not allowed.",
        "Payment.Status.Invalid",
        HttpStatusCode.BadRequest);