namespace Shoply.WebApi.Features.Products.Endpoints.GetProductDetails;

public sealed class GetProductDetailsEndpoint : IEndpoint
{
    public void MapEndpoint(WebApplication app)
    {
        app.MapGet("/products/{productId:guid}",
                async (Guid productId, [FromServices] IMediator mediator) =>
                {
                    var result = await mediator.Send(new GetProductDetailsQuery(ProductId.From(productId)));
                    return Results.Ok(result);
                })
            .WithName("GetProductDetails")
            .WithTags("Products")
            .RequireRateLimiting(Security.RateLimiting.Global)
            .RequireAuthorization(Security.Policies.User)
            .Produces<GetProductDetailsResponse>()
            .ProducesApiProblemDetails();
    }
}