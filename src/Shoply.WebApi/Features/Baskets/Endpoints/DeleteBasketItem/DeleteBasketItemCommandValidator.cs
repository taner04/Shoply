using FluentValidation;

namespace Shoply.WebApi.Features.Baskets.Endpoints.DeleteBasketItem;

public sealed class DeleteBasketItemCommandValidator : AbstractValidator<DeleteBasketItemCommand>
{
    public DeleteBasketItemCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty();
    }
}