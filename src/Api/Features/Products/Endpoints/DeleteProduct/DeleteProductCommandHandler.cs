using Api.Common.Infrastructure.Persistence;
using Api.Common.Shared.Exceptions;
using Mediator;

namespace Api.Features.Products.Endpoints.DeleteProduct;

public sealed class DeleteProductCommandHandler(ApplicationDbContext context) : ICommandHandler<DeleteProductCommand>
{
    public async ValueTask<Unit> Handle(DeleteProductCommand command, CancellationToken cancellationToken)
    {
        var product =
            await context.Products.FirstOrDefaultAsync(p => p.Id == ProductId.From(command.ProductId),
                cancellationToken) ?? throw new EntityNotFoundException<Product>(command.ProductId);

        context.Products.Remove(product);
        await context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}