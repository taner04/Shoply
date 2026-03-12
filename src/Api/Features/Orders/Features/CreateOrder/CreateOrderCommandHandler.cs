using Api.Common.Domain.Orders;
using Api.Common.Infrastructure.Persistence;
using Api.Common.Infrastructure.Services;
using Api.Common.Shared.Exceptions;
using Api.Features.Orders.Exceptions;
using Mediator;

namespace Api.Features.Orders.Features.CreateOrder;

public sealed class CreateOrderCommandHandler(ApplicationDbContext context, CurrentUserService userService)
    : ICommandHandler<CreateOrderCommand>
{
    public async ValueTask<Unit> Handle(CreateOrderCommand command, CancellationToken cancellationToken)
    {
        var userId = userService.GetCurrentUserId();

        var user = await context.Users.Include(u => u.Basket).ThenInclude(b => b.BasketItems)
                       .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken) ??
                   throw new EntityNotFoundException<User>(userId);

        if (user.Basket.IsEmpty())
        {
            throw new OrderBasketEmptyException();
        }

        var productIds = user.Basket.BasketItems.Select(bi => bi.ProductId).ToList();
        var products = await context.Products
            .Where(p => productIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id, cancellationToken);

        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var orderItems = new List<OrderItem>();
            foreach (var basketItem in user.Basket.BasketItems)
            {
                if (!products.TryGetValue(basketItem.ProductId, out var product))
                {
                    throw new EntityNotFoundException<Product>(basketItem.ProductId);
                }

                product.DecreaseQuantity(basketItem.Quantity);
                orderItems.Add(OrderItem.From(product, basketItem.Quantity));
            }

            var newOrder = Order.Create(user.Id, orderItems);
            user.AddOrder(newOrder);
            user.Basket.EmptyBasket();

            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }

        return Unit.Value;
    }
}