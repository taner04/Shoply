using Shoply.WebApi.Common.Infrastructure.Persistence.Extensions;
using Shoply.WebApi.Common.Infrastructure.Services;
using Shoply.WebApi.Features.Baskets.Models;
using ProductId = Shoply.WebApi.Features.Products.Models.ProductId;

namespace Shoply.WebApi.Features.Baskets.Endpoints.DeleteBasketItem;

public sealed class DeleteBasketItemCommandHandler(ShoplyDbContext context, CurrentUserService userService)
    : ICommandHandler<DeleteBasketItemCommand>
{
    public async ValueTask<Unit> Handle(DeleteBasketItemCommand command, CancellationToken cancellationToken)
    {
        var userId = userService.GetCurrentUserId();
        var user = await context.Users
                       .WithBasket(userId)
                       .FirstOrDefaultAsync(cancellationToken)
                   ?? throw new EntityNotFoundException<User>(userId.Value);

        var productId = ProductId.From(command.ProductId);

        var basketItem = user.Basket.BasketItems.FirstOrDefault(p => p.ProductId == productId) ??
                         throw new EntityNotFoundException<BasketItem>(productId.Value);

        if (basketItem.Quantity <= 1)
        {
            user.Basket.BasketItems.Remove(basketItem);
        }
        else
        {
            basketItem.Quantity--;
        }

        context.Update(user);
        await context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}