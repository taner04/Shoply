using Api.Common.Domain.Baskets.Exceptions;

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
    public void From_ShouldCreateBasketItemFromProduct()
    {
        var product = CreateProduct();

        var basketItem = BasketItem.From(product);

        Assert.Equal(product.Id, basketItem.ProductId);
        Assert.Equal(1, basketItem.Quantity);
    }

    [Fact]
    public void From_ShouldInitializeWithQuantityOne()
    {
        var product = CreateProduct();

        var basketItem = BasketItem.From(product);

        Assert.Equal(1, basketItem.Quantity);
    }

    [Fact]
    public void IncreaseQuantity_ShouldIncrementByOne()
    {
        var product = CreateProduct();
        var basketItem = BasketItem.From(product);

        basketItem.IncreaseQuantity();

        Assert.Equal(2, basketItem.Quantity);
    }

    [Fact]
    public void IncreaseQuantity_MultipleTimes_ShouldAccumulate()
    {
        var product = CreateProduct();
        var basketItem = BasketItem.From(product);

        basketItem.IncreaseQuantity();
        basketItem.IncreaseQuantity();
        basketItem.IncreaseQuantity();

        Assert.Equal(4, basketItem.Quantity);
    }

    [Fact]
    public void DecreaseQuantity_WhenQuantityIsOne_ShouldThrowException()
    {
        var product = CreateProduct();
        var basketItem = BasketItem.From(product); // qty = 1

        // Quantity must stay >= 1, so decreasing from 1 throws
        var ex = Assert.Throws<BasketItemQuantityDecreaseException>(() =>
            basketItem.DecreaseQuantity());

        Assert.NotNull(ex);
        Assert.Equal(1, basketItem.Quantity); // Unchanged
    }

    [Fact]
    public void DecreaseQuantity_WhenQuantityIsTwo_ShouldDecreaseToOne()
    {
        var product = CreateProduct();
        var basketItem = BasketItem.From(product);
        basketItem.IncreaseQuantity(); // qty = 2

        basketItem.DecreaseQuantity();

        Assert.Equal(1, basketItem.Quantity);
    }

    [Fact]
    public void DecreaseQuantity_WhenAtMinimumQuantity_ShouldThrow()
    {
        var product = CreateProduct();
        var basketItem = BasketItem.From(product);
        basketItem.IncreaseQuantity(); // qty = 2
        basketItem.IncreaseQuantity(); // qty = 3
        basketItem.DecreaseQuantity(); // qty = 2
        basketItem.DecreaseQuantity(); // qty = 1

        // At minimum (qty=1), further decrease throws
        var ex = Assert.Throws<BasketItemQuantityDecreaseException>(() =>
            basketItem.DecreaseQuantity());

        Assert.Equal(1, basketItem.Quantity); // Quantity unchanged due to exception
    }
}