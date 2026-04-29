namespace Shoply.WebApi.Features.Orders.Enums;

public enum PaymentStatus
{
    Pending,
    Paid,
    Canceled,
    Refunded,
    Failed,
    PartiallyRefunded
}