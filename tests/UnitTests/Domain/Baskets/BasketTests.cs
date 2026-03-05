using Api.Common.Domain.Baskets;
using Api.Common.Domain.Baskets.Exceptions;
using Api.Common.Domain.Products;
using Api.Common.Domain.Users;
using Api.Common.Shared.Exceptions;

namespace UnitTests.Domain.Baskets;

public sealed class BasketTests
{
    private static Product CreateProduct(string name = "Coffee Beans", decimal price = 12.99m)
        => Product.Create(
            name: name,
            price: price,
            description: "Fresh roasted beans",
            stock: 10,
            imageUrl: "https://example.com/image.jpg");

    private static User CreateUser()
        => User.Create("test@example.com", "auth0|123");

    [Fact]
    public void CreateEmpty_ShouldCreateBasketWithoutItems()
    {
        var user = CreateUser();
        var basket = Basket.CreateEmpty(user.Id);

        Assert.Empty(basket.BasketItems);
        Assert.Equal(user.Id, basket.UserId);
    }

    [Fact]
    public void AddProduct_FirstTime_ShouldAddBasketItemWithQuantityOne()
    {
        var user = CreateUser();
        var basket = Basket.CreateEmpty(user.Id);
        var product = CreateProduct();

        basket.AddProduct(BasketItem.From(product));

        var item = Assert.Single(basket.BasketItems);
        Assert.Equal(product.Id, item.ProductId);
        Assert.Equal(1, item.Quantity);
    }

    [Fact]
    public void AddProduct_SameProductTwice_ShouldIncreaseQuantity()
    {
        var user = CreateUser();
        var basket = Basket.CreateEmpty(user.Id);
        var product = CreateProduct();

        basket.AddProduct(BasketItem.From(product));
        basket.AddProduct(BasketItem.From(product));

        var item = Assert.Single(basket.BasketItems);
        Assert.Equal(product.Id, item.ProductId);
        Assert.Equal(2, item.Quantity);
    }

    [Fact]
    public void AddProduct_DifferentProducts_ShouldAddBothItems()
    {
        var user = CreateUser();
        var basket = Basket.CreateEmpty(user.Id);
        var product1 = CreateProduct("Coffee", 12.99m);
        var product2 = CreateProduct("Tea", 8.99m);

        basket.AddProduct(BasketItem.From(product1));
        basket.AddProduct(BasketItem.From(product2));

        Assert.Equal(2, basket.BasketItems.Count);
        Assert.Single(basket.BasketItems, x => x.ProductId == product1.Id);
        Assert.Single(basket.BasketItems, x => x.ProductId == product2.Id);
    }

    [Fact]
    public void RemoveProduct_ProductNotInBasket_ShouldThrowEntityNotFound()
    {
        var user = CreateUser();
        var basket = Basket.CreateEmpty(user.Id);
        var product = CreateProduct();

        Assert.Throws<EntityNotFoundException<BasketItem>>(() =>
            basket.RemoveProduct(product.Id));
    }

    [Fact]
    public void RemoveProduct_WhenQuantityIsTwo_ShouldDecreaseToOne()
    {
        var user = CreateUser();
        var basket = Basket.CreateEmpty(user.Id);
        var product = CreateProduct();

        basket.AddProduct(BasketItem.From(product));
        basket.AddProduct(BasketItem.From(product)); // qty = 2

        basket.RemoveProduct(product.Id);

        var item = Assert.Single(basket.BasketItems);
        Assert.Equal(1, item.Quantity);
    }

    [Fact]
    public void RemoveProduct_WhenQuantityIsOne_ShouldRemoveItemFromBasket()
    {
        var user = CreateUser();
        var basket = Basket.CreateEmpty(user.Id);
        var product = CreateProduct();

        basket.AddProduct(BasketItem.From(product)); // qty = 1
        basket.RemoveProduct(product.Id);            // removes it

        Assert.Empty(basket.BasketItems);
    }

    [Fact]
    public void RemoveProduct_WhenQuantityIsOneAndCalledTwice_ShouldThrowEntityNotFound()
    {
        var user = CreateUser();
        var basket = Basket.CreateEmpty(user.Id);
        var product = CreateProduct();

        basket.AddProduct(BasketItem.From(product));
        basket.RemoveProduct(product.Id);

        Assert.Throws<EntityNotFoundException<BasketItem>>(() =>
            basket.RemoveProduct(product.Id));
    }
}
