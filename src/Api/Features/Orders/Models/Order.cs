using Api.Common.Shared.Guards;
using Api.Common.Shared.Models;
using Api.Features.Users.Models;
using UserId = Api.Features.Users.Models.UserId;

namespace Api.Features.Orders.Models;

[ValueObject<Guid>]
public readonly partial struct OrderId;

public enum OrderPaymentStatus
{
    Pending,
    Paid,
    Failed
}

public sealed class Order : Entity<OrderId>
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
        IdempotencyKey = $"order_{Id.Value:N}_checkout_v1";
        PaymentStatus = OrderPaymentStatus.Pending;
    }

    public UserId UserId { get; private set; }
    public string IdempotencyKey { get; private set; }
    public OrderPaymentStatus PaymentStatus { get; private set; }

    public IReadOnlyCollection<OrderItem> OrderItems => _orderItems.AsReadOnly();
    public User User { get; private set; } = null!;

    public static Order Create(UserId userId, List<OrderItem> orderItems)
    {
        Guard.Against.EmptyCollection(orderItems);
        return new Order(userId, orderItems);
    }

    public decimal TotalPrice()
    {
        return OrderItems.Sum(orderItem => orderItem.TotalPrice);
    }

    public void MarkPaid()
    {
        PaymentStatus = OrderPaymentStatus.Paid;
    }

    public void MarkFailed()
    {
        PaymentStatus = OrderPaymentStatus.Failed;
    }
}