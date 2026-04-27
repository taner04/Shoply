using Shoply.WebApi.Common.Infrastructure.Persistence.Extensions;
using Shoply.WebApi.Common.Infrastructure.Services;
using Shoply.WebApi.Common.Infrastructure.Services.Emails;

namespace Shoply.WebApi.Features.Orders.Endpoints.CancelOrder;

public sealed class CancelOrderCommandHandler(
    ShoplyDbContext context,
    CurrentUserService userService,
    StripePaymentProvider stripePaymentProvider,
    IEmailService emailService) : ICommandHandler<CancelOrderCommand>
{
    public async ValueTask<Unit> Handle(CancelOrderCommand command, CancellationToken cancellationToken)
    {
        var userId = userService.GetCurrentUserId();

        var user = await context.Users.WithOrders(userId).FirstOrDefaultAsync(cancellationToken) ??
                   throw new EntityNotFoundException<User>(userId.Value);

        var order = user.Orders.FirstOrDefault(o => o.Id == command.OrderId) ??
                    throw new EntityNotFoundException<Order>(command.OrderId.Value);

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
                    throw new EntityNotFoundException<Product>(orderItem.ProductId.Value);
                }

                product.IncreaseQuantity(orderItem.Quantity);
            }

            await stripePaymentProvider.RefundOrderAsync(order, cancellationToken);

            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            await emailService.SendEmailAsync(new CancelOrderEmailTemplate(user.Email, order), cancellationToken);
            return Unit.Value;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}