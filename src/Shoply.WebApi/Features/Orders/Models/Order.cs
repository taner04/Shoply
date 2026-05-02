using Shoply.WebApi.Common.Shared.Guards;
using Shoply.WebApi.Common.Shared.Models;
using Shoply.WebApi.Features.Orders.Enums;

namespace Shoply.WebApi.Features.Orders.Models;

[ValueObject<Guid>]
public readonly partial struct OrderId
{
    private static Validation Validate(Guid value)
    {
        return value != Guid.Empty ? Validation.Ok : Validation.Invalid("OrderId must set to non-default value.");
    }
}

public sealed class Order : Entity<OrderId>
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private Order()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {
    } // For EF Core

    public Order(UserId userId, List<OrderItem> orderItems)
    {
        Guard.Against.EmptyCollection(orderItems);

        Id = OrderId.From(Guid.CreateVersion7());
        UserId = userId;
        OrderItems = orderItems;
        IdempotencyKey = Guid.CreateVersion7();
        Status = OrderStatus.Pending;
        Payment = new Payment(Id, OrderItems.Sum(orderItem => orderItem.TotalPrice));
    }

    public UserId UserId { get; init; }
    public Guid IdempotencyKey { get; init; }
    public OrderStatus Status { get; set; }

    public ICollection<OrderItem> OrderItems { get; init; } = [];
    public User User { get; private set; } = null!;

    public Payment Payment { get; }
}