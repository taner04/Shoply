using Api.Features.Orders.Exceptions;
using Api.Features.Orders.Models;
using Api.Features.Products.Models;
using Api.Features.Users.Models;

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
        var orderItem = new OrderItem(product.Id, product.Name, product.Description, product.Price, 1);

        var order = Order.Create(user.Id, [orderItem]);

        Assert.Equal(user.Id, order.UserId);
    }

    [Fact]
    public void OrderId_ShouldBeUnique()
    {
        var user = CreateUser();
        var product = CreateProduct();
        var orderItem = new OrderItem(product.Id, product.Name, product.Description, product.Price, 1);

        var order1 = Order.Create(user.Id, [orderItem]);
        var order2 = Order.Create(user.Id, [orderItem]);

        Assert.NotEqual(order1.Id, order2.Id);
    }

    [Fact]
    public void Create_ShouldCreateValidOrderStructure()
    {
        var user = CreateUser();
        var product = CreateProduct();
        var orderItem = new OrderItem(product.Id, product.Name, product.Description, product.Price, 1);

        var order = Order.Create(user.Id, [orderItem]);

        // Validate order structure
        Assert.NotNull(order);
        Assert.NotEqual(Guid.Empty, order.Id.Value);
        Assert.Equal(user.Id, order.UserId);
        Assert.Single(order.OrderItems);
    }

    [Fact]
    public void Create_ShouldInitializeWithPendingPaymentStatus()
    {
        var user = CreateUser();
        var product = CreateProduct();
        var orderItem = new OrderItem(product.Id, product.Name, product.Description, product.Price, 1);

        var order = Order.Create(user.Id, [orderItem]);

        Assert.Equal(OrderStatus.Pending, order.Status);
    }

    [Fact]
    public void Create_ShouldCreatePaymentWithCorrectAmount()
    {
        var user = CreateUser();
        var product1 = CreateProduct("Product 1", 10.00m);
        var product2 = CreateProduct("Product 2", 20.00m);

        var orderItem1 = new OrderItem(product1.Id, product1.Name, product1.Description, 10.00m, 2);
        var orderItem2 = new OrderItem(product2.Id, product2.Name, product2.Description, 20.00m, 1);

        var order = Order.Create(user.Id, [orderItem1, orderItem2]);

        Assert.NotNull(order.Payment);
        Assert.Equal(40.00m, order.Payment.Amount);
        Assert.Equal(PaymentStatus.Pending, order.Payment.Status);
    }

    [Fact]
    public void Create_ShouldLinkPaymentToOrder()
    {
        var user = CreateUser();
        var product = CreateProduct();
        var orderItem = new OrderItem(product.Id, product.Name, product.Description, product.Price, 1);

        var order = Order.Create(user.Id, [orderItem]);

        Assert.NotNull(order.Payment);
        Assert.Equal(order.Id, order.Payment.OrderId);
    }

    [Fact]
    public void TotalPrice_ShouldSumAllOrderItems()
    {
        var user = CreateUser();
        var product1 = CreateProduct("Product 1", 10.00m);
        var product2 = CreateProduct("Product 2", 20.00m);

        var orderItem1 = new OrderItem(product1.Id, product1.Name, product1.Description, 10.00m, 2);
        var orderItem2 = new OrderItem(product2.Id, product2.Name, product2.Description, 20.00m, 1);

        var order = Order.Create(user.Id, [orderItem1, orderItem2]);

        // (10 * 2) + (20 * 1) = 20 + 20 = 40
        Assert.Equal(40.00m, order.TotalPrice());
    }

    [Fact]
    public void MarkPaid_ShouldChangeOrderStatusToPaid()
    {
        var user = CreateUser();
        var product = CreateProduct();
        var orderItem = new OrderItem(product.Id, product.Name, product.Description, product.Price, 1);
        var order = Order.Create(user.Id, [orderItem]);

        order.MarkPaid();

        Assert.Equal(OrderStatus.Paid, order.Status);
    }

    [Fact]
    public void MarkPaid_WhenNotPending_ShouldThrow()
    {
        var user = CreateUser();
        var product = CreateProduct();
        var orderItem = new OrderItem(product.Id, product.Name, product.Description, product.Price, 1);
        var order = Order.Create(user.Id, [orderItem]);
        order.MarkPaid();

        var ex = Assert.Throws<InvalidOrderPaymentStatusException>(() => order.MarkPaid());
        Assert.NotNull(ex);
    }

    [Fact]
    public void MarkFailed_ShouldChangeOrderStatusToFailed()
    {
        var user = CreateUser();
        var product = CreateProduct();
        var orderItem = new OrderItem(product.Id, product.Name, product.Description, product.Price, 1);
        var order = Order.Create(user.Id, [orderItem]);

        order.MarkFailed();

        Assert.Equal(OrderStatus.Failed, order.Status);
    }

    [Fact]
    public void MarkCancelled_ShouldChangeOrderStatusToCancelled()
    {
        var user = CreateUser();
        var product = CreateProduct();
        var orderItem = new OrderItem(product.Id, product.Name, product.Description, product.Price, 1);
        var order = Order.Create(user.Id, [orderItem]);

        order.MarkCancelled();

        Assert.Equal(OrderStatus.Cancelled, order.Status);
    }

    [Fact]
    public void MarkCancelled_WhenPaymentSucceeded_ShouldThrow()
    {
        var user = CreateUser();
        var product = CreateProduct();
        var orderItem = new OrderItem(product.Id, product.Name, product.Description, product.Price, 1);
        var order = Order.Create(user.Id, [orderItem]);

        // Setup payment as succeeded
        order.Payment!.MarkProcessing();
        order.Payment.MarkSucceeded();

        var ex = Assert.Throws<InvalidOperationException>(() => order.MarkCancelled());
        Assert.Contains("Cannot cancel an order that has already been paid.", ex.Message);
    }

    [Fact]
    public void Create_ShouldGenerateIdempotencyKey()
    {
        var user = CreateUser();
        var product = CreateProduct();
        var orderItem = new OrderItem(product.Id, product.Name, product.Description, product.Price, 1);

        var order = Order.Create(user.Id, [orderItem]);

        Assert.NotNull(order.IdempotencyKey);
        Assert.Contains("order_", order.IdempotencyKey);
    }
}