using Api.Common.Shared.Guards;

namespace Api.Common.Domain.Orders;

[ValueObject<Guid>]
public readonly partial struct OrderId;

public sealed class Order : Aggregate<OrderId>
{
    private readonly List<OrderItem> _orderItems = [];

    private Order()
    {
    } // For EF Core

    private Order(UserId userId, List<OrderItem> orderItems)
    {
        Id = OrderId.From(Guid.CreateVersion7());
        UserId = userId;
        _orderItems = orderItems;
    }

    public UserId UserId { get; private set; }
    public IReadOnlyCollection<OrderItem> OrderItems => _orderItems.AsReadOnly();
    public User User { get; private set; } = null!;

    public static Order Create(UserId userId, List<OrderItem> orderItems)
    {
        Guard.Against.EmptyCollection(orderItems);
        return new Order(userId, orderItems);
    }
}