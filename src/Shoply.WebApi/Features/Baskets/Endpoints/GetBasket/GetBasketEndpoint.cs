namespace Shoply.WebApi.Features.Baskets.Endpoints.GetBasket;

public sealed class GetBasketEndpoint : IEndpoint
{
    public void MapEndpoint(WebApplication app)
    {
        app.MapGet("/baskets", async ([FromServices] IMediator mediator) =>
            {
                var basket = await mediator.Send(new GetBasketQuery());
                return Results.Ok(basket);
            })
            .WithName("GetBasket")
            .WithTags("Baskets")
            .RequireRateLimiting(Security.RateLimiting.Global)
            .RequireAuthorization(Security.Policies.User)
            .Produces<BasketItemResponse>()
            .ProducesApiProblemDetails();
    }
}