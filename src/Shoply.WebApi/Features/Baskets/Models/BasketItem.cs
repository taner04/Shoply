namespace Shoply.WebApi.Features.Baskets.Models;

public sealed class BasketItem(ProductId productId)
{
    public ProductId ProductId { get; init; } = productId;
    public Product Product { get; init; } = null!;

    public int Quantity { get; set; } = 1;
}