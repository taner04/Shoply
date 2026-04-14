using Shoply.WebApi.Features.Products.Exceptions;

namespace Shoply.WebApi.Features.Products.Endpoints.CreateProduct;

public sealed class CreateProductCommandHandler(ShoplyDbContext context) : ICommandHandler<CreateProductCommand>
{
    public async ValueTask<Unit> Handle(CreateProductCommand command, CancellationToken cancellationToken)
    {
        var doesSameNameExists = await context.Products
            .AsNoTracking()
            .AnyAsync(p => EF.Functions.ILike(p.Name, command.Name), cancellationToken);

        if (doesSameNameExists)
        {
            throw new DuplicateProductNameException(command.Name);
        }

        var product = Product.Create(
            command.Name,
            command.Price,
            command.Description,
            command.Stock,
            command.ImageUrl);

        context.Products.Add(product);
        await context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}