using Api.Common.Abstractions;
using Mediator;

namespace Api.Features.Products.Endpoints.DeleteProduct;

public sealed record DeleteProductCommand(Guid ProductId) : ICommand, IUserRequest;