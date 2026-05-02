using Shoply.WebApi.Common.Shared.Guards;
using Shoply.WebApi.Common.Shared.Models;

namespace Shoply.WebApi.Features.Products.Models;

[ValueObject<Guid>]
public readonly partial struct ProductId
{
    private static Validation Validate(Guid value)
    {
        return value != Guid.Empty ? Validation.Ok : Validation.Invalid("ProductId must set to non-default value.");
    }
}

public sealed class Product : Entity<ProductId>
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private Product()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {
    } // EF

    public Product(string name, decimal price, string description, int stock, string imageUrl)
    {
        Guard.Against.NullOrEmpty<Product>(name);
        Guard.Against.LengthBetween<Product>(name, ProductRules.MinNameLength, ProductRules.NameMaxLength);

        Guard.Against.NullOrEmpty<Product>(description);
        Guard.Against.LengthBetween<Product>(description, ProductRules.MinDescriptionLength,
            ProductRules.MaxDescriptionMaxLength);

        Guard.Against.NegativeOrZero<Product>(price);
        Guard.Against.Negative<Product>(stock);

        Id = ProductId.From(Guid.CreateVersion7());
        Name = name;
        Price = price;
        Description = description;
        Quantity = stock;
        ImageUrl = imageUrl;
    }

    public string Name { get; set; }
    public decimal Price { get; set; }
    public string Description { get; set; }
    public int Quantity { get; set; }
    public string ImageUrl { get; set; }
}