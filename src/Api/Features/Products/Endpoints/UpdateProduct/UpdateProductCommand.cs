using Api.Common.Abstractions;
using Mediator;

namespace Api.Features.Products.Endpoints.UpdateProduct;

public sealed record UpdateProductCommand(
    Guid ProductId,
    string Name,
    decimal Price,
    string? Description,
    int Stock,
    string ImageUrl) : ICommand, IUserRequest;