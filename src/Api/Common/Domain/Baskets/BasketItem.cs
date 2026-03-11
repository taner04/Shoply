using Api.Common.Domain.Baskets.Exceptions;

namespace Api.Common.Domain.Baskets;

[ValueObject<Guid>]
public readonly partial struct BasketItemId;

public sealed class BasketItem : Entity<BasketItemId>
{
    public BasketItem(ProductId productId, BasketId basketId)
    {
        Id = BasketItemId.From(Guid.CreateVersion7());
        ProductId = productId;
        BasketId = basketId;
        Quantity = 1;
    }

    public BasketId BasketId { get; internal set; }
    public ProductId ProductId { get; private set; }
    public int Quantity { get; private set; }

    public Product Product { get; private set; } = null!;
    
    public void IncreaseQuantity()
    {
        Quantity++;
    }

    public void DecreaseQuantity()
    {
        if (Quantity <= 1)
        {
            throw new BasketItemQuantityDecreaseException();
        }

        Quantity--;
    }
}