using Api.Features.Orders.Features.CreateOrder;
using Api.Features.Orders.Features.GetOrders;
using Refit;

namespace Api.Features.Orders;

public interface IOrderEndpoint
{
    [Get("/orders")]
    Task<HttpResponseMessage> GetOrdersAsync([Query] int pageIndex = 1, [Query] int pageSize = 10, CancellationToken cancellationToken = default);

    [Post("/orders")]
    Task<HttpResponseMessage> CreateOrderAsync(CreateOrderCommand command, CancellationToken cancellationToken);
}