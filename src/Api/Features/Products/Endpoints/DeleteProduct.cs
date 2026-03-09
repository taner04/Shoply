using Api.Common.Abstractions;
using Api.Common.Composition.Extensions;
using Api.Common.Infrastructure.Persistence;
using Api.Common.Shared.Exceptions;
using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace Api.Features.Products.Endpoints;

public record DeleteProductCommand(Guid ProductId) : ICommand;

public class DeleteProductEndpoint : IEndpoint
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

public sealed class DeleteProductCommandHandler(ApplicationDbContext context) : ICommandHandler<DeleteProductCommand>
{
    public async ValueTask<Unit> Handle(DeleteProductCommand command, CancellationToken cancellationToken)
    {
        var product =
            await context.Products.FirstOrDefaultAsync(p => p.Id == ProductId.From(command.ProductId),
                cancellationToken);
        if (product is null)
        {
            throw new EntityNotFoundException<Product>(command.ProductId);
        }

        context.Products.Remove(product);
        await context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}