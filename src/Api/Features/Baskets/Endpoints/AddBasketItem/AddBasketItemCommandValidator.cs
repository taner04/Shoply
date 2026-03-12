using FluentValidation;

namespace Api.Features.Baskets.Endpoints.AddBasketItem;

public sealed class AddBasketItemCommandValidator : AbstractValidator<AddBasketItemCommand>
{
    public AddBasketItemCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty();
    }
}