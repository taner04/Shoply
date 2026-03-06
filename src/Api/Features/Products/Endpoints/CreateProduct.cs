using Api.Common.Abstractions;
using Api.Common.Composition.Extensions;
using Api.Common.Domain.Products;
using Api.Common.Infrastructure.Persistence;
using Api.Features.Products.Exceptions;
using FluentValidation;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Api.Features.Products.Endpoints;

public readonly record struct CreateProductCommand(
    string Name,
    decimal Price,
    string? Description,
    int Stock,
    string ImageUrl) : ICommand;

public class CreateProductEndpoint : IEndpoint
{
    public void MapEndpoint(WebApplication app)
    {
        app.MapPost("/products", async (CreateProductCommand command, IMediator mediator) =>
        {
            await mediator.Send(command);
            return Results.Ok();
        })
        .WithName("CreateProduct")
        .WithTags("Products")
        .Produces(StatusCodes.Status201Created)
        .ProducesApiProblemDetails();
    }
}

public sealed class CreateProductCommandHandler(ApplicationDbContext context) : ICommandHandler<CreateProductCommand>
{
    public async ValueTask<Unit> Handle(CreateProductCommand command, CancellationToken cancellationToken)
    {
        var doesSameNameExists = await context.Products
            .AnyAsync(p => p.Name.Equals(command.Name, StringComparison.OrdinalIgnoreCase), cancellationToken);

        if (doesSameNameExists)
        {
            throw new ProductNameAlreadyExistsException(command.Name);
        }

        var product = Product.Create(
            command.Name,
            command.Price,
            command.Description,
            command.Stock,
            command.ImageUrl);

        context.Products.Add(product);
        await context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}

public sealed class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
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