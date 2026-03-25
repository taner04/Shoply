using Api.Features.Orders.Exceptions;
using Api.Features.Orders.Models;

namespace UnitTests.Domain.Orders;

public sealed class PaymentTests
{
    private static OrderId CreateOrderId() => OrderId.From(Guid.NewGuid());

    [Fact]
    public void Create_WithValidParameters_ShouldCreatePayment()
    {
        var orderId = CreateOrderId();
        var amount = 100.00m;

        var payment = Payment.Create(orderId, amount);

        Assert.Equal(orderId, payment.OrderId);
        Assert.Equal(amount, payment.Amount);
        Assert.Equal(PaymentStatus.Pending, payment.Status);
        Assert.Null(payment.StripePaymentIntentId);
        Assert.Null(payment.FailureReason);
        Assert.Equal(0, payment.RefundedAmount);
    }

    [Fact]
    public void Create_WithNegativeAmount_ShouldThrow()
    {
        var orderId = CreateOrderId();

        var ex = Assert.Throws<GuardException>(() => Payment.Create(orderId, -10.00m));
        Assert.NotNull(ex);
    }

    [Fact]
    public void Create_WithZeroAmount_ShouldThrow()
    {
        var orderId = CreateOrderId();

        var ex = Assert.Throws<GuardException>(() => Payment.Create(orderId, 0));
        Assert.NotNull(ex);
    }

    [Fact]
    public void SetStripePaymentIntentId_WithValidId_ShouldSetId()
    {
        var payment = Payment.Create(CreateOrderId(), 100.00m);
        var intentId = "pi_1234567890";

        payment.SetStripePaymentIntentId(intentId);

        Assert.Equal(intentId, payment.StripePaymentIntentId);
    }

    [Fact]
    public void SetStripePaymentIntentId_WhenAlreadySet_ShouldThrow()
    {
        var payment = Payment.Create(CreateOrderId(), 100.00m);
        payment.SetStripePaymentIntentId("pi_1234567890");

        var ex = Assert.Throws<PaymentIntentIdAlreadySetException>(() => 
            payment.SetStripePaymentIntentId("pi_9876543210"));
        
        Assert.NotNull(ex);
    }

    [Fact]
    public void MarkProcessing_FromPending_ShouldSucceed()
    {
        var payment = Payment.Create(CreateOrderId(), 100.00m);

        payment.MarkProcessing();

        Assert.Equal(PaymentStatus.Processing, payment.Status);
    }

    [Fact]
    public void MarkProcessing_WhenNotPending_ShouldThrow()
    {
        var payment = Payment.Create(CreateOrderId(), 100.00m);
        payment.MarkProcessing();

        var ex = Assert.Throws<InvalidPaymentStatusTransitionException>(() => payment.MarkProcessing());
        Assert.NotNull(ex);
    }

    [Fact]
    public void MarkSucceeded_FromProcessing_ShouldSucceed()
    {
        var payment = Payment.Create(CreateOrderId(), 100.00m);
        payment.MarkProcessing();

        payment.MarkSucceeded();

        Assert.Equal(PaymentStatus.Succeeded, payment.Status);
    }

    [Fact]
    public void MarkSucceeded_WhenNotProcessing_ShouldThrow()
    {
        var payment = Payment.Create(CreateOrderId(), 100.00m);

        var ex = Assert.Throws<InvalidPaymentStatusTransitionException>(() => payment.MarkSucceeded());
        Assert.NotNull(ex);
    }

    [Fact]
    public void MarkFailed_FromPending_ShouldSucceed()
    {
        var payment = Payment.Create(CreateOrderId(), 100.00m);
        var reason = "Card declined";

        payment.MarkFailed(reason);

        Assert.Equal(PaymentStatus.Failed, payment.Status);
        Assert.Equal(reason, payment.FailureReason);
    }

    [Fact]
    public void MarkFailed_FromProcessing_ShouldSucceed()
    {
        var payment = Payment.Create(CreateOrderId(), 100.00m);
        payment.MarkProcessing();
        var reason = "Card declined";

        payment.MarkFailed(reason);

        Assert.Equal(PaymentStatus.Failed, payment.Status);
        Assert.Equal(reason, payment.FailureReason);
    }

    [Fact]
    public void MarkFailed_WhenSucceeded_ShouldThrow()
    {
        var payment = Payment.Create(CreateOrderId(), 100.00m);
        payment.MarkProcessing();
        payment.MarkSucceeded();

        var ex = Assert.Throws<InvalidPaymentStatusTransitionException>(() => 
            payment.MarkFailed("Card declined"));
        
        Assert.NotNull(ex);
    }

    [Fact]
    public void MarkFailed_WhenRefunded_ShouldThrow()
    {
        var payment = Payment.Create(CreateOrderId(), 100.00m);
        payment.MarkProcessing();
        payment.MarkSucceeded();
        payment.MarkRefunded(100.00m);

        var ex = Assert.Throws<InvalidPaymentStatusTransitionException>(() => 
            payment.MarkFailed("Card declined"));
        
        Assert.NotNull(ex);
    }

    [Fact]
    public void MarkRefunded_WithFullAmount_ShouldMarkAsRefunded()
    {
        var payment = Payment.Create(CreateOrderId(), 100.00m);
        payment.MarkProcessing();
        payment.MarkSucceeded();

        payment.MarkRefunded(100.00m);

        Assert.Equal(PaymentStatus.Refunded, payment.Status);
        Assert.Equal(100.00m, payment.RefundedAmount);
    }

    [Fact]
    public void MarkRefunded_WithPartialAmount_ShouldMarkAsPartiallyRefunded()
    {
        var payment = Payment.Create(CreateOrderId(), 100.00m);
        payment.MarkProcessing();
        payment.MarkSucceeded();

        payment.MarkRefunded(50.00m);

        Assert.Equal(PaymentStatus.PartiallyRefunded, payment.Status);
        Assert.Equal(50.00m, payment.RefundedAmount);
    }

    [Fact]
    public void MarkRefunded_WhenNotSucceeded_ShouldThrow()
    {
        var payment = Payment.Create(CreateOrderId(), 100.00m);

        var ex = Assert.Throws<InvalidPaymentStatusTransitionException>(() => 
            payment.MarkRefunded(50.00m));
        
        Assert.NotNull(ex);
    }

    [Fact]
    public void MarkRefunded_WithAmountExceedingBalance_ShouldThrow()
    {
        var payment = Payment.Create(CreateOrderId(), 100.00m);
        payment.MarkProcessing();
        payment.MarkSucceeded();

        var ex = Assert.Throws<PaymentRefundExceedsBalanceException>(() => 
            payment.MarkRefunded(150.00m));
        
        Assert.NotNull(ex);
    }

    [Fact]
    public void MarkRefunded_MultiplePartialRefunds_ShouldAccumulate()
    {
        var payment = Payment.Create(CreateOrderId(), 100.00m);
        payment.MarkProcessing();
        payment.MarkSucceeded();

        payment.MarkRefunded(30.00m);
        Assert.Equal(30.00m, payment.RefundedAmount);
        Assert.Equal(PaymentStatus.PartiallyRefunded, payment.Status);

        payment.MarkRefunded(40.00m);
        Assert.Equal(70.00m, payment.RefundedAmount);
        Assert.Equal(PaymentStatus.PartiallyRefunded, payment.Status);

        payment.MarkRefunded(30.00m);
        Assert.Equal(100.00m, payment.RefundedAmount);
        Assert.Equal(PaymentStatus.Refunded, payment.Status);
    }

    [Fact]
    public void MarkRefunded_WithZeroAmount_ShouldThrow()
    {
        var payment = Payment.Create(CreateOrderId(), 100.00m);
        payment.MarkProcessing();
        payment.MarkSucceeded();

        var ex = Assert.Throws<GuardException>(() => payment.MarkRefunded(0));
        Assert.NotNull(ex);
    }
}
