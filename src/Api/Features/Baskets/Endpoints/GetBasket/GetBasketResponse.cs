using System.Text.Json.Serialization;
using Api.Common.Domain.Baskets;

namespace Api.Features.Baskets.Endpoints.GetBasket;

public class BasketResponse
{
    [JsonConstructor]
    public BasketResponse(Guid id, List<BasketItemResponse> items)
    {
        Id = id;
        Items = items;
    }

    public Guid Id { get; }
    public List<BasketItemResponse> Items { get; }
    public decimal TotalPrice => Items.Sum(i => i.Price * i.Quantity);

    public static BasketResponse FromBasket(Basket basket)
    {
        return new BasketResponse(basket.Id.Value, basket.BasketItems.Select(bi => new BasketItemResponse(
            bi.Product.Id.Value,
            bi.Product.Name,
            bi.Product.Price,
            bi.Quantity,
            bi.Product.ImageUrl)).ToList());
    }
}

public sealed record BasketItemResponse(
    Guid ProductId,
    string ProductName,
    decimal Price,
    int Quantity,
    string ImageUrl);