namespace Shoply.WebApi.Features.Orders.Endpoints.CreateOrder;

public sealed class CreateOrderEndpoint : IEndpoint
{
    public void MapEndpoint(WebApplication app)
    {
        app.MapPost("/orders", async ([FromServices] IMediator mediator) =>
            {
                var checkOutSessionUrl = await mediator.Send(new CreateOrderCommand());
                return Results.Created("/orders", checkOutSessionUrl);
            })
            .WithName("CreateOrder")
            .WithTags("Orders")
            .RequireRateLimiting(Security.RateLimiting.Global)
            .RequireAuthorization(Security.Policies.User)
            .Produces(StatusCodes.Status201Created)
            .ProducesApiProblemDetails();
    }
}