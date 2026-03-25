using Api.Common.Shared.Pagination;

namespace Api.Features.Products.Endpoints.GetProducts;

public sealed class GetProductsEndpoint : IEndpoint
{
    public void MapEndpoint(WebApplication app)
    {
        app.MapGet("/products",
                async ([FromServices] IMediator mediator, [FromQuery] int pageIndex = 1,
                    [FromQuery] int pageSize = 10) =>
                {
                    var query = new GetProductsQuery(pageIndex, pageSize);
                    return await mediator.Send(query);
                })
            .WithName("GetProducts")
            .WithTags("Products")
            .Produces<PaginationResult<GetProductsResponse>>()
            .ProducesApiProblemDetails();
    }
}