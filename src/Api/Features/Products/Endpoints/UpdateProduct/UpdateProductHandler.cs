using ProductId = Api.Features.Products.Models.ProductId;

namespace Api.Features.Products.Endpoints.UpdateProduct;

public sealed class UpdateProductHandler(ApplicationDbContext context) : ICommandHandler<UpdateProductCommand>
{
    public async ValueTask<Unit> Handle(UpdateProductCommand command, CancellationToken cancellationToken)
    {
        var product =
            await context.ProductsQuery.FirstOrDefaultAsync(p => p.Id == ProductId.From(command.ProductId),
                cancellationToken) ?? throw new EntityNotFoundException<Product>(command.ProductId);

        product.Update(command.Name, command.Price, command.Description, command.Stock, command.ImageUrl);

        context.Update(product);
        await context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}