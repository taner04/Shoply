using Api.Common.Infrastructure.Persistence;
using Api.Common.Infrastructure.Persistence.Extensions;
using Api.Common.Infrastructure.Services;
using Api.Common.Shared.Exceptions;
using Api.Features.Users.Models;
using Mediator;

namespace Api.Features.Baskets.Endpoints.GetBasket;

public sealed class GetBasketQueryHandler(ApplicationDbContext context, CurrentUserService userService)
    : IQueryHandler<GetBasketQuery, BasketResponse>
{
    public async ValueTask<BasketResponse> Handle(GetBasketQuery _, CancellationToken cancellationToken)
    {
        var userId = userService.GetCurrentUserId();
        var user = await context.Users
                       .WithBasket(userId)
                       .AsNoTracking()
                       .FirstOrDefaultAsync(cancellationToken)
                   ?? throw new EntityNotFoundException<User>(userId);

        return BasketResponse.FromBasket(user.Basket);
    }
}