using Api.Common.Abstractions;
using Api.Common.Composition.Extensions;
using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace Api.Features.Products.Endpoints.UpdateProduct;

public sealed class UpdateProductEndpoint : IEndpoint
{
    public void MapEndpoint(WebApplication app)
    {
        app.MapPut("/products/{productId:guid}", async (Guid productId, [FromBody] UpdateProductCommand command,
                [FromServices] IMediator mediator) =>
            {
                command = command with { ProductId = productId };
                await mediator.Send(command);
                return Results.Ok();
            })
            .WithName("UpdateProduct")
            .WithTags("Products")
            .Produces(StatusCodes.Status200OK)
            .ProducesApiProblemDetails();
    }
}