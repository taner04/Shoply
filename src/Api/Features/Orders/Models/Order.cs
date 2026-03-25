using Api.Common.Shared.Guards;
using Api.Common.Shared.Models;
using Api.Features.Orders.Exceptions;
using UserId = Api.Features.Users.Models.UserId;

namespace Api.Features.Orders.Models;

[ValueObject<Guid>]
public readonly partial struct OrderId;

public enum OrderStatus
{
    Pending,
    Paid,
    Failed,
    Cancelled
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
        Status = OrderStatus.Pending;
        Payment = Payment.Create(Id, TotalPrice());
    }

    public UserId UserId { get; private set; }
    public string IdempotencyKey { get; private set; }
    public OrderStatus Status { get; private set; }

    public IReadOnlyCollection<OrderItem> OrderItems => _orderItems.AsReadOnly();
    public User User { get; private set; } = null!;
    
    public Payment Payment { get; private set; }

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
        if (Status != OrderStatus.Pending)
        {
            throw new InvalidOrderPaymentStatusException(Status, OrderStatus.Paid);
        }
        
        Status = OrderStatus.Paid;
    }

    public void MarkFailed()
    {
        if (Status != OrderStatus.Pending)
        {
            throw new InvalidOrderPaymentStatusException(Status, OrderStatus.Failed);
        }
        
        Status = OrderStatus.Failed;
    }
    
    public void MarkCancelled()
    {
        if (Payment?.Status == PaymentStatus.Succeeded)
        {
            throw new InvalidOperationException("Cannot cancel an order that has already been paid.");
        }
        
        Status = OrderStatus.Cancelled;
    }
}