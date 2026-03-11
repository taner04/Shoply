using Api.Common.Abstractions;
using Api.Common.Composition.Extensions;
using Api.Common.Infrastructure.Persistence;
using Api.Common.Infrastructure.Services;
using Api.Common.Shared.Exceptions;
using Api.Features.Baskets.Exceptions;
using FluentValidation;
using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace Api.Features.Baskets.Endpoints;

public sealed record AddBasketItemCommand(Guid ProductId) : ICommand;

public sealed class AddBasketItemEndpoint : IEndpoint
{
    public void MapEndpoint(WebApplication app)
    {
        app.MapPost("/baskets/items", async ([FromBody] AddBasketItemCommand command, [FromServices] IMediator mediator) =>
            {
                await mediator.Send(command);
                return Results.NoContent();
            })
            .WithName("AddBasketItem")
            .WithTags("Baskets")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesApiProblemDetails();
    }
}

public sealed class AddBasketItemCommandHandler(ApplicationDbContext context, CurrentUserService userService)
    : ICommandHandler<AddBasketItemCommand>
{
    public async ValueTask<Unit> Handle(AddBasketItemCommand command, CancellationToken cancellationToken)
    {
        var product = await context.Products.FirstOrDefaultAsync(p => p.Id == ProductId.From(command.ProductId), cancellationToken) ?? throw new EntityNotFoundException<Product>(command.ProductId);
        ProductOutOfStockException.ThrowIfOutOfStock(product);
        
        var userId = userService.GetCurrentUserId();
        var user = await context.Users.Where(u => u.Id == userId).Include(u => u.Basket).ThenInclude(b => b.BasketItems).FirstOrDefaultAsync(cancellationToken) ?? throw new EntityNotFoundException<User>(userId);
        
        user.Basket.AddProduct(product);
        context.Update(user);
        await context.SaveChangesAsync(cancellationToken);
        
        return Unit.Value;
    }
}

public sealed class AddBasketItemCommandValidator : AbstractValidator<AddBasketItemCommand>
{
    public AddBasketItemCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty();
    }
}