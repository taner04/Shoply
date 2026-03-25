using System.Net;

namespace Api.Features.Orders.Exceptions;

public sealed class InvalidOrderPaymentStatusException(OrderStatus currentStatus, OrderStatus attemptedStatus) 
    : ApiException(
        $"Cannot transition from {currentStatus} to {attemptedStatus}.",
        $"The payment status transition from '{currentStatus}' to '{attemptedStatus}' is not allowed.",
        "Order.PaymentStatus.Invalid",
        HttpStatusCode.BadRequest);
