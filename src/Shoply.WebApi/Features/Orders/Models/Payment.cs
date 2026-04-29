using Shoply.WebApi.Common.Shared.Guards;
using Shoply.WebApi.Common.Shared.Models;
using Shoply.WebApi.Features.Orders.Enums;
using Shoply.WebApi.Features.Orders.Exceptions;

namespace Shoply.WebApi.Features.Orders.Models;

[ValueObject<Guid>]
public readonly partial struct PaymentId
{
    private static Validation Validate(Guid value)
        => value != Guid.Empty ? Validation.Ok : Validation.Invalid("PaymentId must set to non-default value.");
}

public sealed class Payment : Entity<PaymentId>
{
    private Payment()
    {
    } // For EF Core

    private Payment(OrderId orderId, decimal amount)
    {
        Id = PaymentId.From(Guid.CreateVersion7());
        OrderId = orderId;
        Amount = amount;
        PaymentIntentId = null!; // Will be set later when creating the payment intent with Stripe
        Status = PaymentStatus.Pending;
    }

    public OrderId OrderId { get; private set; }
    public Order Order { get; private set; } = null!;

    public decimal Amount { get; }
    public string PaymentIntentId { get; private set; }

    public PaymentStatus Status { get; private set; }

    public decimal RefundedAmount { get; private set; }

    public static Payment Create(OrderId orderId, decimal amount)
    {
        Guard.Against.NegativeOrZero<Payment>(amount);

        return new Payment(orderId, amount);
    }

    public void SetStripePaymentIntentId(string paymentIntentId)
    {
        if (PaymentIntentId != null)
        {
            throw new PaymentIntentIdAlreadySetException();
        }

        Guard.Against.NullOrEmpty<Payment>(paymentIntentId);
        PaymentIntentId = paymentIntentId;
    }
    
    public void MarkPaid()
    {
        if (Status is not PaymentStatus.Pending)
        {
            throw new InvalidPaymentStatusTransitionException(Status, PaymentStatus.Paid);
        }

        Status = PaymentStatus.Paid;
    }
    
    public void MarkCanceled()
    {
        if(Status is not PaymentStatus.Paid)
        {
            throw new InvalidPaymentStatusTransitionException(Status, PaymentStatus.Canceled);
        }
        
        Status = PaymentStatus.Canceled;
    }

    public void MarkRefunded(decimal refundAmount)
    {
        Guard.Against.NegativeOrZero<Payment>(refundAmount);

        if (Status is not (PaymentStatus.Paid or PaymentStatus.PartiallyRefunded))
        {
            throw new InvalidPaymentStatusTransitionException(Status, PaymentStatus.Refunded);
        }

        var availableBalance = Amount - RefundedAmount;
        if (refundAmount > availableBalance)
        {
            throw new PaymentRefundExceedsBalanceException(refundAmount, availableBalance);
        }

        RefundedAmount += refundAmount;
        Status = RefundedAmount == Amount ? PaymentStatus.Refunded : PaymentStatus.PartiallyRefunded;
    }

    public void MarkFailed()
    {
        if(Status is not (PaymentStatus.Refunded or PaymentStatus.PartiallyRefunded))
        {
            throw new InvalidPaymentStatusTransitionException(Status, PaymentStatus.Failed);
        }
        
        Status = PaymentStatus.Failed;
    }
}