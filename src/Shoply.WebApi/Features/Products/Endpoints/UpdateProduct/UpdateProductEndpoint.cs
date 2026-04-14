namespace Shoply.WebApi.Features.Products.Endpoints.UpdateProduct;

public sealed class UpdateProductEndpoint : IEndpoint
{
    public void MapEndpoint(WebApplication app)
    {
        app.MapPut("/products/{productId:guid}", async (
                Guid productId,
                [FromBody] UpdateProductCommand command,
                [FromServices] IMediator mediator) =>
            {
                await mediator.Send(command with { ProductId = ProductId.From(productId) });
                return Results.Ok();
            })
            .WithName("UpdateProduct")
            .WithTags("Products")
            .Produces(StatusCodes.Status200OK)
            .ProducesApiProblemDetails();
    }
}