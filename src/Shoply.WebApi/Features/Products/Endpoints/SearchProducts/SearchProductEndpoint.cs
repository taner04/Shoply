using Shoply.WebApi.Common.Infrastructure.Services.Pagination;
using Shoply.WebApi.Features.Products.Endpoints.GetProducts;

namespace Shoply.WebApi.Features.Products.Endpoints.SearchProducts;

internal sealed class SearchProductEndpoint : IEndpoint
{
    public void MapEndpoint(WebApplication app)
    {
        app.MapGet("/products/search={name}",
                async (
                    [FromServices] IMediator mediator,
                    string name,
                    [FromQuery] int pageIndex = 1,
                    [FromQuery] int pageSize = 10) =>
                {
                    var query = new SearchProductQuery(pageIndex, pageSize, name);
                    return await mediator.Send(query);
                })
            .WithName("SearchProduct")
            .WithTags("Products")
            .RequireRateLimiting(Security.RateLimiting.Global)
            .Produces<PaginationResult<ProductsResponse>>()
            .ProducesApiProblemDetails();
    }
}