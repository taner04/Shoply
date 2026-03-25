namespace Api.Features.Orders.Endpoints.CreateOrder;

public record CreateOrderCommand : ICommand<string>, IUserRequest;