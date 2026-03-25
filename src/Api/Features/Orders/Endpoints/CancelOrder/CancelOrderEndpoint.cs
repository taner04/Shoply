namespace Api.Features.Orders.Endpoints.CancelOrder;

public sealed class CancelOrderEndpoint : IEndpoint
{
    public void MapEndpoint(WebApplication app)
    {
        app.MapPost("orders/cancel", async ([FromServices] IMediator mediator, [FromBody] CancelOrderCommand command) =>
            {
                await mediator.Send(command);
                return Results.Ok();
            })
            .WithName("CancelOrder")
            .WithTags("Orders")
            .RequireAuthorization(Policies.User)
            .ProducesApiProblemDetails();
    }
}