namespace Shoply.WebApi.Features.Products.Endpoints.GetProducts;

public sealed record ProductsResponse(
    Guid Id,
    string Name,
    decimal Price,
    string ImageUrl);