using Shoply.WebApi.Common.Infrastructure.Persistence.Extensions;
using Shoply.WebApi.Common.Infrastructure.Services;

namespace Shoply.WebApi.Features.Baskets.Endpoints.GetBasket;

public sealed class GetBasketQueryHandler(ShoplyDbContext context, CurrentUserService userService)
    : IQueryHandler<GetBasketQuery, BasketResponse>
{
    public async ValueTask<BasketResponse> Handle(GetBasketQuery _, CancellationToken cancellationToken)
    {
        var userId = userService.GetCurrentUserId();
        var user = await context.Users
                       .WithBasket(userId)
                       .AsNoTracking()
                       .FirstOrDefaultAsync(cancellationToken)
                   ?? throw new EntityNotFoundException<User>(userId.Value);

        return BasketResponse.FromBasket(user.Basket);
    }
}