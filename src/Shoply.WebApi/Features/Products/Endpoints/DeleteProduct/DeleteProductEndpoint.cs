namespace Shoply.WebApi.Features.Products.Endpoints.DeleteProduct;

public sealed class DeleteProductEndpoint : IEndpoint
{
    public void MapEndpoint(WebApplication app)
    {
        app.MapDelete("/products/{productId:guid}", async (Guid productId, [FromServices] IMediator mediator) =>
            {
                await mediator.Send(new DeleteProductCommand(ProductId.From(productId)));
                return Results.NoContent();
            })
            .WithName("DeleteProduct")
            .WithTags("Products")
            .RequireRateLimiting(Security.RateLimiting.Global)
            .RequireAuthorization(Security.Policies.User)
            .Produces(StatusCodes.Status204NoContent)
            .ProducesApiProblemDetails();
    }
}