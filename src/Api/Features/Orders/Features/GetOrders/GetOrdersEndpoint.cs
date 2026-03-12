using System.Net;
using Api.Common.Abstractions;
using Api.Common.Composition.Extensions;
using Api.Common.Shared.Pagination;
using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace Api.Features.Orders.Features.GetOrders;

public sealed class GetOrdersEndpoint : IEndpoint
{
    public void MapEndpoint(WebApplication app)
    {
        app.MapGet("/orders",
                async ([FromServices] IMediator mediator, [FromQuery] int pageIndex = 1,
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