namespace Shoply.WebApi.Features.Baskets.Endpoints.AddBasketItem;

public sealed record AddBasketItemCommand(ProductId ProductId) : ICommand, IUserRequest;