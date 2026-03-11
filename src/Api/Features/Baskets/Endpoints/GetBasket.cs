using Api.Common.Abstractions;
using Api.Common.Composition.Extensions;
using Api.Common.Infrastructure.Persistence;
using Api.Common.Infrastructure.Services;
using Api.Common.Shared.Exceptions;
using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace Api.Features.Baskets.Endpoints;

public sealed record GetBasketQuery : IQuery<BasketDto>;

public sealed class GetBasketEndpoint : IEndpoint
{
    public void MapEndpoint(WebApplication app)
    {
        app.MapGet("/baskets", async ([FromServices] IMediator mediator) =>
            {
                var basket = await mediator.Send(new GetBasketQuery());
                return Results.Ok(basket);
            })
            .WithName("GetBasket")
            .WithTags("Baskets")
            .Produces<BasketDto>()
            .ProducesApiProblemDetails();
    }
}

public sealed class GetBasketQueryHandler(ApplicationDbContext context, CurrentUserService userService)
    : IQueryHandler<GetBasketQuery, BasketDto>
{
    public async ValueTask<BasketDto> Handle(GetBasketQuery _, CancellationToken cancellationToken)
    {
        var userId = userService.GetCurrentUserId();
        var user = await context.Users.Where(u => u.Id == userId).Include(u => u.Basket).ThenInclude(b => b.BasketItems)
                       .ThenInclude(bi => bi.Product).FirstOrDefaultAsync(cancellationToken) ??
                   throw new EntityNotFoundException<User>(userId);

        var basket = user.Basket;
        var basketItems = basket.BasketItems.Select(bi => new BasketItemDto(
            bi.ProductId.Value,
            bi.Product.Name,
            bi.Product.Price,
            bi.Quantity,
            bi.Product.ImageUrl)).ToList();

        return new BasketDto(basket.Id.Value, basketItems);
    }
}

public sealed record BasketDto(Guid Id, List<BasketItemDto> Items)
{
    public decimal TotalPrice => Items.Sum(i => i.Price * i.Quantity);
}

public sealed record BasketItemDto(Guid ProductId, string ProductName, decimal Price, int Quantity, string ImageUrl);