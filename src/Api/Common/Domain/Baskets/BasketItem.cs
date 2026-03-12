using Api.Common.Domain.Baskets.Exceptions;

namespace Api.Common.Domain.Baskets;

public sealed record BasketItem(ProductId ProductId, int Quantity = 1)
{
    public Product Product { get; private set; } = null!;

    public BasketItem IncreaseQuantity()
    {
        return this with { Quantity = Quantity + 1 };
    }

    public BasketItem DecreaseQuantity()
    {
        if (Quantity <= 1)
        {
            throw new BasketItemQuantityDecreaseException();
        }

        return this with { Quantity = Quantity - 1 };
    }
}