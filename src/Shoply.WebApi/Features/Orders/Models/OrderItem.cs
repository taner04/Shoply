namespace Shoply.WebApi.Features.Orders.Models;

public sealed class OrderItem(
    ProductId productId,
    string productName,
    string productDescription,
    decimal unitPrice,
    int quantity)
{
    public ProductId ProductId { get; init; } = productId;
    public Product Product { get; init; } = null!;

    public string ProductName { get; init; } = productName;
    public string ProductDescription { get; init; } = productDescription;
    public decimal UnitPrice { get; init; } = unitPrice;
    public int Quantity { get; init; } = quantity;

    public decimal TotalPrice => UnitPrice * Quantity;
}