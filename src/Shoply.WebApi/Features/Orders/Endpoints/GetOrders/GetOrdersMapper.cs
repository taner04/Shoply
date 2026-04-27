using Shoply.WebApi.Common.Attributes;

namespace Shoply.WebApi.Features.Orders.Endpoints.GetOrders;

[ServiceInjection(ServiceLifetime.Singleton)]
public sealed class GetOrdersMapper : IMapper<Order, OrdersResponse>
{
    public List<OrdersResponse> Map(List<Order> source)
    {
        return
        [
            .. source.Select(order => new OrdersResponse(
                order.Id.Value,
                order.UserId.Value,
                order.Status.ToString(),
                new PaymentResponse(
                    order.Payment.Id.Value,
                    order.Payment.Amount,
                    order.Payment.Status.ToString(),
                    order.Payment.RefundedAmount),
                [
                    .. order.OrderItems.Select(oi => new OrderItemResponse(
                        oi.ProductId.Value,
                        oi.ProductName,
                        oi.UnitPrice,
                        oi.Quantity
                    ))
                ]
            ))
        ];
    }
}