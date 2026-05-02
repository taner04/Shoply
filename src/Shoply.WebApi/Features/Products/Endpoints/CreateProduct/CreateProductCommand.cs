namespace Shoply.WebApi.Features.Products.Endpoints.CreateProduct;

public sealed record CreateProductCommand(
    string Name,
    decimal Price,
    string Description,
    int Stock,
    string ImageUrl) : ICommand, IUserRequest;