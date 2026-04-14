using Models_UserId = Shoply.WebApi.Features.Users.Models.UserId;

namespace Shoply.WebApi.Common.Infrastructure.Persistence.Extensions;

public static class UserQueryExtensions
{
    extension(IQueryable<User> query)
    {
        public IQueryable<User> WithBasket(Models_UserId userId)
        {
            return query
                .Where(u => u.Id == userId)
                .Include(u => u.Basket)
                .ThenInclude(b => b.BasketItems)
                .ThenInclude(bi => bi.Product);
        }

        public IQueryable<User> WithOrdersAndBasket(Models_UserId userId)
        {
            return query
                .Where(u => u.Id == userId)
                .Include(u => u.Orders)
                .ThenInclude(o => o.OrderItems)
                .Include(u => u.Basket)
                .ThenInclude(b => b.BasketItems)
                .ThenInclude(bi => bi.Product);
        }

        public IQueryable<User> WithOrders(Models_UserId userId)
        {
            return query
                .Where(u => u.Id == userId)
                .Include(u => u.Orders)
                .ThenInclude(o => o.OrderItems);
        }
    }
}