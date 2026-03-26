namespace Api.Features.Products.Endpoints.GetProductDetails;

public sealed class GetProductDetailsEndpoint : IEndpoint
{
    public void MapEndpoint(WebApplication app)
    {
        app.MapGet("/products/{productId:guid}",
                async (Guid productId, [FromServices] IMediator mediator) =>
                {
                    var query = new GetProductDetailsQuery(productId);
                    var result = await mediator.Send(query);
                    return Results.Ok(result);
                })
            .WithName("GetProductDetails")
            .WithTags("Products")
            .Produces<GetProductDetailsResponse>(StatusCodes.Status200OK)
            .ProducesApiProblemDetails();
    }
}
