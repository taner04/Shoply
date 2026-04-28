using System.Net;

namespace Shoply.WebApi.Features.Orders.Exceptions;

public sealed class PaymentIntentIdAlreadySetException()
    : ShoplyException(
        "Stripe Payment Intent ID is already set.",
        "The payment intent ID cannot be changed once it has been set.",
        "Payment.IntentId.AlreadySet",
        HttpStatusCode.BadRequest);