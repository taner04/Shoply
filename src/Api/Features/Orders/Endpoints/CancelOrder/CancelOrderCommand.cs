namespace Api.Features.Orders.Endpoints.CancelOrder;

public record CancelOrderCommand(Guid  OrderId) : ICommand;