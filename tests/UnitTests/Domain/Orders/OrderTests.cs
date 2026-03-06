using Api.Common.Domain.Baskets;
using Api.Common.Domain.Orders;
using Api.Common.Domain.Products;
using Api.Common.Domain.Users;

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
    public void FromBasket_WithEmptyBasket_ShouldCreateOrderWithoutItems()
    {
        var user = CreateUser();
        var basket = Basket.CreateEmpty(user.Id);

        var order = Order.FromBasket(basket);

        Assert.Empty(order.OrderItems);
        Assert.Equal(basket.UserId, order.UserId);
    }

    [Fact]
    public void FromBasket_ShouldHaveSameUserIdAsBasket()
    {
        var user = CreateUser();
        var basket = Basket.CreateEmpty(user.Id);

        var order = Order.FromBasket(basket);

        Assert.Equal(basket.UserId, order.UserId);
    }

    [Fact]
    public void OrderId_ShouldBeUnique()
    {
        var user = CreateUser();
        var basket1 = Basket.CreateEmpty(user.Id);
        var basket2 = Basket.CreateEmpty(user.Id);

        var order1 = Order.FromBasket(basket1);
        var order2 = Order.FromBasket(basket2);

        Assert.NotEqual(order1.Id, order2.Id);
    }

    [Fact]
    public void FromBasket_ShouldCreateValidOrderStructure()
    {
        var user = CreateUser();
        var basket = Basket.CreateEmpty(user.Id);

        var order = Order.FromBasket(basket);

        // Validate order structure
        Assert.NotNull(order);
        Assert.NotEqual(Guid.Empty, order.Id.Value);
        Assert.Equal(user.Id, order.UserId);
    }
}