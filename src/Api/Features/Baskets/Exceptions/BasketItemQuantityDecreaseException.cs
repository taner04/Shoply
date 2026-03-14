using System.Net;
using Api.Common.Shared.Exceptions;

namespace Api.Features.Baskets.Exceptions;

public sealed class BasketItemQuantityDecreaseException()
    : ApiException("Invalid Basket Item Quantity Decrease",
        "The basket item quantity cannot be decreased below 1. Please remove the item from the basket instead.",
        "BasketItem.QuantityDecrease.Invalid",
        HttpStatusCode.BadRequest);