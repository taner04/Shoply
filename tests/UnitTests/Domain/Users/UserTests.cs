using Api.Features.Orders.Models;
using Api.Features.Products.Models;
using Api.Features.Users.Models;

namespace UnitTests.Domain.Users;

public sealed class UserTests
{
    private static Product CreateProduct(string name = "Coffee Beans")
    {
        return Product.Create(
            name,
            12.99m,
            "Fresh roasted beans",
            10,
            "https://example.com/image.jpg");
    }

    [Fact]
    public void Create_WithValidEmail_ShouldCreateUser()
    {
        const string email = "test@example.com";
        const string auth0Id = "auth0|123";

        var user = User.Create(email, auth0Id);

        Assert.Equal(email, user.Email);
        Assert.Equal(auth0Id, user.Auth0Id);
    }

    [Fact]
    public void Create_ShouldNormalizeEmailToLowercase()
    {
        const string email = "TEST@EXAMPLE.COM";
        const string auth0Id = "auth0|123";

        var user = User.Create(email, auth0Id);

        Assert.Equal("test@example.com", user.Email);
    }

    [Fact]
    public void Create_ShouldTrimEmailWhitespace()
    {
        const string email = "  test@example.com  ";
        const string auth0Id = "auth0|123";

        var user = User.Create(email, auth0Id);

        Assert.Equal("test@example.com", user.Email);
    }

    [Fact]
    public void Create_WithInvalidEmail_ShouldThrow()
    {
        const string invalidEmail = "not-an-email";
        const string auth0Id = "auth0|123";

        var ex = Assert.Throws<GuardException>(() =>
            User.Create(invalidEmail, auth0Id));

        Assert.Equal("User.Email.InvalidEmail", ex.ErrorCode);
    }

    [Fact]
    public void Create_WithEmptyEmail_ShouldThrow()
    {
        const string email = "";
        const string auth0Id = "auth0|123";

        var ex = Assert.Throws<GuardException>(() =>
            User.Create(email, auth0Id));

        // Email with empty string will fail email validation
        Assert.Equal("User.Email.InvalidEmail", ex.ErrorCode);
    }

    [Fact]
    public void Create_WithWhitespaceEmail_ShouldNormalizeAndFail()
    {
        // Whitespace gets trimmed, resulting in empty string which fails validation
        const string email = "   ";
        const string auth0Id = "auth0|123";

        var ex = Assert.Throws<GuardException>(() =>
            User.Create(email, auth0Id));

        // After trim, empty string fails email validation
        Assert.Equal("User.Email.InvalidEmail", ex.ErrorCode);
    }

    [Fact]
    public void Create_WithEmptyAuth0Id_ShouldThrow()
    {
        const string email = "test@example.com";
        const string auth0Id = "";

        Assert.Throws<GuardException>(() =>
            User.Create(email, auth0Id));
    }

    [Fact]
    public void Create_ShouldInitializeWithEmptyBasket()
    {
        var user = User.Create("test@example.com", "auth0|123");

        Assert.NotNull(user.Basket);
        Assert.Empty(user.Basket.BasketItems);
    }

    [Fact]
    public void Create_ShouldInitializeBasketAsOwned()
    {
        var user = User.Create("test@example.com", "auth0|123");

        // Basket is owned by User, so it should exist but won't have separate FK
        Assert.NotNull(user.Basket);
        Assert.Empty(user.Basket.BasketItems);
    }

    [Fact]
    public void Create_ShouldInitializeWithEmptyOrders()
    {
        var user = User.Create("test@example.com", "auth0|123");

        Assert.Empty(user.Orders);
    }

    [Fact]
    public void AddOrder_ShouldAddOrderToUserOrders()
    {
        var user = User.Create("test@example.com", "auth0|123");
        var product = CreateProduct();
        var orderItem = new OrderItem(product.Id, product.Name, product.Description, product.Price, 1);

        var order = Order.Create(user.Id, [orderItem]);
        user.AddOrder(order);

        var addedOrder = Assert.Single(user.Orders);
        Assert.Equal(order.Id, addedOrder.Id);
    }

    [Fact]
    public void AddOrder_MultipleOrders_ShouldAddAllOrders()
    {
        var user = User.Create("test@example.com", "auth0|123");
        var product = CreateProduct();
        var orderItem = new OrderItem(product.Id, product.Name, product.Description, product.Price, 1);

        var order1 = Order.Create(user.Id, [orderItem]);
        var order2 = Order.Create(user.Id, [orderItem]);
        var order3 = Order.Create(user.Id, [orderItem]);

        user.AddOrder(order1);
        user.AddOrder(order2);
        user.AddOrder(order3);

        Assert.Equal(3, user.Orders.Count);
    }

    [Fact]
    public void Create_ShouldGenerateUniqueId()
    {
        var user1 = User.Create("user1@example.com", "auth0|1");
        var user2 = User.Create("user2@example.com", "auth0|2");

        Assert.NotEqual(user1.Id, user2.Id);
    }

    [Fact]
    public void Create_ShouldHaveNonEmptyId()
    {
        var user = User.Create("test@example.com", "auth0|123");

        // UserId should not be an empty Guid (represented as UserId.From(Guid.Empty))
        Assert.NotEqual(Guid.Empty, user.Id.Value);
    }

    [Fact]
    public void Orders_ShouldBeReadOnlyCollection()
    {
        var user = User.Create("test@example.com", "auth0|123");

        // IReadOnlyCollection doesn't allow direct modification
        var orders = user.Orders;
        Assert.IsAssignableFrom<IReadOnlyCollection<Order>>(orders);
    }

    [Fact]
    public void Basket_ShouldBeSeparateInstancePerUser()
    {
        var user1 = User.Create("user1@example.com", "auth0|1");
        var user2 = User.Create("user2@example.com", "auth0|2");

        var product = CreateProduct();
        user1.Basket.AddProduct(product);

        Assert.Single(user1.Basket.BasketItems);
        Assert.Empty(user2.Basket.BasketItems);
    }
}