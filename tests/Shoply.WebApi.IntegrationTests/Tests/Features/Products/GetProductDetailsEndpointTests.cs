using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Shoply.WebApi.Features.Products.Endpoints.GetProductDetails;
using Shoply.WebApi.Features.Products.Models;

namespace Shoply.WebApi.IntegrationTests.Tests.Features.Products;

public sealed class GetProductDetailsEndpointTests(TestingFixture fixture) : TestingBase(fixture)
{
    [SuppressMessage("Performance", "CA1869:Cache and reuse \'JsonSerializerOptions\' instances")]
    private static GetProductDetailsResponse DeserializeResponse(string json)
    {
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        return JsonSerializer.Deserialize<GetProductDetailsResponse>(json, options)!;
    }

    [Fact]
    public async Task GetProductDetails_Should_Return200_With_All_Fields()
    {
        // Arrange
        var client = CreateAuthenticatedUserClient();
        await using var dbContext = GetDbContext();

        var product = Product.Create(
            "Premium Ethiopian Coffee",
            29.99m,
            "High-quality single-origin coffee beans from the highlands of Ethiopia",
            250,
            "https://example.com/coffee-premium.jpg"
        );

        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync(CurrentCancellationToken);

        // Act
        var response = await client.GetProductDetailsAsync(product.Id, CurrentCancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync(CurrentCancellationToken);
        var result = DeserializeResponse(content);

        Assert.NotNull(result);
        Assert.Equal(product.Id.Value, result.Id);
        Assert.Equal("Premium Ethiopian Coffee", result.Name);
        Assert.Equal(29.99m, result.Price);
        Assert.Equal("High-quality single-origin coffee beans from the highlands of Ethiopia", result.Description);
        Assert.Equal(250, result.Quantity);
        Assert.Equal("https://example.com/coffee-premium.jpg", result.ImageUrl);
    }

    [Fact]
    public async Task GetProductDetails_Should_Return404_When_ProductDoesNotExist()
    {
        // Arrange
        var client = CreateAuthenticatedUserClient();
        var nonExistentProductId = ProductId.From(Guid.NewGuid());

        // Act
        var response = await client.GetProductDetailsAsync(nonExistentProductId, CurrentCancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetProductDetails_Should_Return_ZeroQuantity_When_OutOfStock()
    {
        // Arrange
        var client = CreateAuthenticatedUserClient();
        await using var dbContext = GetDbContext();

        var product = Product.Create(
            "Out of Stock Product",
            15.50m,
            "This product is currently out of stock",
            0,
            "https://example.com/out-of-stock.jpg"
        );

        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync(CurrentCancellationToken);

        var productId = product.Id.Value;

        // Act
        var response = await client.GetProductDetailsAsync(ProductId.From(productId), CurrentCancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync(CurrentCancellationToken);
        var result = DeserializeResponse(content);

        Assert.NotNull(result);
        Assert.Equal(0, result.Quantity);
        Assert.Equal("Out of Stock Product", result.Name);
    }

    [Fact]
    public async Task GetProductDetails_Should_Return_Correct_Data_For_Each_Product_Independently()
    {
        // Arrange
        var client = CreateAuthenticatedUserClient();
        var dbContext = GetDbContext();

        var product1 = Product.Create(
            "Product One",
            10.00m,
            "Description for product one",
            100,
            "https://example.com/product-one.jpg"
        );

        var product2 = Product.Create(
            "Product Two",
            20.00m,
            "Description for product two",
            200,
            "https://example.com/product-two.jpg"
        );

        var product3 = Product.Create(
            "Product Three",
            30.00m,
            "Description for product three",
            300,
            "https://example.com/product-three.jpg"
        );

        dbContext.Products.Add(product1);
        dbContext.Products.Add(product2);
        dbContext.Products.Add(product3);
        await dbContext.SaveChangesAsync(CurrentCancellationToken);

        // Act & Assert - Verify each product returns correct data
        var response1 = await client.GetProductDetailsAsync(product1.Id, CurrentCancellationToken);
        Assert.Equal(HttpStatusCode.OK, response1.StatusCode);
        var result1 = DeserializeResponse(await response1.Content.ReadAsStringAsync(CurrentCancellationToken));
        Assert.Equal("Product One", result1.Name);
        Assert.Equal(10.00m, result1.Price);
        Assert.Equal(100, result1.Quantity);

        var response2 = await client.GetProductDetailsAsync(product2.Id, CurrentCancellationToken);
        Assert.Equal(HttpStatusCode.OK, response2.StatusCode);
        var result2 = DeserializeResponse(await response2.Content.ReadAsStringAsync(CurrentCancellationToken));
        Assert.Equal("Product Two", result2.Name);
        Assert.Equal(20.00m, result2.Price);
        Assert.Equal(200, result2.Quantity);

        var response3 = await client.GetProductDetailsAsync(product3.Id, CurrentCancellationToken);
        Assert.Equal(HttpStatusCode.OK, response3.StatusCode);
        var result3 = DeserializeResponse(await response3.Content.ReadAsStringAsync(CurrentCancellationToken));
        Assert.Equal("Product Three", result3.Name);
        Assert.Equal(30.00m, result3.Price);
        Assert.Equal(300, result3.Quantity);
    }

    [Fact]
    public async Task GetProductDetails_Should_Return404_For_Deleted_Product()
    {
        // Arrange
        var client = CreateAuthenticatedUserClient();
        var dbContext = GetDbContext();

        var product = Product.Create(
            "Product to Delete",
            12.99m,
            "This product will be deleted",
            50,
            "https://example.com/will-delete.jpg"
        );

        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync(CurrentCancellationToken);

        // Verify product exists
        var response1 = await client.GetProductDetailsAsync(product.Id, CurrentCancellationToken);
        Assert.Equal(HttpStatusCode.OK, response1.StatusCode);

        // Delete the product from database
        dbContext.Products.Remove(product);
        await dbContext.SaveChangesAsync(CurrentCancellationToken);

        // Act - Try to get deleted product
        var response2 = await client.GetProductDetailsAsync(product.Id, CurrentCancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response2.StatusCode);
    }

    [Fact]
    public async Task GetProductDetails_Should_Return_All_Field_Types_Correctly()
    {
        // Arrange
        var client = CreateAuthenticatedUserClient();
        var dbContext = GetDbContext();

        var product = Product.Create(
            "Test Product",
            99.99m,
            "Test description with special characters: äöü €#@!",
            999,
            "https://example.com/test-product.jpg"
        );

        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync(CurrentCancellationToken);

        // Act
        var response = await client.GetProductDetailsAsync(product.Id, CurrentCancellationToken);

        // Assert - Verify field types and values
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync(CurrentCancellationToken);
        var result = DeserializeResponse(content);

        // Verify each field type
        Assert.IsType<Guid>(result.Id);
        Assert.IsType<string>(result.Name);
        Assert.IsType<decimal>(result.Price);
        Assert.IsType<string>(result.Description);
        Assert.IsType<int>(result.Quantity);
        Assert.IsType<string>(result.ImageUrl);

        // Verify exact values
        Assert.Equal(product.Id.Value, result.Id);
        Assert.Equal("Test Product", result.Name);
        Assert.Equal(99.99m, result.Price);
        Assert.Equal("Test description with special characters: äöü €#@!", result.Description);
        Assert.Equal(999, result.Quantity);
        Assert.Equal("https://example.com/test-product.jpg", result.ImageUrl);
    }
}