namespace Shoply.WebApi.Features.Products.Endpoints.UpdateProduct;

public sealed class UpdateProductCommandHandler(ShoplyDbContext context) : ICommandHandler<UpdateProductCommand>
{
    public async ValueTask<Unit> Handle(UpdateProductCommand command, CancellationToken cancellationToken)
    {
        var product =
            await context.Products.FirstOrDefaultAsync(p => p.Id == command.ProductId,
                cancellationToken) ?? throw new EntityNotFoundException<Product>(command.ProductId.Value);

        product.Price = command.Price;
        product.Quantity = command.Quantity;
        product.Name = command.Name;
        product.Description = command.Description;
        product.ImageUrl = command.ImageUrl;

        context.Update(product);
        await context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}