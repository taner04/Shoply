namespace UnitTests.Domain.Orders;

public sealed class OrderTests
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

    private static User CreateUser()
    {
        return User.Create("test@example.com", "auth0|123");
    }

    [Fact]
    public void Create_WithEmptyOrderItems_ShouldThrow()
    {
        var user = CreateUser();

        // Order.Create() now requires at least one item
        var ex = Assert.Throws<GuardException>(() =>
            Order.Create(user.Id, []));

        Assert.Contains("OrderItems collection cannot be null or empty.", ex.Message);
    }

    [Fact]
    public void Create_ShouldHaveSameUserId()
    {
        var user = CreateUser();
        var product = CreateProduct();
        var orderItem = new OrderItem(product.Id, product.Name, product.Price, 1);

        var order = Order.Create(user.Id, [orderItem]);

        Assert.Equal(user.Id, order.UserId);
    }

    [Fact]
    public void OrderId_ShouldBeUnique()
    {
        var user = CreateUser();
        var product = CreateProduct();
        var orderItem = new OrderItem(product.Id, product.Name, product.Price, 1);

        var order1 = Order.Create(user.Id, [orderItem]);
        var order2 = Order.Create(user.Id, [orderItem]);

        Assert.NotEqual(order1.Id, order2.Id);
    }

    [Fact]
    public void Create_ShouldCreateValidOrderStructure()
    {
        var user = CreateUser();
        var product = CreateProduct();
        var orderItem = new OrderItem(product.Id, product.Name, product.Price, 1);

        var order = Order.Create(user.Id, [orderItem]);

        // Validate order structure
        Assert.NotNull(order);
        Assert.NotEqual(Guid.Empty, order.Id.Value);
        Assert.Equal(user.Id, order.UserId);
        Assert.Single(order.OrderItems);
    }
}