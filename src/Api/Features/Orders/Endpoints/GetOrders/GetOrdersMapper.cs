using Api.Common.Abstractions;
using Api.Common.Attributes;
using Api.Features.Orders.Models;

namespace Api.Features.Orders.Endpoints.GetOrders;

[ServiceInjection(ServiceLifetime.Singleton)]
public sealed class GetOrdersMapper : IMapper<Order, OrdersResponse>
{
    public List<OrdersResponse> Map(List<Order> source)
    {
        return source.Select(order => new OrdersResponse(
            order.Id,
            order.UserId,
            order.OrderItems.Select(oi => new OrderItemResponse(
                oi.ProductId,
                oi.ProductName,
                oi.UnitPrice,
                oi.Quantity
            )).ToList()
        )).ToList();
    }
}