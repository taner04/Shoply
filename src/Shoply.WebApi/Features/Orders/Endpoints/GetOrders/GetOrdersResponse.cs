namespace Shoply.WebApi.Features.Orders.Endpoints.GetOrders;

public sealed record OrdersResponse(
    Guid Id,
    Guid UserId,
    string Status,
    PaymentResponse Payment,
    IReadOnlyCollection<OrderItemResponse> Items);

public sealed record PaymentResponse(
    Guid Id,
    decimal Amount,
    string Status,
    decimal RefundedAmount,
    string? FailureReason);

public sealed record OrderItemResponse(
    Guid ProductId,
    string ProductName,
    decimal UnitPrice,
    int Quantity);