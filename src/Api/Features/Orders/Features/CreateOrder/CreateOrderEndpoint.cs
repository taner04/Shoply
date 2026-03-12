using Api.Common.Abstractions;
using Api.Common.Composition.Extensions;
using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace Api.Features.Orders.Features.CreateOrder;

public sealed class CreateOrderEndpoint : IEndpoint
{
    public void MapEndpoint(WebApplication app)
    {
        app.MapPost("/orders", async ([FromServices] IMediator mediator) =>
            {
                await mediator.Send(new CreateOrderCommand());
                return Results.Created();
            })
            .WithName("CreateOrder")
            .WithTags("Orders")
            .Produces(StatusCodes.Status201Created)
            .ProducesApiProblemDetails();
    }
}