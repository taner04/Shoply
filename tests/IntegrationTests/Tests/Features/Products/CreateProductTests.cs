using Api.Features.Products.Endpoints;

namespace IntegrationTests.Tests.Features.Products;

public sealed class CreateProductTests(TestingFixture fixture) : TestingBase(fixture)
{
    private static CreateProductCommand ValidCommand => new(
        "Valid Product",
        19.99m,
        "This is a valid product description.",
        10,
        "https://example.com/product.jpg"
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
            "New Product",
            29.99m,
            "A brand new product.",
            25,
            "https://example.com/new-product.jpg"
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
            "ExistingProduct",
            9.99m,
            "Existing product description here.",
            5,
            "https://example.com/existing.jpg"
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
            "AB", // Less than MinNameLength = 3
            9.99m,
            "Valid description for testing.",
            10,
            "https://example.com/product.jpg"
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
            tooLongName,
            9.99m,
            "Valid description for testing.",
            10,
            "https://example.com/product.jpg"
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
            "Product",
            0m, // Must be > 0
            "Valid description for testing.",
            10,
            "https://example.com/product.jpg"
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
            "Product",
            -9.99m,
            "Valid description for testing.",
            10,
            "https://example.com/product.jpg"
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
            "Product",
            9.99m,
            "Valid description for testing.",
            -5, // Must be >= 0
            "https://example.com/product.jpg"
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
            "Product",
            9.99m,
            tooLongDescription,
            10,
            "https://example.com/product.jpg"
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
            "Product Without Description",
            9.99m,
            "", // Empty description is allowed by validator
            10,
            "https://example.com/product.jpg"
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
            "Product",
            9.99m,
            "Valid description for testing.",
            10,
            "invalid-url" // Not a valid HTTP URL
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
            "ProductWithNullDescription",
            9.99m,
            null, // Description is optional
            10,
            "https://example.com/product.jpg"
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