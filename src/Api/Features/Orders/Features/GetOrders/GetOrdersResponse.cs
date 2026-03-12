namespace Api.Features.Orders.Features.GetOrders;

public sealed record OrdersResponse(
    Guid Id,
    Guid UserId,
    IReadOnlyCollection<OrderItemResponse> Items);

public sealed record OrderItemResponse(
    Guid ProductId,
    string ProductName,
    decimal UnitPrice,
    int Quantity);