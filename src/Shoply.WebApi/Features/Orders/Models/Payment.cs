using Shoply.WebApi.Common.Shared.Guards;
using Shoply.WebApi.Common.Shared.Models;
using Shoply.WebApi.Features.Orders.Enums;

namespace Shoply.WebApi.Features.Orders.Models;

[ValueObject<Guid>]
public readonly partial struct PaymentId
{
    private static Validation Validate(Guid value)
    {
        return value != Guid.Empty ? Validation.Ok : Validation.Invalid("PaymentId must set to non-default value.");
    }
}

public sealed class Payment : Entity<PaymentId>
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private Payment()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {
    } // For EF Core

    public Payment(OrderId orderId, decimal amount)
    {
        Guard.Against.NegativeOrZero<Payment>(amount);

        Id = PaymentId.From(Guid.CreateVersion7());
        OrderId = orderId;
        Amount = amount;
        Status = PaymentStatus.Pending;
        PaymentIntentId = null!; // Will be set later when creating the payment intent with Stripe
    }

    public OrderId OrderId { get; init; }
    public Order Order { get; init; } = null!;

    public decimal Amount { get; }
    public string PaymentIntentId { get; set; }

    public PaymentStatus Status { get; set; }

    public decimal RefundedAmount { get; set; }
}