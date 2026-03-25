using System.Net;

namespace Api.Features.Orders.Exceptions;

public sealed class OrderBasketEmptyException() : ApiException(
    "Order basket is empty.",
    "Cannot create an order with an empty basket.",
    "Order.Basket.Empty",
    HttpStatusCode.BadRequest);