using Shoply.WebApi.Common.Infrastructure.Persistence.Extensions;
using Shoply.WebApi.Common.Infrastructure.Services;
using Shoply.WebApi.Common.Infrastructure.Services.Emails;
using Shoply.WebApi.Common.Shared.Guards;
using Shoply.WebApi.Features.Orders.Exceptions;
using Shoply.WebApi.Features.Products.Exceptions;

namespace Shoply.WebApi.Features.Orders.Endpoints.CreateOrder;

public sealed class CreateOrderCommandHandler(
    ShoplyDbContext context,
    CurrentUserService userService,
    StripePaymentProvider stripePaymentProvider,
    EmailService emailService) : ICommandHandler<CreateOrderCommand, string>
{
    public async ValueTask<string> Handle(CreateOrderCommand command, CancellationToken cancellationToken)
    {
        var userId = userService.GetCurrentUserId();

        var user = await context.Users
                       .WithOrdersAndBasket(userId)
                       .FirstOrDefaultAsync(cancellationToken)
                   ?? throw new EntityNotFoundException<User>(userId.Value);

        if (user.Basket.BasketItems.Count == 0)
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

                Guard.Against.NegativeOrZero<Product>(basketItem.Quantity);
                if (basketItem.Quantity > product.Quantity)
                {
                    throw new InsufficientProductStockException(product.Id, product.Quantity, basketItem.Quantity);
                }

                product.Quantity -= basketItem.Quantity;
                orderItems.Add(new OrderItem(product.Id, product.Name, product.Description, product.Price,
                    basketItem.Quantity));
            }

            var newOrder = new Order(user.Id, orderItems);

            var stripeCheckoutSession =
                await stripePaymentProvider.CreateCheckoutSessionAsync(newOrder, user.Email, cancellationToken);

            newOrder.Payment.PaymentIntentId = stripeCheckoutSession.PaymentIntentId;

            user.Orders.Add(newOrder);
            user.Basket.BasketItems.Clear();

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