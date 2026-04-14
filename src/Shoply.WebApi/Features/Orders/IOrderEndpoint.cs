using Refit;
using Shoply.WebApi.Features.Orders.Endpoints.CreateOrder;

namespace Shoply.WebApi.Features.Orders;

public interface IOrderEndpoint
{
    [Get("/orders")]
    Task<HttpResponseMessage> GetOrdersAsync(
        [Query] int pageIndex = 1,
        [Query] int pageSize = 10,
        CancellationToken cancellationToken = default);

    [Post("/orders")]
    Task<HttpResponseMessage> CreateOrderAsync(
        [Body] CreateOrderCommand command,
        CancellationToken cancellationToken);

    [Post("/orders/{orderId}/cancel")]
    Task<HttpResponseMessage> CancelOrderAsync(
        OrderId orderId,
        CancellationToken cancellationToken = default);
}