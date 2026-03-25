using Api.Common.Shared.Guards;
using Api.Common.Shared.Models;
using Api.Features.Baskets.Models;
using Api.Features.Products.Exceptions;

namespace Api.Features.Products.Models;

[ValueObject<Guid>]
public readonly partial struct ProductId;

public sealed class Product : Entity<ProductId>
{
    public const string DefaultContainer = "product-images";

    private Product()
    {
    } // EF

    private Product(string name, decimal price, string description, int stock, string imageUrl)
    {
        Id = ProductId.From(Guid.CreateVersion7());
        Name = name;
        Price = price;
        Description = description;
        Quantity = stock;
        ImageUrl = imageUrl;
    }

    public string Name { get; private set; }
    public decimal Price { get; private set; }
    public string Description { get; private set; }
    public int Quantity { get; private set; }
    public string ImageUrl { get; private set; } = null!;

    public static Product Create(string name, decimal price, string description, int stock, string imageUrl)
    {
        var nName = NormalizeRequired(name);
        var nDesc = NormalizeOptional(description);
        var nUrl = NormalizeRequired(imageUrl);

        Validate(nName, price, nDesc, stock, nUrl);

        return new Product(nName, price, nDesc, stock, nUrl);
    }

    public void Update(string name, decimal price, string description, int stock, string imageUrl)
    {
        var nName = NormalizeRequired(name);
        var nDesc = NormalizeOptional(description);
        var nUrl = NormalizeRequired(imageUrl);

        Validate(nName, price, nDesc, stock, nUrl);

        Price = price;
        Quantity = stock;
        Name = nName;
        Description = nDesc;
        ImageUrl = nUrl;
    }

    public BasketItem ToBasketItem()
    {
        return new BasketItem(Id);
    }
    
    public void IncreaseQuantity(int basketQuantity)
    {
        Guard.Against.NegativeOrZero<Product>(basketQuantity);

        Quantity += basketQuantity;
    }

    public void DecreaseQuantity(int basketQuantity)
    {
        Guard.Against.NegativeOrZero<Product>(basketQuantity);

        if (basketQuantity > Quantity)
        {
            throw new InsufficientProductStockException(Id, Quantity, basketQuantity);
        }

        Quantity -= basketQuantity;
    }

    private static void Validate(string name, decimal price, string description, int stock, string imageUrl)
    {
        Guard.Against.NullOrEmpty<Product>(name);
        Guard.Against.LengthBetween<Product>(name, ProductRules.MinNameLength, ProductRules.NameMaxLength);

        Guard.Against.NullOrEmpty<Product>(description);
        Guard.Against.LengthBetween<Product>(description, ProductRules.MinDescriptionLength,
            ProductRules.MaxDescriptionMaxLength);

        Guard.Against.NegativeOrZero<Product>(price);
        Guard.Against.Negative<Product>(stock);

        Guard.Against.ValidAbsoluteHttpUrl<Product>(imageUrl);
    }

    private static string NormalizeRequired(string value)
    {
        return value.Trim();
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}