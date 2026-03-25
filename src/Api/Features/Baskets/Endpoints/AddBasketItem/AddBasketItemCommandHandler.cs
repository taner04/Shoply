using Api.Common.Infrastructure.Persistence.Extensions;
using Api.Common.Infrastructure.Services;
using Api.Features.Baskets.Exceptions;
using ProductId = Api.Features.Products.Models.ProductId;

namespace Api.Features.Baskets.Endpoints.AddBasketItem;

public sealed class AddBasketItemCommandHandler(ApplicationDbContext context, CurrentUserService userService)
    : ICommandHandler<AddBasketItemCommand>
{
    public async ValueTask<Unit> Handle(AddBasketItemCommand command, CancellationToken cancellationToken)
    {
        var product =
            await context.ProductsQuery.FirstOrDefaultAsync(p => p.Id == ProductId.From(command.ProductId),
                cancellationToken) ?? throw new EntityNotFoundException<Product>(command.ProductId);
        ProductOutOfStockException.ThrowIfOutOfStock(product);

        var userId = userService.GetCurrentUserId();
        var user = await context.Users
                       .WithBasket(userId)
                       .FirstOrDefaultAsync(cancellationToken)
                   ?? throw new EntityNotFoundException<User>(userId);

        user.Basket.AddProduct(product);
        context.Update(user);
        await context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}