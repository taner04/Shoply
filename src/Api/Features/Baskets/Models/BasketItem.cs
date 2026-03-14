using Api.Features.Baskets.Exceptions;
using Api.Features.Products.Models;
using ProductId = Api.Features.Products.Models.ProductId;

namespace Api.Features.Baskets.Models;

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