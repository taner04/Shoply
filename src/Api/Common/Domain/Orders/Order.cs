using Api.Common.Domain.Baskets;
using Api.Common.Domain.Users;
using Vogen;

namespace Api.Common.Domain.Orders;

[ValueObject]
public readonly partial struct OrderId;

public sealed class Order : AggregateRoot<OrderId>
{
    private readonly List<OrderItem> _orderItems = [];
    
    public UserId UserId { get; private set; }
    public IReadOnlyCollection<OrderItem> OrderItems => _orderItems.AsReadOnly();
    public User User { get; private set; } = null!;
    
    private Order() { } // For EF Core

    private Order(UserId userId, List<OrderItem> orderItems)
    {
        Id = OrderId.From(Guid.CreateVersion7());
        UserId = userId;
        _orderItems = orderItems;
    }
    
    public static Order FromBasket(Basket basket)
    {
        var orderItems = basket.BasketItems
            .Select(bi => bi.Product is null ? throw new InvalidOperationException("BasketItem Product must be eagerly loaded before creating order.") : OrderItem.From(bi.Product, bi.Quantity))
            .ToList();

        return new Order(basket.UserId, orderItems);
    }
}