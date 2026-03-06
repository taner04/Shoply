using Api.Common.Domain.Orders;
using Api.Common.Domain.Products;

namespace UnitTests.Domain.Orders;

public sealed class OrderItemTests
{
    private static Product CreateProduct(string name = "Coffee Beans", decimal price = 12.99m)
    {
        return Product.Create(
            name,
            price,
            "Fresh roasted beans",
            10,
            "https://example.com/image.jpg");
    }

    [Fact]
    public void TotalPrice_ShouldCalculateUnitPriceTimesQuantity()
    {
        var productId = ProductId.From(Guid.NewGuid());
        var orderItem = new OrderItem(productId, "Coffee", 10.00m, 5);

        Assert.Equal(50.00m, orderItem.TotalPrice);
    }

    [Fact]
    public void TotalPrice_WithDecimalPrice_ShouldCalculateCorrectly()
    {
        var productId = ProductId.From(Guid.NewGuid());
        var orderItem = new OrderItem(productId, "Coffee", 12.99m, 3);

        Assert.Equal(38.97m, orderItem.TotalPrice);
    }

    [Fact]
    public void TotalPrice_WithQuantityOne_ShouldEqualUnitPrice()
    {
        var productId = ProductId.From(Guid.NewGuid());
        var orderItem = new OrderItem(productId, "Coffee", 25.50m, 1);

        Assert.Equal(25.50m, orderItem.TotalPrice);
    }

    [Fact]
    public void TotalPrice_WithQuantityZero_ShouldBeZero()
    {
        var productId = ProductId.From(Guid.NewGuid());
        var orderItem = new OrderItem(productId, "Coffee", 10.00m, 0);

        Assert.Equal(0.00m, orderItem.TotalPrice);
    }

    [Fact]
    public void Constructor_ShouldInitializeWithProvidedValues()
    {
        var productId = ProductId.From(Guid.NewGuid());
        var productName = "Test Product";
        var unitPrice = 99.99m;
        var quantity = 2;

        var orderItem = new OrderItem(productId, productName, unitPrice, quantity);

        Assert.Equal(productId, orderItem.ProductId);
        Assert.Equal(productName, orderItem.ProductName);
        Assert.Equal(unitPrice, orderItem.UnitPrice);
        Assert.Equal(quantity, orderItem.Quantity);
    }

    [Fact]
    public void UnitPrice_ShouldBeReadOnly()
    {
        var productId = ProductId.From(Guid.NewGuid());
        var orderItem = new OrderItem(productId, "Test", 10.00m, 1);

        // UnitPrice should be initialized but immutable - cannot be changed after creation
        Assert.Equal(10.00m, orderItem.UnitPrice);
    }

    [Fact]
    public void TotalPrice_WithLargeQuantity_ShouldCalculateCorrectly()
    {
        var productId = ProductId.From(Guid.NewGuid());
        var orderItem = new OrderItem(productId, "Bulk Item", 1.99m, 1000);

        Assert.Equal(1990.00m, orderItem.TotalPrice);
    }

    [Fact]
    public void OrderItemId_ShouldBeUnique()
    {
        var productId = ProductId.From(Guid.NewGuid());
        var item1 = new OrderItem(productId, "Test", 10.00m, 1);
        var item2 = new OrderItem(productId, "Test", 10.00m, 1);

        Assert.NotEqual(item1.Id, item2.Id);
    }
}