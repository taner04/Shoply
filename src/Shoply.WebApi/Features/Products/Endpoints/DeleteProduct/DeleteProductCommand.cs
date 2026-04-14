namespace Shoply.WebApi.Features.Products.Endpoints.DeleteProduct;

public sealed record DeleteProductCommand(ProductId ProductId) : ICommand, IUserRequest;