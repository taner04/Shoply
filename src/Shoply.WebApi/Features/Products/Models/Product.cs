using Shoply.WebApi.Common.Shared.Guards;
using Shoply.WebApi.Common.Shared.Models;
using Shoply.WebApi.Features.Baskets.Models;
using Shoply.WebApi.Features.Products.Exceptions;

namespace Shoply.WebApi.Features.Products.Models;

[ValueObject<Guid>]
public readonly partial struct ProductId
{
    private static Validation Validate(Guid value)
        => value != Guid.Empty ? Validation.Ok : Validation.Invalid("ProductId must set to non-default value.");
}

public sealed class Product : Entity<ProductId>
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private Product()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
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
        var nDesc = NormalizeRequired(description);
        var nUrl = NormalizeRequired(imageUrl);

        Validate(nName, price, nDesc, stock, nUrl);

        return new Product(nName, price, nDesc, stock, nUrl);
    }

    public void Update(string name, decimal price, string description, int stock, string imageUrl)
    {
        var nName = NormalizeRequired(name);
        var nDesc = NormalizeRequired(description);
        var nUrl = NormalizeRequired(imageUrl);

        Validate(nName, price, nDesc, stock, nUrl);

        Price = price;
        Quantity = stock;
        Name = nName;
        Description = nDesc;
        ImageUrl = nUrl;
    }

    public BasketItem ToBasketItem() => new(Id);

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

    private static string NormalizeRequired(string value) => value.Trim();
}