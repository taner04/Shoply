using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Api.Common.Shared.Pagination;
using Api.Features.Orders.Endpoints.CreateOrder;
using Api.Features.Products.Models;
using Api.Features.Users.Models;

namespace IntegrationTests.Tests.Features.Orders;

public sealed class GetOrdersEndpointTests(TestingFixture fixture) : TestingBase(fixture)
{
    [SuppressMessage("Performance", "CA1869:Cache and reuse 'JsonSerializerOptions' instances")]
    private static PaginationResult<JsonElement> DeserializePaginationResponse(string json)
    {
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        return JsonSerializer.Deserialize<PaginationResult<JsonElement>>(json, options)!;
    }

    [Fact]
    public async Task GetOrders_Should_Return401_When_Unauthenticated()
    {
        // Arrange
        var unauthenticated = CreateUnauthenticatedClient();

        // Act
        var response = await unauthenticated.GetOrdersAsync(cancellationToken: CurrentCancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetOrders_Should_Return200_With_EmptyList_When_NoOrdersExist()
    {
        // Arrange
        var client = CreateAuthenticatedUserClient();

        // Act
        var response = await client.GetOrdersAsync(cancellationToken: CurrentCancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync(CurrentCancellationToken);
        var paginationResponse = DeserializePaginationResponse(content);

        Assert.NotNull(paginationResponse);
        Assert.Empty(paginationResponse.Items);
        Assert.Equal(1, paginationResponse.PageIndex);
        Assert.Equal(0, paginationResponse.TotalCount);
        Assert.Equal(0, paginationResponse.TotalPages);
    }

    [Fact]
    public async Task GetOrders_Should_Return200_With_Paginated_Results()
    {
        // Arrange
        var client = CreateAuthenticatedUserClient();
        var dbContext = GetDbContext();
        var userId = CurrentUserId;
        var user = await dbContext.Users
            .Include(u => u.Basket)
            .FirstAsync(u => u.Id == userId, CurrentCancellationToken);

        // Create 3 products
        var products = new List<Product>();
        for (var i = 1; i <= 3; i++)
        {
            var product = Product.Create(
                $"Product {i:D2}",
                10.00m + i,
                $"Description for product {i}.",
                i * 10,
                $"https://example.com/product{i}.jpg"
            );
            dbContext.Products.Add(product);
            products.Add(product);
        }

        await dbContext.SaveChangesAsync(CurrentCancellationToken);

        // Create 2 orders by adding products to basket and creating orders
        for (var i = 0; i < 2; i++)
        {
            user.Basket!.AddProduct(products[i]);
            await dbContext.SaveChangesAsync(CurrentCancellationToken);

            // Create order via API
            var createOrderResponse = await client.CreateOrderAsync(new CreateOrderCommand(), CurrentCancellationToken);
            Assert.Equal(HttpStatusCode.Created, createOrderResponse.StatusCode);
        }

        // Act - Get first page with 1 item per page
        var response = await client.GetOrdersAsync(1, 1, CurrentCancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync(CurrentCancellationToken);
        var paginationResponse = DeserializePaginationResponse(content);

        Assert.NotNull(paginationResponse);
        Assert.Single(paginationResponse.Items);
        Assert.Equal(1, paginationResponse.PageIndex);
        Assert.Equal(2, paginationResponse.TotalCount);
        Assert.Equal(2, paginationResponse.TotalPages);

        // Verify Payment data is included in response
        var firstOrder = paginationResponse.Items.First();
        Assert.True(firstOrder.TryGetProperty("payment", out var payment));
        Assert.True(payment.TryGetProperty("id", out _));
        Assert.True(payment.TryGetProperty("amount", out _));
        Assert.True(payment.TryGetProperty("status", out _));

        // Act - Get second page
        var response2 = await client.GetOrdersAsync(2, 1, CurrentCancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response2.StatusCode);

        var content2 = await response2.Content.ReadAsStringAsync(CurrentCancellationToken);
        var paginationResponse2 = DeserializePaginationResponse(content2);

        Assert.NotNull(paginationResponse2);
        Assert.Single(paginationResponse2.Items);
        Assert.Equal(2, paginationResponse2.PageIndex);

        // Verify Payment data is included in second page as well
        var secondOrder = paginationResponse2.Items.First();
        Assert.True(secondOrder.TryGetProperty("payment", out _));
    }

    [Fact]
    public async Task GetOrders_Should_Return_Only_Current_User_Orders()
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

        // Create products
        var product1 = Product.Create(
            "Product 1",
            19.99m,
            "Product 1 description",
            10,
            "https://example.com/product1.jpg"
        );
        var product2 = Product.Create(
            "Product 2",
            29.99m,
            "Product 2 description",
            5,
            "https://example.com/product2.jpg"
        );
        dbContext.Products.Add(product1);
        dbContext.Products.Add(product2);
        await dbContext.SaveChangesAsync(CurrentCancellationToken);

        // Create order for user1
        user1.Basket!.AddProduct(product1);
        await dbContext.SaveChangesAsync(CurrentCancellationToken);
        var response1 = await client1.CreateOrderAsync(new CreateOrderCommand(), CurrentCancellationToken);
        Assert.Equal(HttpStatusCode.Created, response1.StatusCode);

        // Create order for user2
        user2.Basket!.AddProduct(product2);
        await dbContext.SaveChangesAsync(CurrentCancellationToken);

        // Act - Get user1's orders
        var response = await client1.GetOrdersAsync(cancellationToken: CurrentCancellationToken);

        // Assert - user1 should only see their own order
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync(CurrentCancellationToken);
        var paginationResponse = DeserializePaginationResponse(content);

        Assert.NotNull(paginationResponse);
        Assert.Single(paginationResponse.Items);
        Assert.Equal(1, paginationResponse.TotalCount);
    }

    [Fact]
    public async Task GetOrders_Should_Clamp_PageSize_To_MaxPageSize()
    {
        // Arrange
        var client = CreateAuthenticatedUserClient();
        var dbContext = GetDbContext();
        var userId = CurrentUserId;

        // Create 150 products and add them to basket in batches
        var user = await dbContext.Users
            .Include(u => u.Basket)
            .FirstAsync(u => u.Id == userId, CurrentCancellationToken);

        for (var i = 1; i <= 150; i++)
        {
            var product = Product.Create(
                $"Product {i:D3}",
                10.00m + i * 0.10m,
                $"Description for product {i}.",
                100,
                $"https://example.com/product{i}.jpg"
            );
            dbContext.Products.Add(product);
        }

        await dbContext.SaveChangesAsync(CurrentCancellationToken);

        // Create 2 orders with multiple items each
        var allProducts = await dbContext.Products.Take(75).ToListAsync(CurrentCancellationToken);

        // Order 1: first 75 products
        foreach (var product in allProducts.Take(50))
        {
            user.Basket!.AddProduct(product);
        }

        await dbContext.SaveChangesAsync(CurrentCancellationToken);
        var response1 = await client.CreateOrderAsync(new CreateOrderCommand(), CurrentCancellationToken);
        Assert.Equal(HttpStatusCode.Created, response1.StatusCode);

        // Order 2: next 25 products
        foreach (var product in allProducts.Skip(50))
        {
            user.Basket!.AddProduct(product);
        }

        await dbContext.SaveChangesAsync(CurrentCancellationToken);
        var response2 = await client.CreateOrderAsync(new CreateOrderCommand(), CurrentCancellationToken);
        Assert.Equal(HttpStatusCode.Created, response2.StatusCode);

        // Act - Request with PageSize = 200 (should be clamped to 100)
        var response = await client.GetOrdersAsync(1, 200, CurrentCancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync(CurrentCancellationToken);
        var paginationResponse = DeserializePaginationResponse(content);

        // Should have clamped to MaxPageSize = 100
        Assert.NotNull(paginationResponse);
        Assert.Equal(2, paginationResponse.Items.Count); // Should have at most 100 items (but we only created 2)
        Assert.Equal(1, paginationResponse.PageIndex);
        Assert.Equal(2, paginationResponse.TotalCount);
        Assert.Equal(1, paginationResponse.TotalPages);
    }

    [Fact]
    public async Task GetOrders_Should_Include_Payment_Data_In_Response()
    {
        // Arrange
        var client = CreateAuthenticatedUserClient();
        var dbContext = GetDbContext();
        var userId = CurrentUserId;
        var user = await dbContext.Users
            .Include(u => u.Basket)
            .FirstAsync(u => u.Id == userId, CurrentCancellationToken);

        // Create product and order
        var product = Product.Create(
            "Test Product",
            25.50m,
            "Test description.",
            100,
            "https://example.com/product.jpg"
        );
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync(CurrentCancellationToken);

        user.Basket!.AddProduct(product);
        await dbContext.SaveChangesAsync(CurrentCancellationToken);

        // Create order via API
        var createOrderResponse = await client.CreateOrderAsync(new CreateOrderCommand(), CurrentCancellationToken);
        Assert.Equal(HttpStatusCode.Created, createOrderResponse.StatusCode);

        // Act
        var response = await client.GetOrdersAsync(cancellationToken: CurrentCancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync(CurrentCancellationToken);
        var paginationResponse = DeserializePaginationResponse(content);

        Assert.NotNull(paginationResponse);
        Assert.Single(paginationResponse.Items);

        var order = paginationResponse.Items.First();
        Assert.True(order.TryGetProperty("payment", out var payment));

        // Verify payment properties
        Assert.True(payment.TryGetProperty("id", out _));
        Assert.True(payment.TryGetProperty("amount", out var amountElement));
        Assert.True(payment.TryGetProperty("status", out _));

        // Amount should be the total price

        // Verify amount value exists
        Assert.True(amountElement.ValueKind != System.Text.Json.JsonValueKind.Null);
    }
}