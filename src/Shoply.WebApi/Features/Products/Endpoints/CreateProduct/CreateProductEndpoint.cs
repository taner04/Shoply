namespace Shoply.WebApi.Features.Products.Endpoints.CreateProduct;

public sealed class CreateProductEndpoint : IEndpoint
{
    public void MapEndpoint(WebApplication app)
    {
        app.MapPost("/products", async ([FromBody] CreateProductCommand command, [FromServices] IMediator mediator) =>
            {
                await mediator.Send(command);
                return Results.Ok();
            })
            .WithName("CreateProduct")
            .WithTags("Products")
            .RequireRateLimiting(Security.RateLimiting.Global)
            .RequireAuthorization(Security.Policies.User)
            .Produces(StatusCodes.Status201Created)
            .ProducesApiProblemDetails();
    }
}