namespace Api.Common.Domain.Orders;

public sealed record OrderItem(ProductId ProductId, string ProductName, decimal UnitPrice, int Quantity)
{
    public decimal TotalPrice => UnitPrice * Quantity;

    public static OrderItem From(Product product, int quantity)
    {
        return new OrderItem(product.Id, product.Name, product.Price, quantity);
    }
}