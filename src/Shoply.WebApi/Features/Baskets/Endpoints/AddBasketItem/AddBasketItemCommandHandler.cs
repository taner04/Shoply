using Shoply.WebApi.Common.Infrastructure.Persistence.Extensions;
using Shoply.WebApi.Common.Infrastructure.Services;
using Shoply.WebApi.Features.Baskets.Exceptions;
using Shoply.WebApi.Features.Baskets.Models;

namespace Shoply.WebApi.Features.Baskets.Endpoints.AddBasketItem;

public sealed class AddBasketItemCommandHandler(ShoplyDbContext context, CurrentUserService userService)
    : ICommandHandler<AddBasketItemCommand>
{
    public async ValueTask<Unit> Handle(AddBasketItemCommand command, CancellationToken cancellationToken)
    {
        var product =
            await context.ProductsQuery.FirstOrDefaultAsync(p => p.Id == command.ProductId,
                cancellationToken) ?? throw new EntityNotFoundException<Product>(command.ProductId.Value);
        ProductOutOfStockException.ThrowIfOutOfStock(product);

        var userId = userService.GetCurrentUserId();
        var user = await context.Users
                       .WithBasket(userId)
                       .FirstOrDefaultAsync(cancellationToken)
                   ?? throw new EntityNotFoundException<User>(userId.Value);

        var basketItem = user.Basket.BasketItems.FirstOrDefault(p => p.ProductId == product.Id);
        if (basketItem is null)
        {
            user.Basket.BasketItems.Add(new BasketItem(product.Id));
        }
        else
        {
            basketItem.Quantity++;
        }

        context.Update(user);
        await context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}