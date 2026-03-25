using Api.Features.Baskets.Exceptions;
using Api.Features.Baskets.Models;
using Api.Features.Products.Models;

namespace UnitTests.Domain.Baskets;

public sealed class BasketItemTests
{
    private static Product CreateProduct(string name = "Coffee Beans")
    {
        return Product.Create(
            name,
            12.99m,
            "Fresh roasted beans",
            10,
            "https://example.com/image.jpg");
    }

    [Fact]
    public void Create_BasketItem_WithDefaultQuantity_ShouldHaveQuantityOne()
    {
        var product = CreateProduct();

        var basketItem = new BasketItem(product.Id);

        Assert.Equal(product.Id, basketItem.ProductId);
        Assert.Equal(1, basketItem.Quantity);
    }

    [Fact]
    public void Create_BasketItem_WithCustomQuantity_ShouldHaveProvidedQuantity()
    {
        var product = CreateProduct();
        const int customQuantity = 5;

        var basketItem = new BasketItem(product.Id, customQuantity);

        Assert.Equal(product.Id, basketItem.ProductId);
        Assert.Equal(customQuantity, basketItem.Quantity);
    }

    [Fact]
    public void IncreaseQuantity_ShouldReturnNewItemWithIncrementedQuantity()
    {
        var basketItem = new BasketItem(ProductId.From(Guid.NewGuid()), 1);

        var increased = basketItem.IncreaseQuantity();

        Assert.Equal(1, basketItem.Quantity); // Original unchanged (record is immutable)
        Assert.Equal(2, increased.Quantity); // New item has incremented quantity
    }

    [Fact]
    public void IncreaseQuantity_MultipleTimes_ShouldAccumulate()
    {
        var productId = ProductId.From(Guid.NewGuid());
        var basketItem = new BasketItem(productId, 1);

        var qty2 = basketItem.IncreaseQuantity();
        var qty3 = qty2.IncreaseQuantity();
        var qty4 = qty3.IncreaseQuantity();

        Assert.Equal(1, basketItem.Quantity);
        Assert.Equal(4, qty4.Quantity);
    }

    [Fact]
    public void DecreaseQuantity_WhenQuantityIsOne_ShouldThrowException()
    {
        var basketItem = new BasketItem(ProductId.From(Guid.NewGuid()), 1);

        var ex = Assert.Throws<InvalidBasketItemQuantityException>(() =>
            basketItem.DecreaseQuantity());

        Assert.NotNull(ex);
    }

    [Fact]
    public void DecreaseQuantity_WhenQuantityIsTwo_ShouldReturnNewItemWithDecrementedQuantity()
    {
        var basketItem = new BasketItem(ProductId.From(Guid.NewGuid()), 1);
        var increased = basketItem.IncreaseQuantity(); // qty = 2

        var decreased = increased.DecreaseQuantity();

        Assert.Equal(2, increased.Quantity); // Original unchanged
        Assert.Equal(1, decreased.Quantity); // New item has decremented quantity
    }

    [Fact]
    public void DecreaseQuantity_WhenAtMinimumQuantity_ShouldThrow()
    {
        var productId = ProductId.From(Guid.NewGuid());
        var basketItem = new BasketItem(productId, 1);
        var qty2 = basketItem.IncreaseQuantity();
        var qty3 = qty2.IncreaseQuantity();
        var qty2Again = qty3.DecreaseQuantity();
        var qty1 = qty2Again.DecreaseQuantity();

        // At minimum (qty=1), further decrease throws
        var ex = Assert.Throws<InvalidBasketItemQuantityException>(() =>
            qty1.DecreaseQuantity());

        Assert.NotNull(ex);
    }

    [Fact]
    public void ToBasketItem_ShouldCreateBasketItemFromProduct()
    {
        var product = CreateProduct();

        var basketItem = product.ToBasketItem();

        Assert.Equal(product.Id, basketItem.ProductId);
        Assert.Equal(1, basketItem.Quantity);
    }
}