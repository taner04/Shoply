using Api.Common.Abstractions;
using Api.Common.Composition.Extensions;
using Api.Common.Infrastructure.Persistence;
using Api.Common.Shared.Exceptions;
using FluentValidation;
using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace Api.Features.Products.Endpoints;

public sealed record UpdateProductCommand(
    Guid ProductId,
    string Name,
    decimal Price,
    string? Description,
    int Stock,
    string ImageUrl) : ICommand;

public sealed class UpdateProductEndpoint : IEndpoint
{
    public void MapEndpoint(WebApplication app)
    {
        app.MapPut("/products/{productId:guid}", async (Guid productId, [FromBody] UpdateProductCommand command,
                [FromServices] IMediator mediator) =>
            {
                command = command with { ProductId = productId };
                await mediator.Send(command);
                return Results.Ok();
            })
            .WithName("UpdateProduct")
            .WithTags("Products")
            .Produces(StatusCodes.Status200OK)
            .ProducesApiProblemDetails();
    }
}

public sealed class UpdateProductHandler(ApplicationDbContext context) : ICommandHandler<UpdateProductCommand>
{
    public async ValueTask<Unit> Handle(UpdateProductCommand command, CancellationToken cancellationToken)
    {
        var product =
            await context.Products.FirstOrDefaultAsync(p => p.Id == ProductId.From(command.ProductId),
                cancellationToken) ?? throw new EntityNotFoundException<Product>(command.ProductId);

        product.Update(command.Name, command.Price, command.Description, command.Stock, command.ImageUrl);

        context.Update(product);
        await context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}

public sealed class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotNull()
            .NotEmpty()
            .Length(ProductRules.MinNameLength, ProductRules.NameMaxLength);

        RuleFor(x => x.Price)
            .GreaterThan(0);

        RuleFor(x => x.Description)
            .MaximumLength(ProductRules.MaxDescriptionMaxLength)
            .When(x => !string.IsNullOrWhiteSpace(x.Description));

        RuleFor(x => x.Stock)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.ImageUrl)
            .NotNull()
            .NotEmpty()
            .Must(IsValidHttpUrl);
    }

    private static bool IsValidHttpUrl(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out var uri) &&
               (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
    }
}