using Shoply.WebApi.Common.Infrastructure.Persistence.Extensions;
using Shoply.WebApi.Common.Infrastructure.Services;
using Shoply.WebApi.Common.Infrastructure.Services.Emails;
using Shoply.WebApi.Features.Orders.Exceptions;

namespace Shoply.WebApi.Features.Orders.Endpoints.CreateOrder;

public sealed class CreateOrderCommandHandler(
    ShoplyDbContext context,
    CurrentUserService userService,
    StripePaymentProvider stripePaymentProvider,
    IEmailService emailService) : ICommandHandler<CreateOrderCommand, string>
{
    public async ValueTask<string> Handle(CreateOrderCommand command, CancellationToken cancellationToken)
    {
        var userId = userService.GetCurrentUserId();

        var user = await context.Users
                       .WithOrdersAndBasket(userId)
                       .FirstOrDefaultAsync(cancellationToken)
                   ?? throw new EntityNotFoundException<User>(userId.Value);

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
                    throw new EntityNotFoundException<Product>(basketItem.ProductId.Value);
                }

                product.DecreaseQuantity(basketItem.Quantity);
                orderItems.Add(OrderItem.From(product, basketItem.Quantity));
            }

            var newOrder = Order.Create(user.Id, orderItems);
            
            var stripeCheckoutSession = await stripePaymentProvider.CreateCheckoutSessionAsync(newOrder, user.Email, cancellationToken);
            newOrder.SetStripePaymentIntentId(stripeCheckoutSession.PaymentIntentId);
            
            user.AddOrder(newOrder);
            user.Basket.Clear();

            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            await emailService.SendEmailAsync(new CreateOrderEmailTemplate(user.Email, newOrder), cancellationToken);
            return stripeCheckoutSession.Url;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}