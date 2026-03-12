using Api.Common.Abstractions;
using Api.Common.Composition.Extensions;
using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace Api.Features.Products.Endpoints.DeleteProduct;

public sealed class DeleteProductEndpoint : IEndpoint
{
    public void MapEndpoint(WebApplication app)
    {
        app.MapDelete("/products/{productId:guid}", async (Guid productId, [FromServices] IMediator mediator) =>
            {
                var command = new DeleteProductCommand(productId);
                await mediator.Send(command);
                return Results.NoContent();
            })
            .WithName("DeleteProduct")
            .WithTags("Products")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesApiProblemDetails();
    }
}