using Api.Common.Abstractions;
using Api.Common.Composition.Extensions;
using Api.Common.Infrastructure.Persistence;
using Api.Common.Infrastructure.Services;
using Api.Common.Shared.Exceptions;
using FluentValidation;
using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace Api.Features.Baskets.Endpoints;

public sealed record DeleteBasketItem(Guid ProductId) : ICommand;

public sealed class DeleteBasketItemEndpoint : IEndpoint
{
    public void MapEndpoint(WebApplication app)
    {
        app.MapDelete("/baskets/items", async ([FromBody] DeleteBasketItem command, [FromServices] IMediator mediator) =>
            {
                await mediator.Send(command);
                return Results.NoContent();
            })
            .WithName("DeleteBasketItem")
            .WithTags("Baskets")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesApiProblemDetails();
    }
}

public sealed class DeleteBasketItemCommandHandler(ApplicationDbContext context, CurrentUserService userService) : ICommandHandler<DeleteBasketItem>
{
    public async ValueTask<Unit> Handle(DeleteBasketItem command, CancellationToken cancellationToken)
    {
        var userId = userService.GetCurrentUserId();
        var user = await context.Users.Where(u => u.Id == userId).Include(u => u.Basket).ThenInclude(b => b.BasketItems).FirstOrDefaultAsync(cancellationToken) ?? throw new EntityNotFoundException<User>(userId);
        
        var productId = ProductId.From(command.ProductId);
        user.Basket.RemoveProduct(productId);
        
        context.Update(user);
        await context.SaveChangesAsync(cancellationToken);
        
        return  Unit.Value;
    }
}

public sealed  class DeleteBasketItemCommandValidator : AbstractValidator<DeleteBasketItem>
{
    public DeleteBasketItemCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty();
    }
}