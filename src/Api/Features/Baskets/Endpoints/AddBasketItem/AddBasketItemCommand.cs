namespace Api.Features.Baskets.Endpoints.AddBasketItem;

public sealed record AddBasketItemCommand(Guid ProductId) : ICommand, IUserRequest;