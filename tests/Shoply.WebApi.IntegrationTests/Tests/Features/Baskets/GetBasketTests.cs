using System.Text.Json;
using Shoply.WebApi.Features.Baskets.Endpoints.GetBasket;
using Shoply.WebApi.Features.Products.Models;

namespace Shoply.WebApi.IntegrationTests.Tests.Features.Baskets;

public sealed class GetBasketTests(TestingFixture fixture) : TestingBase(fixture)
{
    [Fact]
    public async Task GetBasket_Should_Return401_When_Unauthenticated()
    {
        // Arrange
        var unauthenticated = CreateUnauthenticatedClient();
        var query = new GetBasketQuery();

        // Act
        var response = await unauthenticated.GetBasketAsync(query, CurrentCancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetBasket_Should_Return200_With_EmptyBasket()
    {
        // Arrange
        var client = CreateAuthenticatedUserClient();
        var query = new GetBasketQuery();

        // Act
        var response = await client.GetBasketAsync(query, CurrentCancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync(CurrentCancellationToken);
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var basketDto = JsonSerializer.Deserialize<BasketResponse>(content, options);

        Assert.NotNull(basketDto);
        Assert.Empty(basketDto.Items);
        Assert.Equal(0m, basketDto.TotalPrice);
    }

    [Fact]
    public async Task GetBasket_Should_Return200_With_BasketItems()
    {
        // Arrange
        var client = CreateAuthenticatedUserClient();
        await using var dbContext = GetDbContext();

        // Create products
        var product1 = Product.Create(
            "Test Product 1",
            19.99m,
            "Product 1 description",
            10,
            "https://example.com/product1.jpg"
        );
        var product2 = Product.Create(
            "Test Product 2",
            29.99m,
            "Product 2 description",
            5,
            "https://example.com/product2.jpg"
        );
        dbContext.Products.Add(product1);
        dbContext.Products.Add(product2);
        await dbContext.SaveChangesAsync(CurrentCancellationToken);

        // Add items to user's basket
        var user = await dbContext.Users
            .Include(u => u.Basket)
            .FirstAsync(u => u.Id == CurrentUserId, CurrentCancellationToken);

        user.Basket!.AddProduct(product1);
        user.Basket!.AddProduct(product1); // Add second time to get quantity of 2
        user.Basket!.AddProduct(product2);
        await dbContext.SaveChangesAsync(CurrentCancellationToken);

        var query = new GetBasketQuery();

        // Act
        var response = await client.GetBasketAsync(query, CurrentCancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync(CurrentCancellationToken);
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var basketDto = JsonSerializer.Deserialize<BasketResponse>(content, options);

        Assert.NotNull(basketDto);
        Assert.Equal(2, basketDto.Items.Count);

        // Verify first product
        var item1 = basketDto.Items.First(i => i.ProductName == "Test Product 1");
        Assert.Equal(product1.Id.Value, item1.ProductId);
        Assert.Equal(19.99m, item1.Price);
        Assert.Equal(2, item1.Quantity);
        Assert.Equal("https://example.com/product1.jpg", item1.ImageUrl);

        // Verify second product
        var item2 = basketDto.Items.First(i => i.ProductName == "Test Product 2");
        Assert.Equal(product2.Id.Value, item2.ProductId);
        Assert.Equal(29.99m, item2.Price);
        Assert.Equal(1, item2.Quantity);
        Assert.Equal("https://example.com/product2.jpg", item2.ImageUrl);

        // Verify total price: (19.99 * 2) + (29.99 * 1) = 39.98 + 29.99 = 69.97
        Assert.Equal(69.97m, basketDto.TotalPrice);
    }

    [Fact]
    public async Task GetBasket_Should_Return_Only_Current_User_Basket()
    {
        // Arrange
        var client = CreateAuthenticatedUserClient();
        await using var dbContext = GetDbContext();
        var userId = CurrentUserId;

        // Create a product
        var product = Product.Create(
            "Test Product",
            15.99m,
            "Product description",
            10,
            "https://example.com/product.jpg"
        );
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync(CurrentCancellationToken);

        // Add item to current user's basket
        var user = await dbContext.Users
            .Include(u => u.Basket)
            .FirstAsync(u => u.Id == userId, CurrentCancellationToken);

        user.Basket!.AddProduct(product);
        user.Basket!.AddProduct(product);
        user.Basket!.AddProduct(product); // Add three times to get quantity of 3
        await dbContext.SaveChangesAsync(CurrentCancellationToken);

        // Create another user and add product to their basket
        var otherUser = await CreateForeignUserAsync();
        var otherUserEntity = await dbContext.Users
            .AsNoTracking()
            .Include(u => u.Basket)
            .FirstAsync(u => u.Id == otherUser, CurrentCancellationToken);

        var query = new GetBasketQuery();

        // Act
        var response = await client.GetBasketAsync(query, CurrentCancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync(CurrentCancellationToken);
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var basketDto = JsonSerializer.Deserialize<BasketResponse>(content, options);

        Assert.NotNull(basketDto);
        Assert.Single(basketDto.Items);
        Assert.Equal(product.Id.Value, basketDto.Items[0].ProductId);
        Assert.Equal(3, basketDto.Items[0].Quantity);
        Assert.Equal(15.99m * 3, basketDto.TotalPrice);
    }
}