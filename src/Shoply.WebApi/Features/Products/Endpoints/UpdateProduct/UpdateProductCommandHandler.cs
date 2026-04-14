namespace Shoply.WebApi.Features.Products.Endpoints.UpdateProduct;

public sealed class UpdateProductCommandHandler(ShoplyDbContext context) : ICommandHandler<UpdateProductCommand>
{
    public async ValueTask<Unit> Handle(UpdateProductCommand command, CancellationToken cancellationToken)
    {
        var product =
            await context.ProductsQuery.FirstOrDefaultAsync(p => p.Id == command.ProductId,
                cancellationToken) ?? throw new EntityNotFoundException<Product>(command.ProductId.Value);

        product.Update(command.Name, command.Price, command.Description, command.Stock, command.ImageUrl);

        context.Update(product);
        await context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}