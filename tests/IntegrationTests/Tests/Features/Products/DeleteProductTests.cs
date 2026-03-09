using System.Net;
using Api.Common.Domain.Products;
using Api.Features.Products.Endpoints;
using IntegrationTests.Infrastructure;
using IntegrationTests.Infrastructure.Fixtures;
using Microsoft.EntityFrameworkCore;

namespace IntegrationTests.Tests.Features.Products;

public sealed class DeleteProductTests(TestingFixture fixture) : TestingBase(fixture)
{
    [Fact]
    public async Task DeleteProduct_Should_Return401_When_Unauthenticated()
    {
        // Arrange
        var unauthenticated = CreateUnauthenticatedClient();
        var productId = Guid.NewGuid();

        // Act
        var response = await unauthenticated.DeleteProductAsync(productId, CurrentCancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task DeleteProduct_Should_Return204_When_ProductIsDeleted()
    {
        // Arrange
        var client = CreateAuthenticatedUserClient();
        var dbContext = GetDbContext();

        // Create a product first
        var product = Product.Create(
            "Product to Delete",
            19.99m,
            "This product will be deleted.",
            5,
            "https://example.com/delete.jpg"
        );
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync(CurrentCancellationToken);

        var productId = product.Id;

        // Verify product exists
        var productBeforeDelete = await dbContext.Products
            .FirstOrDefaultAsync(p => p.Id == productId, CurrentCancellationToken);
        Assert.NotNull(productBeforeDelete);

        // Act
        var response = await client.DeleteProductAsync(productId, CurrentCancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        // Verify product is deleted from database
        var deletedProduct = await dbContext.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == productId, CurrentCancellationToken);
        Assert.Null(deletedProduct);
    }

    [Fact]
    public async Task DeleteProduct_Should_Return404_When_ProductDoesNotExist()
    {
        // Arrange
        var client = CreateAuthenticatedUserClient();
        var nonExistentProductId = Guid.NewGuid();

        // Act
        var response = await client.DeleteProductAsync(nonExistentProductId, CurrentCancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteProduct_Should_Return404_When_Trying_To_Delete_Already_Deleted_Product()
    {
        // Arrange
        var client = CreateAuthenticatedUserClient();
        var dbContext = GetDbContext();

        // Create a product
        var product = Product.Create(
            "Product to Delete Twice",
            15.00m,
            "This product will be deleted twice.",
            3,
            "https://example.com/delete-twice.jpg"
        );
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync(CurrentCancellationToken);

        var productId = product.Id;

        // Delete the product
        var firstDelete = await client.DeleteProductAsync(productId, CurrentCancellationToken);
        Assert.Equal(HttpStatusCode.NoContent, firstDelete.StatusCode);

        // Act - Try to delete the same product again
        var secondDelete = await client.DeleteProductAsync(productId, CurrentCancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, secondDelete.StatusCode);
    }

    [Fact]
    public async Task DeleteProduct_Should_Only_Delete_Specified_Product()
    {
        // Arrange
        var client = CreateAuthenticatedUserClient();
        var dbContext = GetDbContext();

        // Create two products
        var product1 = Product.Create(
            "Product 1",
            10.00m,
            "Product 1 description.",
            5,
            "https://example.com/product1.jpg"
        );

        var product2 = Product.Create(
            "Product 2",
            20.00m,
            "Product 2 description.",
            10,
            "https://example.com/product2.jpg"
        );

        dbContext.Products.Add(product1);
        dbContext.Products.Add(product2);
        await dbContext.SaveChangesAsync(CurrentCancellationToken);

        var product1Id = product1.Id;
        var product2Id = product2.Id;

        // Act - Delete only product1
        var response = await client.DeleteProductAsync(product1Id, CurrentCancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        // Verify only product1 is deleted
        var deletedProduct = await dbContext.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == product1Id, CurrentCancellationToken);
        Assert.Null(deletedProduct);

        // Verify product2 still exists
        var existingProduct = await dbContext.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == product2Id, CurrentCancellationToken);
        Assert.NotNull(existingProduct);
        Assert.Equal("Product 2", existingProduct.Name);
    }

    [Fact]
    public async Task DeleteProduct_With_InvalidGuid_Should_Return400_Or_404()
    {
        // Arrange
        var client = CreateAuthenticatedUserClient();

        // Act - Guid.Empty should not match any product
        var response = await client.DeleteProductAsync(Guid.Empty, CurrentCancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
