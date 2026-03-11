using Api.Features.Baskets.Endpoints;
using Api.Features.Products.Endpoints;

namespace IntegrationTests.Tests.Features.Baskets;

public sealed class AddBasketItemEndpointTests(TestingFixture fixture) : TestingBase(fixture)
{
    private static CreateProductCommand ValidProductCommand => new(
        "Test Product",
        19.99m,
        "Test product description",
        10,
        "https://example.com/product.jpg"
    );

    [Fact]
    public async Task AddBasketItem_Should_Return401_When_Unauthenticated()
    {
        // Arrange
        var unauthenticated = CreateUnauthenticatedClient();
        var command = new AddBasketItemCommand(Guid.NewGuid());

        // Act
        var response = await unauthenticated.AddBasketItemAsync(command, CurrentCancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task AddBasketItem_Should_Return204_When_ProductIsAddedToBasket()
    {
        // Arrange
        var client = CreateAuthenticatedUserClient();
        var dbContext = GetDbContext();

        // Create a product first
        var product = Api.Common.Domain.Products.Product.Create(
            "Test Product",
            19.99m,
            "Test description",
            10,
            "https://example.com/product.jpg"
        );
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync(CurrentCancellationToken);

        var command = new AddBasketItemCommand(product.Id.Value);

        // Act
        var response = await client.AddBasketItemAsync(command, CurrentCancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        // Verify basket was updated
        var user = await dbContext.Users
            .AsNoTracking()
            .Include(u => u.Basket)
            .ThenInclude(b => b.BasketItems)
            .FirstAsync(u => u.Id == CurrentUserId, CurrentCancellationToken);

        var basketItem = Assert.Single(user.Basket.BasketItems);
        Assert.Equal(product.Id, basketItem.ProductId);
        Assert.Equal(1, basketItem.Quantity);
    }

    [Fact]
    public async Task AddBasketItem_Should_Return400_When_ProductIsOutOfStock()
    {
        // Arrange
        var client = CreateAuthenticatedUserClient();
        var dbContext = GetDbContext();

        // Create an out-of-stock product
        var outOfStockProduct = Api.Common.Domain.Products.Product.Create(
            "Out of Stock Product",
            29.99m,
            "Out of stock",
            0, // No quantity
            "https://example.com/oos.jpg"
        );
        dbContext.Products.Add(outOfStockProduct);
        await dbContext.SaveChangesAsync(CurrentCancellationToken);

        var command = new AddBasketItemCommand(outOfStockProduct.Id.Value);

        // Act
        var response = await client.AddBasketItemAsync(command, CurrentCancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        // Verify basket is still empty
        var user = await dbContext.Users
            .AsNoTracking()
            .Include(u => u.Basket)
            .ThenInclude(b => b.BasketItems)
            .FirstAsync(u => u.Id == CurrentUserId, CurrentCancellationToken);

        Assert.Empty(user.Basket.BasketItems);
    }

    [Fact]
    public async Task AddBasketItem_Should_Return404_When_ProductDoesNotExist()
    {
        // Arrange
        var client = CreateAuthenticatedUserClient();
        var nonExistentProductId = Guid.NewGuid();
        var command = new AddBasketItemCommand(nonExistentProductId);

        // Act
        var response = await client.AddBasketItemAsync(command, CurrentCancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task AddBasketItem_Should_IncreaseQuantity_When_AddingSameProductTwice()
    {
        // Arrange
        var client = CreateAuthenticatedUserClient();
        var dbContext = GetDbContext();

        var product = Api.Common.Domain.Products.Product.Create(
            "Test Product",
            19.99m,
            "Test description",
            10,
            "https://example.com/product.jpg"
        );
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync(CurrentCancellationToken);

        var command = new AddBasketItemCommand(product.Id.Value);

        // Act - Add product first time
        var firstResponse = await client.AddBasketItemAsync(command, CurrentCancellationToken);
        Assert.Equal(HttpStatusCode.NoContent, firstResponse.StatusCode);

        // Add product second time
        var secondResponse = await client.AddBasketItemAsync(command, CurrentCancellationToken);
        Assert.Equal(HttpStatusCode.NoContent, secondResponse.StatusCode);

        // Assert
        var user = await dbContext.Users
            .AsNoTracking()
            .Include(u => u.Basket)
            .ThenInclude(b => b.BasketItems)
            .FirstAsync(u => u.Id == CurrentUserId, CurrentCancellationToken);

        var basketItem = Assert.Single(user.Basket.BasketItems);
        Assert.Equal(product.Id, basketItem.ProductId);
        Assert.Equal(2, basketItem.Quantity);
    }
}