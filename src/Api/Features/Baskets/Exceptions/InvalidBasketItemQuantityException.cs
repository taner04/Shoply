using System.Net;

namespace Api.Features.Baskets.Exceptions;

public sealed class InvalidBasketItemQuantityException()
    : ApiException("Invalid Basket Item Quantity",
        "The basket item quantity cannot be decreased below 1. Please remove the item from the basket instead.",
        "BasketItem.Quantity.Invalid",
        HttpStatusCode.BadRequest);