using UserId = Api.Features.Users.Models.UserId;

namespace Api.Common.Infrastructure.Persistence.Extensions;

public static class UserQueryExtensions
{
    extension(IQueryable<User> query)
    {
        public IQueryable<User> WithBasket(UserId userId)
        {
            return query
                .Where(u => u.Id == userId)
                .Include(u => u.Basket)
                .ThenInclude(b => b.BasketItems)
                .ThenInclude(bi => bi.Product);
        }
        
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

        public IQueryable<User> WithOrders(UserId userId)
        {
            return query
                .Where(u => u.Id == userId)
                .Include(u => u.Orders)
                .ThenInclude(o => o.OrderItems);
        }
    }
}