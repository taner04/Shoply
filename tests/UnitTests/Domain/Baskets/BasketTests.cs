namespace UnitTests.Domain.Baskets;

public sealed class BasketTests
{
    private static Product CreateProduct(string name = "Coffee Beans", decimal price = 12.99m)
    {
        return Product.Create(
            name,
            price,
            "Fresh roasted beans",
            10,
            "https://example.com/image.jpg");
    }

    private static User CreateUser()
    {
        return User.Create("test@example.com", "auth0|123");
    }

    [Fact]
    public void CreateEmpty_ShouldCreateBasketWithoutItems()
    {
        var basket = Basket.CreateEmpty();

        Assert.Empty(basket.BasketItems);
        Assert.NotNull(basket.Id);
    }

    [Fact]
    public void AddProduct_FirstTime_ShouldAddBasketItemWithQuantityOne()
    {
        var basket = Basket.CreateEmpty();
        var product = CreateProduct();

        basket.AddProduct(product);

        var item = Assert.Single(basket.BasketItems);
        Assert.Equal(product.Id, item.ProductId);
        Assert.Equal(1, item.Quantity);
    }

    [Fact]
    public void AddProduct_SameProductTwice_ShouldIncreaseQuantity()
    {
        var basket = Basket.CreateEmpty();
        var product = CreateProduct();

        basket.AddProduct(product);
        basket.AddProduct(product);

        var item = Assert.Single(basket.BasketItems);
        Assert.Equal(product.Id, item.ProductId);
        Assert.Equal(2, item.Quantity);
    }

    [Fact]
    public void AddProduct_DifferentProducts_ShouldAddBothItems()
    {
        var basket = Basket.CreateEmpty();
        var product1 = CreateProduct("Coffee");
        var product2 = CreateProduct("Tea", 8.99m);

        basket.AddProduct(product1);
        basket.AddProduct(product2);

        Assert.Equal(2, basket.BasketItems.Count);
        Assert.Single(basket.BasketItems, x => x.ProductId == product1.Id);
        Assert.Single(basket.BasketItems, x => x.ProductId == product2.Id);
    }

    [Fact]
    public void RemoveProduct_ProductNotInBasket_ShouldThrowEntityNotFound()
    {
        var basket = Basket.CreateEmpty();
        var product = CreateProduct();

        Assert.Throws<EntityNotFoundException<BasketItem>>(() =>
            basket.RemoveProduct(product.Id));
    }

    [Fact]
    public void RemoveProduct_WhenQuantityIsTwo_ShouldDecreaseToOne()
    {
        var basket = Basket.CreateEmpty();
        var product = CreateProduct();

        basket.AddProduct(product);
        basket.AddProduct(product); // qty = 2

        basket.RemoveProduct(product.Id);

        var item = Assert.Single(basket.BasketItems);
        Assert.Equal(1, item.Quantity);
    }

    [Fact]
    public void RemoveProduct_WhenQuantityIsOne_ShouldRemoveItemFromBasket()
    {
        var basket = Basket.CreateEmpty();
        var product = CreateProduct();

        basket.AddProduct(product); // qty = 1
        basket.RemoveProduct(product.Id); // removes it

        Assert.Empty(basket.BasketItems);
    }

    [Fact]
    public void RemoveProduct_WhenQuantityIsOneAndCalledTwice_ShouldThrowEntityNotFound()
    {
        var basket = Basket.CreateEmpty();
        var product = CreateProduct();

        basket.AddProduct(product);
        basket.RemoveProduct(product.Id);

        Assert.Throws<EntityNotFoundException<BasketItem>>(() =>
            basket.RemoveProduct(product.Id));
    }
}