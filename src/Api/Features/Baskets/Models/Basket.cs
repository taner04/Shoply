using Api.Common.Shared.Models;
using ProductId = Api.Features.Products.Models.ProductId;

namespace Api.Features.Baskets.Models;

[ValueObject<Guid>]
public readonly partial struct BasketId;

public sealed class Basket : Entity<BasketId>
{
    private readonly List<BasketItem> _basketItems = [];

    private Basket()
    {
    } // For EF Core

    public IReadOnlyCollection<BasketItem> BasketItems => _basketItems.AsReadOnly();

    public static Basket CreateEmpty()
    {
        return new Basket { Id = BasketId.From(Guid.CreateVersion7()) };
    }

    public void AddProduct(Product product)
    {
        var existingIndex = _basketItems.FindIndex(p => p.ProductId == product.Id);
        if (existingIndex == -1)
        {
            _basketItems.Add(product.ToBasketItem());
        }
        else
        {
            var existing = _basketItems[existingIndex];
            _basketItems[existingIndex] = existing.IncreaseQuantity();
        }
    }

    public void RemoveProduct(ProductId productId)
    {
        var existingIndex = _basketItems.FindIndex(p => p.ProductId == productId);
        if (existingIndex == -1)
        {
            throw new EntityNotFoundException<BasketItem>(productId);
        }

        var existing = _basketItems[existingIndex];
        if (existing.Quantity == 1)
        {
            _basketItems.RemoveAt(existingIndex);
        }
        else
        {
            _basketItems[existingIndex] = existing.DecreaseQuantity();
        }
    }

    public void EmptyBasket()
    {
        _basketItems.Clear();
    }

    public bool IsEmpty()
    {
        return _basketItems.Count == 0;
    }
}