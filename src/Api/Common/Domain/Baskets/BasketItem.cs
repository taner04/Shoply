using Api.Common.Domain.Baskets.Exceptions;
using Api.Common.Domain.Products;
using Vogen;

namespace Api.Common.Domain.Baskets;

[ValueObject]
public readonly partial struct BasketItemId;

public sealed class BasketItem : Entity<BasketItemId>
{
    private BasketItem(ProductId productId)
    {
        Id = BasketItemId.From(Guid.CreateVersion7());
        ProductId = productId;
        Quantity = 1;
    }

    public ProductId ProductId { get; private set; }
    public int Quantity { get; private set; }

    public Product Product { get; private set; } = null!;
    
    public static BasketItem From(Product product) => new(product.Id);

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