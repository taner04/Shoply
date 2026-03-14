using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Api.Common.Shared.Pagination;
using Api.Features.Products.Models;

namespace IntegrationTests.Tests.Features.Products;

public sealed class GetProductsEndpointTests(TestingFixture fixture) : TestingBase(fixture)
{
    [SuppressMessage("Performance", "CA1869:Cache and reuse \'JsonSerializerOptions\' instances")]
    private static PaginationResult<JsonElement> DeserializePaginationResponse(string json)
    {
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        return JsonSerializer.Deserialize<PaginationResult<JsonElement>>(json, options)!;
    }

    [Fact]
    public async Task GetProducts_Should_Return200_With_EmptyList_When_NoProductsExist()
    {
        // Arrange
        var client = CreateAuthenticatedUserClient();

        // Act
        var response = await client.GetProductsAsync(cancellationToken: CurrentCancellationToken);

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
    public async Task GetProducts_Should_Return200_With_Paginated_Results()
    {
        // Arrange
        var client = CreateAuthenticatedUserClient();
        var dbContext = GetDbContext();

        // Create 5 products
        for (var i = 1; i <= 5; i++)
        {
            var product = Product.Create(
                $"Product {i:D2}",
                10.00m + i,
                $"Description for product {i}.",
                i * 10,
                $"https://example.com/product{i}.jpg"
            );
            dbContext.Products.Add(product);
        }

        await dbContext.SaveChangesAsync(CurrentCancellationToken);

        // Act - Get first page with 2 items per page
        var response = await client.GetProductsAsync(1, 2, CurrentCancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync(CurrentCancellationToken);
        var paginationResponse = DeserializePaginationResponse(content);

        Assert.NotNull(paginationResponse);
        Assert.Equal(2, paginationResponse.Items.Count);
        Assert.Equal(1, paginationResponse.PageIndex);
        Assert.Equal(5, paginationResponse.TotalCount);
        Assert.Equal(3, paginationResponse.TotalPages);
    }

    [Fact]
    public async Task GetProducts_Should_Return_Products_Sorted_By_Name()
    {
        // Arrange
        var client = CreateAuthenticatedUserClient();
        var dbContext = GetDbContext();

        // Create products with names out of order
        var productNames = new[] { "Zebra Product", "Apple Product", "Mango Product" };
        foreach (var name in productNames)
        {
            var product = Product.Create(
                name,
                9.99m,
                "Test description.",
                5,
                "https://example.com/product.jpg"
            );
            dbContext.Products.Add(product);
        }

        await dbContext.SaveChangesAsync(CurrentCancellationToken);

        // Act
        var response = await client.GetProductsAsync(1, 10, CurrentCancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync(CurrentCancellationToken);
        var paginationResponse = DeserializePaginationResponse(content);

        Assert.NotNull(paginationResponse);
        Assert.Equal(3, paginationResponse.Items.Count);
    }

    [Fact]
    public async Task GetProducts_Should_Clamp_PageSize_To_MaxPageSize()
    {
        // Arrange
        var client = CreateAuthenticatedUserClient();
        var dbContext = GetDbContext();

        // Create 150 products
        for (var i = 1; i <= 150; i++)
        {
            var product = Product.Create(
                $"Product {i:D3}",
                5.00m,
                "Test description.",
                1,
                "https://example.com/product.jpg"
            );
            dbContext.Products.Add(product);
        }

        await dbContext.SaveChangesAsync(CurrentCancellationToken);

        // Act - Request page size of 200 (should be clamped to 100)
        var response = await client.GetProductsAsync(1, 200, CurrentCancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync(CurrentCancellationToken);
        var paginationResponse = DeserializePaginationResponse(content);

        Assert.NotNull(paginationResponse);
        Assert.Equal(100, paginationResponse.Items.Count); // Clamped to MaxPageSize
        Assert.Equal(150, paginationResponse.TotalCount);
        Assert.Equal(2, paginationResponse.TotalPages); // 150 items / 100 per page
    }

    [Fact]
    public async Task GetProducts_Should_Return_Correct_TotalPages_Calculation()
    {
        // Arrange
        var client = CreateAuthenticatedUserClient();
        var dbContext = GetDbContext();

        // Create 25 products
        for (var i = 1; i <= 25; i++)
        {
            var product = Product.Create(
                $"Product {i:D2}",
                1.00m,
                "Test description.",
                1,
                "https://example.com/product.jpg"
            );
            dbContext.Products.Add(product);
        }

        await dbContext.SaveChangesAsync(CurrentCancellationToken);

        // Act
        var response = await client.GetProductsAsync(1, 10, CurrentCancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync(CurrentCancellationToken);
        var paginationResponse = DeserializePaginationResponse(content);

        Assert.NotNull(paginationResponse);
        Assert.Equal(25, paginationResponse.TotalCount);
        Assert.Equal(3, paginationResponse.TotalPages); // Math.Ceiling(25 / 10) = 3
    }

    [Fact]
    public async Task GetProducts_Should_Return_Correct_Page_When_Requesting_Last_Page()
    {
        // Arrange
        var client = CreateAuthenticatedUserClient();
        var dbContext = GetDbContext();

        // Create 15 products
        for (var i = 1; i <= 15; i++)
        {
            var product = Product.Create(
                $"Product {i:D2}",
                2.00m,
                "Test description.",
                1,
                "https://example.com/product.jpg"
            );
            dbContext.Products.Add(product);
        }

        await dbContext.SaveChangesAsync(CurrentCancellationToken);

        // Act - Get last page (page 3 with 5 items per page)
        var response = await client.GetProductsAsync(3, 5, CurrentCancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync(CurrentCancellationToken);
        var paginationResponse = DeserializePaginationResponse(content);

        Assert.NotNull(paginationResponse);
        Assert.Equal(5, paginationResponse.Items.Count); // Last page with 5 items (15 % 5 = 0)
        Assert.Equal(3, paginationResponse.PageIndex);
        Assert.Equal(15, paginationResponse.TotalCount);
        Assert.Equal(3, paginationResponse.TotalPages);
    }

    [Fact]
    public async Task GetProducts_Should_Return_HasPreviousPage_And_HasNextPage_Correctly()
    {
        // Arrange
        var client = CreateAuthenticatedUserClient();
        var dbContext = GetDbContext();

        // Create 10 products
        for (var i = 1; i <= 10; i++)
        {
            var product = Product.Create(
                $"Product {i:D2}",
                3.00m,
                "Test description.",
                1,
                "https://example.com/product.jpg"
            );
            dbContext.Products.Add(product);
        }

        await dbContext.SaveChangesAsync(CurrentCancellationToken);

        // Act - Get page 2 with 3 items per page
        var response = await client.GetProductsAsync(2, 3, CurrentCancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync(CurrentCancellationToken);
        var paginationResponse = DeserializePaginationResponse(content);

        Assert.NotNull(paginationResponse);
        Assert.True(paginationResponse.HasPreviousPage); // Page 2 has a previous page
        Assert.True(paginationResponse.HasNextPage); // Page 2 has a next page (4 total pages)
    }

    [Fact]
    public async Task GetProducts_Should_Return_Default_PageSize_When_Not_Specified()
    {
        // Arrange
        var client = CreateAuthenticatedUserClient();
        var dbContext = GetDbContext();

        // Create 15 products
        for (var i = 1; i <= 15; i++)
        {
            var product = Product.Create(
                $"Product {i:D2}",
                4.00m,
                "Test description.",
                1,
                "https://example.com/product.jpg"
            );
            dbContext.Products.Add(product);
        }

        await dbContext.SaveChangesAsync(CurrentCancellationToken);

        // Act - Don't specify pageSize (should default to 10)
        var response = await client.GetProductsAsync(cancellationToken: CurrentCancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync(CurrentCancellationToken);
        var paginationResponse = DeserializePaginationResponse(content);

        Assert.NotNull(paginationResponse);
        Assert.Equal(10, paginationResponse.Items.Count); // Default page size
        Assert.Equal(15, paginationResponse.TotalCount);
    }

    [Fact]
    public async Task GetProducts_Should_ClampPageIndex_To_Minimum_1_When_Invalid()
    {
        // Arrange
        var client = CreateAuthenticatedUserClient();
        var dbContext = GetDbContext();

        // Create 15 products
        for (var i = 1; i <= 15; i++)
        {
            var product = Product.Create(
                $"Product {i:D2}",
                6.00m,
                "Test description.",
                1,
                "https://example.com/product.jpg"
            );
            dbContext.Products.Add(product);
        }

        await dbContext.SaveChangesAsync(CurrentCancellationToken);

        // Act - Request pageIndex -5 (should be clamped to 1)
        var response = await client.GetProductsAsync(-5, 10, CurrentCancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync(CurrentCancellationToken);
        var paginationResponse = DeserializePaginationResponse(content);

        Assert.NotNull(paginationResponse);
        Assert.Equal(1, paginationResponse.PageIndex); // Clamped to minimum 1
    }
}