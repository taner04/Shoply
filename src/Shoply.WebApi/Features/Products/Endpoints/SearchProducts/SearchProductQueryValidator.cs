using FluentValidation;

namespace Shoply.WebApi.Features.Products.Endpoints.SearchProducts;

internal sealed class SearchProductQueryValidator : AbstractValidator<SearchProductQuery>
{
    public SearchProductQueryValidator()
    {
        RuleFor(query => query.Name).NotEmpty();
    }
}