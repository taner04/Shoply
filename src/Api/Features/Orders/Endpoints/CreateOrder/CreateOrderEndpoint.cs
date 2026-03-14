using Api.Common.Abstractions;
using Api.Common.Composition.Extensions;
using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace Api.Features.Orders.Endpoints.CreateOrder;

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
            .Produces(StatusCodes.Status201Created)
            .ProducesApiProblemDetails();
    }
}