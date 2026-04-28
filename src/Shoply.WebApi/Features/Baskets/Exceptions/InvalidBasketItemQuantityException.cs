using System.Net;

namespace Shoply.WebApi.Features.Baskets.Exceptions;

public sealed class InvalidBasketItemQuantityException()
    : ShoplyException("Invalid Basket Item Quantity",
        "The basket item quantity cannot be decreased below 1. Please remove the item from the basket instead.",
        "BasketItem.Quantity.Invalid",
        HttpStatusCode.BadRequest);