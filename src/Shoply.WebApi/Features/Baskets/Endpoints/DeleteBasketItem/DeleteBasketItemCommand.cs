namespace Shoply.WebApi.Features.Baskets.Endpoints.DeleteBasketItem;

public sealed record DeleteBasketItemCommand(Guid ProductId) : ICommand, IUserRequest;