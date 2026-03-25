namespace Api.Features.Baskets.Endpoints.DeleteBasketItem;

public sealed class DeleteBasketItemEndpoint : IEndpoint
{
    public void MapEndpoint(WebApplication app)
    {
        app.MapDelete("/baskets/items",
                async ([FromBody] DeleteBasketItemCommand command, [FromServices] IMediator mediator) =>
                {
                    await mediator.Send(command);
                    return Results.NoContent();
                })
            .WithName("DeleteBasketItem")
            .WithTags("Baskets")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesApiProblemDetails();
    }
}