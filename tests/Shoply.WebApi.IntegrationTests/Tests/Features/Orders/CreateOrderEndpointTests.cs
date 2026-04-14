using Shoply.WebApi.Features.Orders.Endpoints.CreateOrder;
using Shoply.WebApi.Features.Orders.Models;
using Shoply.WebApi.Features.Products.Models;
using Shoply.WebApi.Features.Users.Models;

namespace Shoply.WebApi.IntegrationTests.Tests.Features.Orders;

public sealed class CreateOrderEndpointTests(TestingFixture fixture) : TestingBase(fixture)
{
    [Fact]
    public async Task CreateOrder_Should_Return401_When_Unauthenticated()
    {
        // Arrange
        var unauthenticated = CreateUnauthenticatedClient();
        var command = new CreateOrderCommand();

        // Act
        var response = await unauthenticated.CreateOrderAsync(command, CurrentCancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateOrder_Should_Return400_When_BasketIsEmpty()
    {
        // Arrange
        var client = CreateAuthenticatedUserClient();
        var command = new CreateOrderCommand();

        // Act
        var response = await client.CreateOrderAsync(command, CurrentCancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync(CurrentCancellationToken);
        Assert.Contains("basket", content, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task CreateOrder_Should_Return201_When_OrderIsCreatedSuccessfully()
    {
        // Arrange
        var client = CreateAuthenticatedUserClient();
        await using var dbContext = GetDbContext();
        var userId = CurrentUserId;

        var user = await dbContext.Users
            .Include(u => u.Basket)
            .FirstAsync(u => u.Id == userId, CurrentCancellationToken);

        // Create and add product to basket
        var product = Product.Create(
            "Order Product",
            49.99m,
            "Product for order test",
            10,
            "https://example.com/order-product.jpg"
        );
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync(CurrentCancellationToken);

        user.Basket!.AddProduct(product);
        await dbContext.SaveChangesAsync(CurrentCancellationToken);

        var command = new CreateOrderCommand();

        // Act
        var response = await client.CreateOrderAsync(command, CurrentCancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        // Verify order was created
        var createdOrder = await dbContext.Set<Order>()
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.UserId == userId, CurrentCancellationToken);

        Assert.NotNull(createdOrder);
        Assert.Equal(userId, createdOrder.UserId);
        Assert.NotEmpty(createdOrder.OrderItems);

        // Verify basket is empty
        var updatedUser = await dbContext.Users
            .AsNoTracking()
            .Include(u => u.Basket)
            .ThenInclude(b => b.BasketItems)
            .FirstAsync(u => u.Id == userId, CurrentCancellationToken);
        Assert.Empty(updatedUser.Basket!.BasketItems);
    }

    [Fact]
    public async Task CreateOrder_Should_Create_OrderItems_With_Correct_Details()
    {
        // Arrange
        var client = CreateAuthenticatedUserClient();
        await using var dbContext = GetDbContext();
        var userId = CurrentUserId;

        var user = await dbContext.Users
            .Include(u => u.Basket)
            .FirstAsync(u => u.Id == userId, CurrentCancellationToken);

        // Create multiple products and add to basket
        var product1 = Product.Create(
            "Product 1",
            19.99m,
            "Description 1",
            15,
            "https://example.com/product1.jpg"
        );
        var product2 = Product.Create(
            "Product 2",
            29.99m,
            "Description 2",
            20,
            "https://example.com/product2.jpg"
        );

        dbContext.Products.Add(product1);
        dbContext.Products.Add(product2);
        await dbContext.SaveChangesAsync(CurrentCancellationToken);

        // Add to basket: product1 twice (qty=2), product2 once (qty=1)
        user.Basket!.AddProduct(product1);
        user.Basket!.AddProduct(product1);
        user.Basket!.AddProduct(product2);
        await dbContext.SaveChangesAsync(CurrentCancellationToken);

        var command = new CreateOrderCommand();

        // Act
        var response = await client.CreateOrderAsync(command, CurrentCancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var createdOrder = await dbContext.Set<Order>()
            .AsNoTracking()
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.UserId == userId, CurrentCancellationToken);

        Assert.NotNull(createdOrder);
        Assert.Equal(2, createdOrder.OrderItems.Count); // 2 unique products

        var item1 = createdOrder.OrderItems.First(oi => oi.ProductId == product1.Id);
        var item2 = createdOrder.OrderItems.First(oi => oi.ProductId == product2.Id);

        Assert.Equal(2, item1.Quantity);
        Assert.Equal(19.99m, item1.UnitPrice);
        Assert.Equal(product1.Name, item1.ProductName);

        Assert.Equal(1, item2.Quantity);
        Assert.Equal(29.99m, item2.UnitPrice);
        Assert.Equal(product2.Name, item2.ProductName);
    }

    [Fact]
    public async Task CreateOrder_Should_Decrease_Product_Quantity()
    {
        // Arrange
        var client = CreateAuthenticatedUserClient();
        await using var dbContext = GetDbContext();
        var userId = CurrentUserId;

        var user = await dbContext.Users
            .Include(u => u.Basket)
            .FirstAsync(u => u.Id == userId, CurrentCancellationToken);

        // Create product with 10 items
        var product = Product.Create(
            "Stock Product",
            9.99m,
            "Product with stock",
            10,
            "https://example.com/stock-product.jpg"
        );
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync(CurrentCancellationToken);

        var productId = product.Id;

        // Add 3 items to basket
        user.Basket!.AddProduct(product);
        user.Basket!.AddProduct(product);
        user.Basket!.AddProduct(product);
        await dbContext.SaveChangesAsync(CurrentCancellationToken);

        var command = new CreateOrderCommand();

        // Act
        var response = await client.CreateOrderAsync(command, CurrentCancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        // Verify product quantity was decreased by 3
        var updatedProduct = await dbContext.Products
            .AsNoTracking()
            .FirstAsync(p => p.Id == productId, CurrentCancellationToken);

        Assert.Equal(7, updatedProduct.Quantity); // 10 - 3 = 7
    }

    [Fact]
    public async Task CreateOrder_Should_Return400_When_Product_Out_Of_Stock()
    {
        // Arrange
        var client = CreateAuthenticatedUserClient();
        var dbContext = GetDbContext();
        var userId = CurrentUserId;

        var user = await dbContext.Users
            .Include(u => u.Basket)
            .FirstAsync(u => u.Id == userId, CurrentCancellationToken);

        // Create product with only 2 items
        var product = Product.Create(
            "Limited Stock Product",
            15.99m,
            "Product with limited stock",
            2,
            "https://example.com/limited-product.jpg"
        );
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync(CurrentCancellationToken);

        // Add 3 items to basket (more than stock)
        user.Basket!.AddProduct(product);
        user.Basket!.AddProduct(product);
        user.Basket!.AddProduct(product);
        await dbContext.SaveChangesAsync(CurrentCancellationToken);

        var command = new CreateOrderCommand();

        // Act
        var response = await client.CreateOrderAsync(command, CurrentCancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync(CurrentCancellationToken);
        Assert.Contains("stock", content, StringComparison.OrdinalIgnoreCase);

        // Verify basket still has items (transaction rolled back)
        var userAfter = await dbContext.Users
            .Include(u => u.Basket)
            .FirstAsync(u => u.Id == userId, CurrentCancellationToken);

        Assert.NotEmpty(userAfter.Basket!.BasketItems);

        // Verify product quantity wasn't changed
        var productAfter = await dbContext.Products
            .AsNoTracking()
            .FirstAsync(p => p.Id == product.Id, CurrentCancellationToken);

        Assert.Equal(2, productAfter.Quantity); // unchanged
    }

    [Fact]
    public async Task CreateOrder_Should_Prevent_Other_Users_Modifying_Orders()
    {
        // Arrange
        var client1 = CreateAuthenticatedUserClient();
        var dbContext = GetDbContext();

        var user1Id = CurrentUserId;
        var user1 = await dbContext.Users
            .Include(u => u.Basket)
            .FirstAsync(u => u.Id == user1Id, CurrentCancellationToken);

        // Create second user
        var user2 = User.Create("test@example2.com", "auth0|user2");
        dbContext.Users.Add(user2);
        await dbContext.SaveChangesAsync(CurrentCancellationToken);

        // Create product
        var product = Product.Create(
            "Shared Product",
            24.99m,
            "Product shared between users",
            10,
            "https://example.com/shared-product.jpg"
        );
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync(CurrentCancellationToken);

        // User 1 creates order
        user1.Basket!.AddProduct(product);
        await dbContext.SaveChangesAsync(CurrentCancellationToken);
        var response1 = await client1.CreateOrderAsync(new CreateOrderCommand(), CurrentCancellationToken);
        Assert.Equal(HttpStatusCode.Created, response1.StatusCode);

        // Verify user1 has 1 order
        var user1Orders = await dbContext.Set<Order>()
            .AsNoTracking()
            .Where(o => o.UserId == user1Id)
            .ToListAsync(CurrentCancellationToken);

        Assert.Single(user1Orders);

        // Verify user2 has 0 orders
        var user2Orders = await dbContext.Set<Order>()
            .AsNoTracking()
            .Where(o => o.UserId == user2.Id)
            .ToListAsync(CurrentCancellationToken);

        Assert.Empty(user2Orders);
    }

    [Fact]
    public async Task CreateOrder_Should_Create_Payment_With_Correct_Amount()
    {
        // Arrange
        var client = CreateAuthenticatedUserClient();
        var dbContext = GetDbContext();
        var userId = CurrentUserId;

        var user = await dbContext.Users
            .Include(u => u.Basket)
            .FirstAsync(u => u.Id == userId, CurrentCancellationToken);

        // Create product
        var product = Product.Create(
            "Payment Test Product",
            50.00m,
            "Product for payment test",
            10,
            "https://example.com/payment-product.jpg"
        );
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync(CurrentCancellationToken);

        // Add product to basket
        user.Basket!.AddProduct(product);
        await dbContext.SaveChangesAsync(CurrentCancellationToken);

        var command = new CreateOrderCommand();

        // Act
        var response = await client.CreateOrderAsync(command, CurrentCancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        // Verify order and payment were created
        var createdOrder = await dbContext.Set<Order>()
            .Include(o => o.Payment)
            .FirstOrDefaultAsync(o => o.UserId == userId, CurrentCancellationToken);

        Assert.NotNull(createdOrder);
        Assert.NotNull(createdOrder.Payment);
        Assert.Equal(50.00m, createdOrder.Payment.Amount);
        Assert.Equal(PaymentStatus.Pending, createdOrder.Payment.Status);
        Assert.Null(createdOrder.Payment.StripePaymentIntentId);
        Assert.Equal(0, createdOrder.Payment.RefundedAmount);
    }

    [Fact]
    public async Task CreateOrder_Should_Create_Payment_With_Correct_Total_For_Multiple_Items()
    {
        // Arrange
        var client = CreateAuthenticatedUserClient();
        var dbContext = GetDbContext();
        var userId = CurrentUserId;

        var user = await dbContext.Users
            .Include(u => u.Basket)
            .FirstAsync(u => u.Id == userId, CurrentCancellationToken);

        // Create multiple products
        var product1 = Product.Create(
            "Product A",
            25.00m,
            "This is a description for Product A",
            10,
            "https://example.com/productA.jpg"
        );
        var product2 = Product.Create(
            "Product B",
            15.00m,
            "This is a description for Product B",
            10,
            "https://example.com/productB.jpg"
        );

        dbContext.Products.Add(product1);
        dbContext.Products.Add(product2);
        await dbContext.SaveChangesAsync(CurrentCancellationToken);

        // Add to basket: product1 (qty=2) = 50.00, product2 (qty=1) = 15.00, total = 65.00
        user.Basket!.AddProduct(product1);
        user.Basket!.AddProduct(product1);
        user.Basket!.AddProduct(product2);
        await dbContext.SaveChangesAsync(CurrentCancellationToken);

        var command = new CreateOrderCommand();

        // Act
        var response = await client.CreateOrderAsync(command, CurrentCancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var createdOrder = await dbContext.Set<Order>()
            .Include(o => o.Payment)
            .FirstOrDefaultAsync(o => o.UserId == userId, CurrentCancellationToken);

        Assert.NotNull(createdOrder);
        Assert.NotNull(createdOrder.Payment);
        Assert.Equal(65.00m, createdOrder.Payment.Amount);
    }
}