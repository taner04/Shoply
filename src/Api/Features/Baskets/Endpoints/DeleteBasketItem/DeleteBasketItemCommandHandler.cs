using Api.Common.Infrastructure.Persistence;
using Api.Common.Infrastructure.Persistence.Extensions;
using Api.Common.Infrastructure.Services;
using Api.Common.Shared.Exceptions;
using Api.Features.Users.Models;
using Mediator;
using ProductId = Api.Features.Products.Models.ProductId;

namespace Api.Features.Baskets.Endpoints.DeleteBasketItem;

public sealed class DeleteBasketItemCommandHandler(ApplicationDbContext context, CurrentUserService userService)
    : ICommandHandler<DeleteBasketItemCommand>
{
    public async ValueTask<Unit> Handle(DeleteBasketItemCommand command, CancellationToken cancellationToken)
    {
        var userId = userService.GetCurrentUserId();
        var user = await context.Users
                       .WithBasket(userId)
                       .FirstOrDefaultAsync(cancellationToken)
                   ?? throw new EntityNotFoundException<User>(userId);

        var productId = ProductId.From(command.ProductId);
        user.Basket.RemoveProduct(productId);

        context.Update(user);
        await context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}