using Api.Common.Domain.Products;
using Vogen;

namespace Api.Common.Domain.Orders;

[ValueObject<Guid>]
public readonly partial struct OrderItemId;

public sealed class OrderItem : Entity<OrderItemId>
{
    private OrderItem()
    {
    } // For EF Core

    public OrderItem(ProductId productId, string productName, decimal unitPrice, int quantity)
    {
        Id = OrderItemId.From(Guid.CreateVersion7());
        ProductId = productId;
        ProductName = productName;
        UnitPrice = unitPrice;
        Quantity = quantity;
    }

    public ProductId ProductId { get; private set; }

    public string ProductName { get; private set; } = null!;

    public decimal UnitPrice { get; }

    public int Quantity { get; }

    public decimal TotalPrice => UnitPrice * Quantity;


    public static OrderItem From(Product product, int quantity)
    {
        return new OrderItem(product.Id, product.Name, product.Price, quantity);
    }
}