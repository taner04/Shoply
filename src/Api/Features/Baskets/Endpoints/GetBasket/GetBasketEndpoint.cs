using Api.Common.Abstractions;
using Api.Common.Composition.Extensions;
using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace Api.Features.Baskets.Endpoints.GetBasket;

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
            .Produces<BasketItemResponse>()
            .ProducesApiProblemDetails();
    }
}