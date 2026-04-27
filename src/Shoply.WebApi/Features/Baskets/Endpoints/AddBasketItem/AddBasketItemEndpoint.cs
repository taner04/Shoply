namespace Shoply.WebApi.Features.Baskets.Endpoints.AddBasketItem;

public sealed class AddBasketItemEndpoint : IEndpoint
{
    public void MapEndpoint(WebApplication app)
    {
        app.MapPost("/baskets/items",
                async ([FromBody] AddBasketItemCommand command, [FromServices] IMediator mediator) =>
                {
                    await mediator.Send(command);
                    return Results.NoContent();
                })
            .WithName("AddBasketItem")
            .WithTags("Baskets")
            .RequireRateLimiting(Security.RateLimiting.Global)
            .RequireAuthorization(Security.Policies.User)
            .Produces(StatusCodes.Status204NoContent)
            .ProducesApiProblemDetails();
    }
}