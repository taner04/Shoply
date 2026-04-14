namespace Shoply.WebApi.Features.Orders.Endpoints.CancelOrder;

public record CancelOrderCommand(OrderId OrderId) : ICommand;