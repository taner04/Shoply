using Api.Common.Domain.Baskets;
using Api.Common.Domain.Orders;
using Api.Common.Domain.Products;
using Api.Common.Domain.Users;
using Api.Common.Shared.Exceptions;

namespace UnitTests.Domain.Users;

public sealed class UserTests
{
    private static Product CreateProduct(string name = "Coffee Beans")
        => Product.Create(
            name: name,
            price: 12.99m,
            description: "Fresh roasted beans",
            stock: 10,
            imageUrl: "https://example.com/image.jpg");

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
    public void Create_ShouldInitializeBasketWithUserIdAssignedCorrectly()
    {
        var user = User.Create("test@example.com", "auth0|123");

        Assert.Equal(user.Id, user.Basket.UserId);
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
        
        // Create an empty order for testing (doesn't require loaded products)
        var order = Order.FromBasket(user.Basket);
        user.AddOrder(order);

        var addedOrder = Assert.Single(user.Orders);
        Assert.Equal(order.Id, addedOrder.Id);
    }

    [Fact]
    public void AddOrder_MultipleOrders_ShouldAddAllOrders()
    {
        var user = User.Create("test@example.com", "auth0|123");

        var order1 = Order.FromBasket(Basket.CreateEmpty(user.Id));
        var order2 = Order.FromBasket(Basket.CreateEmpty(user.Id));
        var order3 = Order.FromBasket(Basket.CreateEmpty(user.Id));

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
        user1.Basket.AddProduct(BasketItem.From(product));

        Assert.Single(user1.Basket.BasketItems);
        Assert.Empty(user2.Basket.BasketItems);
    }
}
