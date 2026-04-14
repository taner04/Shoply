using Shoply.WebApi.Common.Infrastructure.Persistence.Extensions;
using Shoply.WebApi.Common.Infrastructure.Services;
using Shoply.WebApi.Features.Baskets.Exceptions;

namespace Shoply.WebApi.Features.Baskets.Endpoints.AddBasketItem;

public sealed class AddBasketItemCommandHandler(ShoplyDbContext context, CurrentUserService userService)
    : ICommandHandler<AddBasketItemCommand>
{
    public async ValueTask<Unit> Handle(AddBasketItemCommand command, CancellationToken cancellationToken)
    {
        var product =
            await context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == command.ProductId,
                cancellationToken) ?? throw new EntityNotFoundException<Product>(command.ProductId.Value);
        ProductOutOfStockException.ThrowIfOutOfStock(product);

        var userId = userService.GetCurrentUserId();
        var user = await context.Users
                       .WithBasket(userId)
                       .FirstOrDefaultAsync(cancellationToken)
                   ?? throw new EntityNotFoundException<User>(userId.Value);

        user.Basket.AddProduct(product);
        context.Update(user);
        await context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}