using Shoply.WebApi.Features.Baskets.Endpoints.AddBasketItem;
using Shoply.WebApi.Features.Baskets.Endpoints.DeleteBasketItem;
using Shoply.WebApi.Features.Products.Models;

namespace Shoply.WebApi.IntegrationTests.Tests.Features.Baskets;

public sealed class DeleteBasketItemEndpointTests(TestingFixture fixture) : TestingBase(fixture)
{
    [Fact]
    public async Task DeleteBasketItem_Should_Return401_When_Unauthenticated()
    {
        // Arrange
        var unauthenticated = CreateUnauthenticatedClient();
        var command = new DeleteBasketItemCommand(Guid.NewGuid());

        // Act
        var response = await unauthenticated.DeleteBasketItemAsync(command, CurrentCancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task DeleteBasketItem_Should_Return204_When_ItemIsRemovedFromBasket()
    {
        // Arrange
        var client = CreateAuthenticatedUserClient();
        await using var dbContext = GetDbContext();

        // Create a product
        var product = Product.Create(
            "Test Product",
            19.99m,
            "Test description",
            10,
            "https://example.com/product.jpg"
        );
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync(CurrentCancellationToken);

        var productFromDb = await dbContext.Products
            .AsNoTracking()
            .FirstAsync(p => p.Id == product.Id, CurrentCancellationToken);

        // Add product to basket via endpoint
        var addCommand = new AddBasketItemCommand(product.Id);
        var addResponse = await client.AddBasketItemAsync(addCommand, CurrentCancellationToken);
        Assert.Equal(HttpStatusCode.NoContent, addResponse.StatusCode);

        var deleteCommand = new DeleteBasketItemCommand(product.Id.Value);

        // Act
        var response = await client.DeleteBasketItemAsync(deleteCommand, CurrentCancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        // Verify basket is empty now
        var updatedUser = await dbContext.Users
            .AsNoTracking()
            .Include(u => u.Basket)
            .ThenInclude(b => b.BasketItems)
            .FirstAsync(u => u.Id == CurrentUserId, CurrentCancellationToken);

        Assert.Empty(updatedUser.Basket.BasketItems);
    }

    [Fact]
    public async Task DeleteBasketItem_Should_Return404_When_ProductNotInBasket()
    {
        // Arrange
        var client = CreateAuthenticatedUserClient();
        var nonExistentProductId = Guid.NewGuid();
        var command = new DeleteBasketItemCommand(nonExistentProductId);

        // Act
        var response = await client.DeleteBasketItemAsync(command, CurrentCancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteBasketItem_Should_DecreaseQuantity_When_QuantityIsGreaterThanOne()
    {
        // Arrange
        var client = CreateAuthenticatedUserClient();
        await using var dbContext = GetDbContext();

        var product = Product.Create(
            "Test Product",
            19.99m,
            "Test description",
            10,
            "https://example.com/product.jpg"
        );
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync(CurrentCancellationToken);

        // Add product twice to basket via endpoint
        var addCommand = new AddBasketItemCommand(product.Id);
        await client.AddBasketItemAsync(addCommand, CurrentCancellationToken);
        await client.AddBasketItemAsync(addCommand, CurrentCancellationToken);

        var deleteCommand = new DeleteBasketItemCommand(product.Id.Value);

        // Act
        var response = await client.DeleteBasketItemAsync(deleteCommand, CurrentCancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        // Verify quantity decreased
        var updatedUser = await dbContext.Users
            .AsNoTracking()
            .Include(u => u.Basket)
            .ThenInclude(b => b.BasketItems)
            .FirstAsync(u => u.Id == CurrentUserId, CurrentCancellationToken);

        var basketItem = Assert.Single(updatedUser.Basket.BasketItems);
        Assert.Equal(product.Id, basketItem.ProductId);
        Assert.Equal(1, basketItem.Quantity);
    }

    [Fact]
    public async Task DeleteBasketItem_Should_RemoveItemEntirely_When_QuantityIsOne()
    {
        // Arrange
        var client = CreateAuthenticatedUserClient();
        await using var dbContext = GetDbContext();

        var product = Product.Create(
            "Test Product",
            19.99m,
            "Test description",
            10,
            "https://example.com/product.jpg"
        );
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync(CurrentCancellationToken);

        // Add product once via endpoint
        var addCommand = new AddBasketItemCommand(product.Id);
        await client.AddBasketItemAsync(addCommand, CurrentCancellationToken);

        var deleteCommand = new DeleteBasketItemCommand(product.Id.Value);

        // Act
        var response = await client.DeleteBasketItemAsync(deleteCommand, CurrentCancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        // Verify item removed entirely
        var updatedUser = await dbContext.Users
            .AsNoTracking()
            .Include(u => u.Basket)
            .ThenInclude(b => b.BasketItems)
            .FirstAsync(u => u.Id == CurrentUserId, CurrentCancellationToken);

        Assert.Empty(updatedUser.Basket.BasketItems);
    }
}