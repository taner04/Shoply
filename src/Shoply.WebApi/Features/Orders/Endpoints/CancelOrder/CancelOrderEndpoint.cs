namespace Shoply.WebApi.Features.Orders.Endpoints.CancelOrder;

public sealed class CancelOrderEndpoint : IEndpoint
{
    public void MapEndpoint(WebApplication app)
    {
        app.MapPost("orders/{orderId:guid}/cancel",
                async ([FromRoute] Guid orderId, [FromServices] IMediator mediator) =>
                {
                    await mediator.Send(new CancelOrderCommand(OrderId.From(orderId)));
                    return Results.Ok();
                })
            .WithName("CancelOrder")
            .WithTags("Orders")
            .RequireAuthorization(Policies.User)
            .ProducesApiProblemDetails();
    }
}