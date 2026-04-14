using Shoply.WebApi.Features.Baskets.Exceptions;

namespace Shoply.WebApi.Features.Baskets.Models;

public sealed record BasketItem(ProductId ProductId, int Quantity = 1)
{
    public Product Product { get; private set; } = null!;

    public BasketItem IncreaseQuantity() => this with { Quantity = Quantity + 1 };

    public BasketItem DecreaseQuantity()
    {
        if (Quantity <= 1)
        {
            throw new InvalidBasketItemQuantityException();
        }

        return this with { Quantity = Quantity - 1 };
    }
}