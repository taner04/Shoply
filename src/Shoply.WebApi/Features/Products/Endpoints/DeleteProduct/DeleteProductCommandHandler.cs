namespace Shoply.WebApi.Features.Products.Endpoints.DeleteProduct;

public sealed class DeleteProductCommandHandler(ShoplyDbContext context) : ICommandHandler<DeleteProductCommand>
{
    public async ValueTask<Unit> Handle(DeleteProductCommand command, CancellationToken cancellationToken)
    {
        var product =
            await context.Products.FirstOrDefaultAsync(p => p.Id == command.ProductId,
                cancellationToken) ?? throw new EntityNotFoundException<Product>(command.ProductId.Value);

        context.Products.Remove(product);
        await context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}