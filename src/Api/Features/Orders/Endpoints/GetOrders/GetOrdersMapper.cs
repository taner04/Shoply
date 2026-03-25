using Api.Common.Attributes;

namespace Api.Features.Orders.Endpoints.GetOrders;

[ServiceInjection(ServiceLifetime.Singleton)]
public sealed class GetOrdersMapper : IMapper<Order, OrdersResponse>
{
    public List<OrdersResponse> Map(List<Order> source)
    {
        return [.. source.Select(order => new OrdersResponse(
            order.Id,
            order.UserId,
            order.Status.ToString(),
            new PaymentResponse(
                order.Payment.Id,
                order.Payment.Amount,
                order.Payment.Status.ToString(),
                order.Payment.RefundedAmount,
                order.Payment.FailureReason),
            [.. order.OrderItems.Select(oi => new OrderItemResponse(
                oi.ProductId,
                oi.ProductName,
                oi.UnitPrice,
                oi.Quantity
            ))]
        ))];
    }
}