using Shoply.WebApi.Common.Shared.Guards;
using Shoply.WebApi.Common.Shared.Models;
using Shoply.WebApi.Features.Orders.Exceptions;

namespace Shoply.WebApi.Features.Orders.Models;

[ValueObject<Guid>]
public readonly partial struct PaymentId;

public enum PaymentStatus
{
    Pending,
    Processing,
    Succeeded,
    Failed,
    Refunded,
    PartiallyRefunded
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
        PaymentIntentId = null; 
        Status = PaymentStatus.Pending;
    }

    public OrderId OrderId { get; private set; }
    public Order Order { get; private set; } = null!;

    public decimal Amount { get; }
    public string PaymentIntentId { get; private set; }

    public PaymentStatus Status { get; private set; }
    public string? FailureReason { get; private set; }

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

    public void MarkProcessing()
    {
        if (Status != PaymentStatus.Pending)
        {
            throw new InvalidPaymentStatusTransitionException(Status, PaymentStatus.Processing);
        }

        Status = PaymentStatus.Processing;
    }

    public void MarkSucceeded()
    {
        if (Status != PaymentStatus.Processing)
        {
            throw new InvalidPaymentStatusTransitionException(Status, PaymentStatus.Succeeded);
        }

        Status = PaymentStatus.Succeeded;
    }

    public void MarkFailed(string reason)
    {
        if (Status is PaymentStatus.Succeeded or PaymentStatus.Refunded)
        {
            throw new InvalidPaymentStatusTransitionException(Status, PaymentStatus.Failed);
        }

        Status = PaymentStatus.Failed;
        FailureReason = reason;
    }

    public void MarkRefunded(decimal refundAmount)
    {
        Guard.Against.NegativeOrZero<Payment>(refundAmount);

        if (Status is not (PaymentStatus.Succeeded or PaymentStatus.PartiallyRefunded))
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
}