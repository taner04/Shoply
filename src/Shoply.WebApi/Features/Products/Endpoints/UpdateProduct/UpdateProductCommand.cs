namespace Shoply.WebApi.Features.Products.Endpoints.UpdateProduct;

public sealed record UpdateProductCommand(
    ProductId ProductId,
    string Name,
    decimal Price,
    string Description,
    int Stock,
    string ImageUrl) : ICommand, IUserRequest;