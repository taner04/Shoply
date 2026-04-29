using System.Net;
using Shoply.WebApi.Features.Orders.Enums;

namespace Shoply.WebApi.Features.Orders.Exceptions;

public sealed class InvalidOrderPaymentStatusException(OrderStatus currentStatus, OrderStatus attemptedStatus)
    : ShoplyException(
        $"Cannot transition from {currentStatus} to {attemptedStatus}.",
        $"The payment status transition from '{currentStatus}' to '{attemptedStatus}' is not allowed.",
        "Order.PaymentStatus.Invalid",
        HttpStatusCode.BadRequest);