using Api.Common.Domain.Baskets.Exceptions;
using Api.Common.Domain.Products;
using Api.Common.Domain.Users;
using Api.Common.Shared.Exceptions;
using Vogen;

namespace Api.Common.Domain.Baskets;

[ValueObject]
public readonly partial struct BasketId;

public sealed class Basket : AggregateRoot<BasketId>
{
    private readonly List<BasketItem> _basketItems = [];

    private Basket() { } // For EF Core

    private Basket(UserId userId)
    {
        Id = BasketId.From(Guid.CreateVersion7());
        UserId = userId;
    }

    public static Basket CreateEmpty(UserId userId)
    {
        return new Basket(userId);
    }

    public UserId UserId { get; private set; }
    public IReadOnlyCollection<BasketItem> BasketItems => _basketItems.AsReadOnly();
    public User User { get; private set; } = null!;
    
    public void AddProduct(BasketItem basketItem)
    {
        var existing = _basketItems.FirstOrDefault(p => p.ProductId == basketItem.ProductId);
        if (existing is null)
        {
            _basketItems.Add(basketItem);
        }
        else
        {
            existing.IncreaseQuantity();
        }
    }

    public void RemoveProduct(ProductId productId)
    {
        var existing = _basketItems.FirstOrDefault(p => p.ProductId == productId);
        if (existing is null)
        {
            throw new EntityNotFoundException<BasketItem>(productId);
        }

        if (existing.Quantity == 1)
        {
            // Remove item entirely when quantity reaches minimum
            _basketItems.Remove(existing);
        }
        else
        {
            // Decrease quantity when there's more than one
            existing.DecreaseQuantity();
        }
    }
}