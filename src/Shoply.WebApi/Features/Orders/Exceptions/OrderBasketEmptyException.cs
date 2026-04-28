using System.Net;

namespace Shoply.WebApi.Features.Orders.Exceptions;

public sealed class OrderBasketEmptyException() : ShoplyException(
    "Order basket is empty.",
    "Cannot create an order with an empty basket.",
    "Order.Basket.Empty",
    HttpStatusCode.BadRequest);