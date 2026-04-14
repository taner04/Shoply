namespace Shoply.WebApi.Features.Orders.Endpoints.CreateOrder;

public record CreateOrderCommand : ICommand<string>, IUserRequest;