using Api.Common.Infrastructure.Persistence;
using Api.Common.Infrastructure.Services;
using Api.Common.Shared.Exceptions;
using Mediator;

namespace Api.Features.Baskets.Endpoints.GetBasket;

public sealed class GetBasketQueryHandler(ApplicationDbContext context, CurrentUserService userService)
    : IQueryHandler<GetBasketQuery, BasketResponse>
{
    public async ValueTask<BasketResponse> Handle(GetBasketQuery _, CancellationToken cancellationToken)
    {
        var userId = userService.GetCurrentUserId();
        var user = await context.Users.Where(u => u.Id == userId).Include(u => u.Basket).ThenInclude(b => b.BasketItems)
                       .ThenInclude(bi => bi.Product).FirstOrDefaultAsync(cancellationToken) ??
                   throw new EntityNotFoundException<User>(userId);

        return BasketResponse.FromBasket(user.Basket);
    }
}