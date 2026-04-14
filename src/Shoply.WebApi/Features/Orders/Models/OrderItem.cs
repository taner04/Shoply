namespace Shoply.WebApi.Features.Orders.Models;

public sealed record OrderItem(
    ProductId ProductId,
    string ProductName,
    string ProductDescription,
    decimal UnitPrice,
    int Quantity)
{
    public decimal TotalPrice => UnitPrice * Quantity;

    public static OrderItem From(Product product, int quantity) =>
        new(product.Id, product.Name, product.Description, product.Price, quantity);
}