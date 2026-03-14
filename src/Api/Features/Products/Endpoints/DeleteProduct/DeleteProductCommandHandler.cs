using Api.Common.Infrastructure.Persistence;
using Api.Common.Shared.Exceptions;
using Api.Features.Products.Models;
using Mediator;
using ProductId = Api.Features.Products.Models.ProductId;

namespace Api.Features.Products.Endpoints.DeleteProduct;

public sealed class DeleteProductCommandHandler(ApplicationDbContext context) : ICommandHandler<DeleteProductCommand>
{
    public async ValueTask<Unit> Handle(DeleteProductCommand command, CancellationToken cancellationToken)
    {
        var product =
            await context.ProductsQuery.FirstOrDefaultAsync(p => p.Id == ProductId.From(command.ProductId),
                cancellationToken) ?? throw new EntityNotFoundException<Product>(command.ProductId);

        context.Products.Remove(product);
        await context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}