namespace Shoply.WebApi.Features.Products.Endpoints.GetProducts;

public sealed record GetProductsResponse(
    Guid Id,
    string Name,
    decimal Price,
    string ImageUrl);