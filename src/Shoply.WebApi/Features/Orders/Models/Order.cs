using Shoply.WebApi.Common.Shared.Guards;
using Shoply.WebApi.Common.Shared.Models;
using Shoply.WebApi.Features.Orders.Exceptions;

namespace Shoply.WebApi.Features.Orders.Models;

[ValueObject<Guid>]
public readonly partial struct OrderId;

public enum OrderStatus
 {
     Pending,
     Cancelled,
     Processing,
     Delivered
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
        IdempotencyKey = Guid.CreateVersion7();
        Status = OrderStatus.Pending;
        Payment = Payment.Create(Id, TotalPrice());
    }

    public UserId UserId { get; init; }
    public Guid IdempotencyKey { get; init; }
    public OrderStatus Status { get; private set; }

    public IReadOnlyCollection<OrderItem> OrderItems => _orderItems.AsReadOnly();
    public User User { get; private set; } = null!;

    public Payment Payment { get; }

    public static Order Create(UserId userId, List<OrderItem> orderItems)
    {
        Guard.Against.EmptyCollection(orderItems);

        return new Order(userId, orderItems);
    }

    public decimal TotalPrice()
    {
        return OrderItems.Sum(orderItem => orderItem.TotalPrice);
    }

    public long TotalAmountInCents() => (long)(TotalPrice() * 100);

    public void SetStripePaymentIntentId(string stripePaymentIntentId)
    {
        Payment.SetStripePaymentIntentId(stripePaymentIntentId);
    }

    public Dictionary<string, string> ToMetadata() =>
        new()
        {
            ["OrderId"] = Id.Value.ToString(),
            ["UserId"] = UserId.Value.ToString(),
            ["StripePaymentIntentId"] = Payment.PaymentIntentId,
            ["IdempotencyKey"] = IdempotencyKey.ToString()
        };

    public void MarkCancelled()
    {
        if(Status != OrderStatus.Delivered)
        {
            throw new InvalidOrderPaymentStatusException(Status, OrderStatus.Cancelled);
        }
        
        Status = OrderStatus.Cancelled;
    }

    public void MarkProcessing()
    {
        if(Status != OrderStatus.Delivered)
        {
            throw new InvalidOrderPaymentStatusException(Status, OrderStatus.Processing);
        }
        
        Status = OrderStatus.Processing;
    }
}