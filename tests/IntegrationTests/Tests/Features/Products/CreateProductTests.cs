using System.Net;
using Api.Features.Products.Endpoints;
using IntegrationTests.Infrastructure;
using IntegrationTests.Infrastructure.Fixtures;
using Microsoft.EntityFrameworkCore;

namespace IntegrationTests.Tests.Features.Products;

public sealed class CreateProductTests(TestingFixture fixture) : TestingBase(fixture)
{
    private static CreateProductCommand ValidCommand => new(
        Name: "Valid Product",
        Price: 19.99m,
        Description: "This is a valid product description.",
        Stock: 10,
        ImageUrl: "https://example.com/product.jpg"
    );

    [Fact]
    public async Task CreateProduct_Should_Return401_When_Unauthenticated()
    {
        // Arrange
        var unauthenticated = CreateUnauthenticatedClient();

        // Act
        var response = await unauthenticated.CreateProductAsync(ValidCommand, CurrentCancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateProduct_Should_Return200_When_ProductIsCreated()
    {
        // Arrange
        var client = CreateAuthenticatedUserClient();

        var command = new CreateProductCommand(
            Name: "New Product",
            Price: 29.99m,
            Description: "A brand new product.",
            Stock: 25,
            ImageUrl: "https://example.com/new-product.jpg"
        );

        // Act
        var response = await client.CreateProductAsync(command, CurrentCancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var dbContext = GetDbContext();
        var createdProduct = await dbContext.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Name == "New Product", CurrentCancellationToken);

        Assert.NotNull(createdProduct);
        Assert.Equal("New Product", createdProduct.Name);
        Assert.Equal(29.99m, createdProduct.Price);
        Assert.Equal("A brand new product.", createdProduct.Description);
        Assert.Equal(25, createdProduct.Quantity);
        Assert.Equal("https://example.com/new-product.jpg", createdProduct.ImageUrl);
    }

    [Fact]
    public async Task CreateProduct_Should_Return400_When_ProductNameAlreadyExists()
    {
        // Arrange
        var client = CreateAuthenticatedUserClient();

        var command = new CreateProductCommand(
            Name: "ExistingProduct",
            Price: 9.99m,
            Description: "Existing product description here.",
            Stock: 5,
            ImageUrl: "https://example.com/existing.jpg"
        );

        // Create the first product
        var firstResponse = await client.CreateProductAsync(command, CurrentCancellationToken);
        Assert.Equal(HttpStatusCode.OK, firstResponse.StatusCode);

        // Act - Try to create another product with the same name
        var secondResponse = await client.CreateProductAsync(command, CurrentCancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, secondResponse.StatusCode);
    }

    [Fact]
    public async Task CreateProduct_Should_Return400_When_NameIsTooShort()
    {
        // Arrange
        var client = CreateAuthenticatedUserClient();

        var command = new CreateProductCommand(
            Name: "AB", // Less than MinNameLength = 3
            Price: 9.99m,
            Description: "Valid description for testing.",
            Stock: 10,
            ImageUrl: "https://example.com/product.jpg"
        );

        // Act
        var response = await client.CreateProductAsync(command, CurrentCancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateProduct_Should_Return400_When_NameIsTooLong()
    {
        // Arrange
        var client = CreateAuthenticatedUserClient();

        var tooLongName = new string('x', 101); // NameMaxLength = 100

        var command = new CreateProductCommand(
            Name: tooLongName,
            Price: 9.99m,
            Description: "Valid description for testing.",
            Stock: 10,
            ImageUrl: "https://example.com/product.jpg"
        );

        // Act
        var response = await client.CreateProductAsync(command, CurrentCancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateProduct_Should_Return400_When_PriceIsZero()
    {
        // Arrange
        var client = CreateAuthenticatedUserClient();

        var command = new CreateProductCommand(
            Name: "Product",
            Price: 0m, // Must be > 0
            Description: "Valid description for testing.",
            Stock: 10,
            ImageUrl: "https://example.com/product.jpg"
        );

        // Act
        var response = await client.CreateProductAsync(command, CurrentCancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateProduct_Should_Return400_When_PriceIsNegative()
    {
        // Arrange
        var client = CreateAuthenticatedUserClient();

        var command = new CreateProductCommand(
            Name: "Product",
            Price: -9.99m,
            Description: "Valid description for testing.",
            Stock: 10,
            ImageUrl: "https://example.com/product.jpg"
        );

        // Act
        var response = await client.CreateProductAsync(command, CurrentCancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateProduct_Should_Return400_When_StockIsNegative()
    {
        // Arrange
        var client = CreateAuthenticatedUserClient();

        var command = new CreateProductCommand(
            Name: "Product",
            Price: 9.99m,
            Description: "Valid description for testing.",
            Stock: -5, // Must be >= 0
            ImageUrl: "https://example.com/product.jpg"
        );

        // Act
        var response = await client.CreateProductAsync(command, CurrentCancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateProduct_Should_Return400_When_DescriptionIsTooLong()
    {
        // Arrange
        var client = CreateAuthenticatedUserClient();

        var tooLongDescription = new string('x', 501); // MaxDescriptionMaxLength = 500

        var command = new CreateProductCommand(
            Name: "Product",
            Price: 9.99m,
            Description: tooLongDescription,
            Stock: 10,
            ImageUrl: "https://example.com/product.jpg"
        );

        // Act
        var response = await client.CreateProductAsync(command, CurrentCancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateProduct_Should_Return200_When_DescriptionIsEmpty()
    {
        // Arrange
        var client = CreateAuthenticatedUserClient();

        var command = new CreateProductCommand(
            Name: "Product Without Description",
            Price: 9.99m,
            Description: "", // Empty description is allowed by validator
            Stock: 10,
            ImageUrl: "https://example.com/product.jpg"
        );

        // Act
        var response = await client.CreateProductAsync(command, CurrentCancellationToken);

        // Assert
        Assert.True(response.IsSuccessStatusCode);
    }

    [Fact]
    public async Task CreateProduct_Should_Return400_When_ImageUrlIsInvalid()
    {
        // Arrange
        var client = CreateAuthenticatedUserClient();

        var command = new CreateProductCommand(
            Name: "Product",
            Price: 9.99m,
            Description: "Valid description for testing.",
            Stock: 10,
            ImageUrl: "invalid-url" // Not a valid HTTP URL
        );

        // Act
        var response = await client.CreateProductAsync(command, CurrentCancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateProduct_Should_Return200_When_DescriptionIsNull()
    {
        // Arrange
        var client = CreateAuthenticatedUserClient();

        var command = new CreateProductCommand(
            Name: "ProductWithNullDescription",
            Price: 9.99m,
            Description: null, // Description is optional
            Stock: 10,
            ImageUrl: "https://example.com/product.jpg"
        );

        // Act
        var response = await client.CreateProductAsync(command, CurrentCancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var dbContext = GetDbContext();
        var createdProduct = await dbContext.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Name == "ProductWithNullDescription", CurrentCancellationToken);

        Assert.NotNull(createdProduct);
        Assert.Null(createdProduct.Description);
    }
}