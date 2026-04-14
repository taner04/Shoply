using Shoply.WebApi.Common.Shared.Pagination;

namespace Shoply.WebApi.Features.Orders.Endpoints.GetOrders;

public sealed class GetOrdersEndpoint : IEndpoint
{
    public void MapEndpoint(WebApplication app)
    {
        app.MapGet("/orders",
                async (
                    [FromServices] IMediator mediator,
                    [FromQuery] int pageIndex = 1,
                    [FromQuery] int pageSize = 10) =>
                {
                    var query = new GetOrdersQuery(pageIndex, pageSize);
                    return await mediator.Send(query);
                })
            .WithName("GetOrders")
            .WithTags("Orders")
            .Produces<PaginationResult<OrdersResponse>>()
            .ProducesApiProblemDetails();
    }
}