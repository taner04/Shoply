using Api.Common.Abstractions;
using Api.Common.Composition.Extensions;
using Api.Common.Domain.Products;
using Api.Common.Infrastructure.Persistence;
using Api.Common.Shared.Pagination;
using FluentValidation;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Features.Products.Endpoints;

public record GetProductsQuery(int PageIndex, int PageSize) : PaginationQuery(PageIndex, PageSize), IQuery<PaginationResponse<Product>>;

public class GetProductsEndpoint : IEndpoint
{
    public void MapEndpoint(WebApplication app)
    {
        app.MapGet("/products", async ([FromServices] IMediator mediator, [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10) =>
        {
            var query = new GetProductsQuery(pageIndex, pageSize);
            return await mediator.Send(query);
        })
        .WithName("GetProducts")
        .WithTags("Products")
        .Produces<PaginationResponse<Product>>()
        .ProducesApiProblemDetails();
    }
}

public sealed class GetProductsQueryHandler(ApplicationDbContext context) : IQueryHandler<GetProductsQuery, PaginationResponse<Product>>
{
    private const int MaxPageSize = 100;

    public async ValueTask<PaginationResponse<Product>> Handle(GetProductsQuery query, CancellationToken cancellationToken)
    {
        // Validate pagination parameters
        var pageIndex = Math.Max(1, query.PageIndex);
        var pageSize = Math.Clamp(query.PageSize, 1, MaxPageSize);

        // Get total count and paginated products in one efficient query
        var totalCount = await context.Products.CountAsync(cancellationToken);
        
        var products = await context.Products
            .OrderBy(p => p.Name)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
        
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        
        return new PaginationResponse<Product>(products, pageIndex, totalPages, totalCount);
    }
}

public sealed class GetProductsQueryValidator : AbstractValidator<GetProductsQuery>
{
    public GetProductsQueryValidator()
    {
        RuleFor(x => x.PageIndex)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Page index must be at least 1");

        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Page size must be at least 1")
            .LessThanOrEqualTo(100)
            .WithMessage("Page size cannot exceed 100");
    }
}
