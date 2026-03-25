using Api.Common.Infrastructure.Persistence.Extensions;
using Api.Common.Infrastructure.Services;
using Api.Common.Infrastructure.Services.Emails;

namespace Api.Features.Orders.Endpoints.CancelOrder;

public sealed class CancelOrderCommandHandler(
    ApplicationDbContext context,
    CurrentUserService userService,
    IEmailService emailService) : ICommandHandler<CancelOrderCommand>
{
    public async ValueTask<Unit> Handle(CancelOrderCommand command, CancellationToken cancellationToken)
    {
        var userId = userService.GetCurrentUserId();
        var user = await context.Users.WithOrders(userId).FirstOrDefaultAsync(cancellationToken) ??
                   throw new EntityNotFoundException<User>(userId);
        
        var order = user.Orders.FirstOrDefault(o => o.Id == command.OrderId) ??
                    throw new EntityNotFoundException<Order>(command.OrderId);
        
        var productIds = user.Basket.BasketItems.Select(bi => bi.ProductId).ToList();
        var products = await context.Products
            .Where(p => productIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id, cancellationToken);
        
        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            order.MarkCancelled();
            foreach (var orderItem in order.OrderItems)
            {
                if (!products.TryGetValue(orderItem.ProductId, out var product))
                {
                    throw new EntityNotFoundException<Product>(orderItem.ProductId);
                }

                product.IncreaseQuantity(orderItem.Quantity);
            }
            
            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            
            await emailService.SendEmailAsync(new CancelOrderEmailTemplate(order, user.Email), cancellationToken);
            return Unit.Value;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}