using Api.Common.Abstractions;
using Mediator;

namespace Api.Features.Orders.Endpoints.CreateOrder;

public record CreateOrderCommand : ICommand<string>, IUserRequest;