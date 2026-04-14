using Shoply.WebApi.Features.Products.Endpoints.UpdateProduct;
using Shoply.WebApi.Features.Products.Models;
using ProductId = Shoply.WebApi.Features.Products.Models.ProductId;

namespace Shoply.WebApi.IntegrationTests.Tests.Features.Products;

public sealed class UpdateProductEndpointTests(TestingFixture fixture) : TestingBase(fixture)
{
    private static UpdateProductCommand ValidCommand => new(
        ProductId.From(Guid.Empty), // Will be set by the test
        "Updated Product",
        29.99m,
        "This is an updated product description.",
        20,
        "https://example.com/updated-product.jpg"
    );

    [Fact]
    public async Task UpdateProduct_Should_Return401_When_Unauthenticated()
    {
        // Arrange
        var unauthenticated = CreateUnauthenticatedClient();
        var productId = ProductId.From(Guid.NewGuid());

        // Act
        var response = await unauthenticated.UpdateProductAsync(productId, ValidCommand, CurrentCancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task UpdateProduct_Should_Return404_When_ProductDoesNotExist()
    {
        // Arrange
        var client = CreateAuthenticatedUserClient();
        var nonExistentProductId = ProductId.From(Guid.NewGuid());
        var command = ValidCommand with { ProductId = nonExistentProductId };

        // Act
        var response = await client.UpdateProductAsync(nonExistentProductId, command, CurrentCancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateProduct_Should_Return200_When_ProductIsUpdated()
    {
        // Arrange
        var client = CreateAuthenticatedUserClient();
        await using var dbContext = GetDbContext();

        // Create a product first
        var product = Product.Create(
            "Original Product",
            19.99m,
            "Original description.",
            10,
            "https://example.com/original.jpg"
        );
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync(CurrentCancellationToken);

        var updateCommand = new UpdateProductCommand(
            product.Id,
            "Updated Product",
            29.99m,
            "Updated description.",
            20,
            "https://example.com/updated.jpg"
        );

        // Act
        var response = await client.UpdateProductAsync(product.Id, updateCommand, CurrentCancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var updatedProduct = await dbContext.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == product.Id, CurrentCancellationToken);

        Assert.NotNull(updatedProduct);
        Assert.Equal("Updated Product", updatedProduct.Name);
        Assert.Equal(29.99m, updatedProduct.Price);
        Assert.Equal("Updated description.", updatedProduct.Description);
        Assert.Equal(20, updatedProduct.Quantity);
        Assert.Equal("https://example.com/updated.jpg", updatedProduct.ImageUrl);
    }

    [Fact]
    public async Task UpdateProduct_Should_Return400_When_NameIsTooShort()
    {
        // Arrange
        var client = CreateAuthenticatedUserClient();
        await using var dbContext = GetDbContext();

        var product = Product.Create(
            "Original Product",
            19.99m,
            "Description.",
            10,
            "https://example.com/product.jpg"
        );
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync(CurrentCancellationToken);

        var command = new UpdateProductCommand(
            product.Id,
            "AB", // Less than MinNameLength = 3
            9.99m,
            "Valid description.",
            10,
            "https://example.com/product.jpg"
        );

        // Act
        var response = await client.UpdateProductAsync(product.Id, command, CurrentCancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateProduct_Should_Return400_When_NameIsTooLong()
    {
        // Arrange
        var client = CreateAuthenticatedUserClient();
        var dbContext = GetDbContext();

        var product = Product.Create(
            "Original Product",
            19.99m,
            "Description.",
            10,
            "https://example.com/product.jpg"
        );
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync(CurrentCancellationToken);

        var tooLongName = new string('x', 101); // NameMaxLength = 100
        var command = new UpdateProductCommand(
            product.Id,
            tooLongName,
            9.99m,
            "Valid description.",
            10,
            "https://example.com/product.jpg"
        );

        // Act
        var response = await client.UpdateProductAsync(product.Id, command, CurrentCancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateProduct_Should_Return400_When_PriceIsZero()
    {
        // Arrange
        var client = CreateAuthenticatedUserClient();
        var dbContext = GetDbContext();

        var product = Product.Create(
            "Original Product",
            19.99m,
            "Description.",
            10,
            "https://example.com/product.jpg"
        );
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync(CurrentCancellationToken);

        var command = new UpdateProductCommand(
            product.Id,
            "Product",
            0m, // Must be > 0
            "Valid description.",
            10,
            "https://example.com/product.jpg"
        );

        // Act
        var response = await client.UpdateProductAsync(product.Id, command, CurrentCancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateProduct_Should_Return400_When_PriceIsNegative()
    {
        // Arrange
        var client = CreateAuthenticatedUserClient();
        var dbContext = GetDbContext();

        var product = Product.Create(
            "Original Product",
            19.99m,
            "Description.",
            10,
            "https://example.com/product.jpg"
        );
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync(CurrentCancellationToken);

        var command = new UpdateProductCommand(
            product.Id,
            "Product",
            -9.99m,
            "Valid description.",
            10,
            "https://example.com/product.jpg"
        );

        // Act
        var response = await client.UpdateProductAsync(product.Id, command, CurrentCancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateProduct_Should_Return400_When_StockIsNegative()
    {
        // Arrange
        var client = CreateAuthenticatedUserClient();
        var dbContext = GetDbContext();

        var product = Product.Create(
            "Original Product",
            19.99m,
            "Description.",
            10,
            "https://example.com/product.jpg"
        );
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync(CurrentCancellationToken);

        var command = new UpdateProductCommand(
            product.Id,
            "Product",
            9.99m,
            "Valid description.",
            -5, // Must be >= 0
            "https://example.com/product.jpg"
        );

        // Act
        var response = await client.UpdateProductAsync(product.Id, command, CurrentCancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateProduct_Should_Return400_When_DescriptionIsTooLong()
    {
        // Arrange
        var client = CreateAuthenticatedUserClient();
        var dbContext = GetDbContext();

        var product = Product.Create(
            "Original Product",
            19.99m,
            "Description.",
            10,
            "https://example.com/product.jpg"
        );
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync(CurrentCancellationToken);

        var tooLongDescription = new string('x', 501); // MaxDescriptionMaxLength = 500
        var command = new UpdateProductCommand(
            product.Id,
            "Product",
            9.99m,
            tooLongDescription,
            10,
            "https://example.com/product.jpg"
        );

        // Act
        var response = await client.UpdateProductAsync(product.Id, command, CurrentCancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateProduct_Should_Return400_When_ImageUrlIsInvalid()
    {
        // Arrange
        var client = CreateAuthenticatedUserClient();
        var dbContext = GetDbContext();

        var product = Product.Create(
            "Original Product",
            19.99m,
            "Description.",
            10,
            "https://example.com/product.jpg"
        );
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync(CurrentCancellationToken);

        var command = new UpdateProductCommand(
            product.Id,
            "Product",
            9.99m,
            "Valid description.",
            10,
            "invalid-url" // Not a valid HTTP URL
        );

        // Act
        var response = await client.UpdateProductAsync(product.Id, command, CurrentCancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}