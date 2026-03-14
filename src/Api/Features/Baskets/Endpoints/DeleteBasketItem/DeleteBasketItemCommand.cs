using Api.Common.Abstractions;
using Mediator;

namespace Api.Features.Baskets.Endpoints.DeleteBasketItem;

public sealed record DeleteBasketItemCommand(Guid ProductId) : ICommand, IUserRequest;