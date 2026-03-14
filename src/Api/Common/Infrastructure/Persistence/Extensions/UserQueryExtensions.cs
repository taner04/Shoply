using Api.Features.Users.Models;
using UserId = Api.Features.Users.Models.UserId;

namespace Api.Common.Infrastructure.Persistence.Extensions;

public static class UserQueryExtensions
{
    extension(IQueryable<User> query)
    {
        /// <summary>
        ///     Includes Basket and BasketItems for a User query.
        ///     Use when only basket data is needed.
        /// </summary>
        public IQueryable<User> WithBasket(UserId userId)
        {
            return query
                .Where(u => u.Id == userId)
                .Include(u => u.Basket)
                .ThenInclude(b => b.BasketItems)
                .ThenInclude(bi => bi.Product);
        }

        /// <summary>
        ///     Includes Orders, OrderItems, Basket and BasketItems for a User query.
        ///     Use when full aggregate data is needed (command-side operations).
        ///     Performance: Use specific extensions (WithBasket/WithOrders) when possible.
        /// </summary>
        public IQueryable<User> WithOrdersAndBasket(UserId userId)
        {
            return query
                .Where(u => u.Id == userId)
                .Include(u => u.Orders)
                .ThenInclude(o => o.OrderItems)
                .Include(u => u.Basket)
                .ThenInclude(b => b.BasketItems)
                .ThenInclude(bi => bi.Product);
        }
    }
}