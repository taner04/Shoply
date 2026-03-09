using Api.Common.Shared.Guards;

namespace Api.Common.Domain.Products;

[ValueObject<Guid>]
public readonly partial struct ProductId;

public sealed class Product : AggregateRoot<ProductId>
{
    public const string DefaultContainer = "product-images";

    private Product()
    {
    } // EF

    private Product(string name, decimal price, string? description, int stock, string imageUrl)
    {
        Id = ProductId.From(Guid.CreateVersion7());
        Name = name;
        Price = price;
        Description = description;
        Quantity = stock;
        ImageUrl = imageUrl;
    }

    public string Name { get; private set; } = null!;
    public decimal Price { get; private set; }
    public string? Description { get; private set; }
    public int Quantity { get; private set; }
    public string ImageUrl { get; private set; } = null!;

    public static Product Create(string name, decimal price, string? description, int stock, string imageUrl)
    {
        var nName = NormalizeRequired(name);
        var nDesc = NormalizeOptional(description);
        var nUrl = NormalizeRequired(imageUrl);

        Validate(nName, price, nDesc, stock, nUrl);

        return new Product(nName, price, nDesc, stock, nUrl);
    }

    public void Update(string name, decimal price, string? description, int stock, string imageUrl)
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

    private static void Validate(string name, decimal price, string? description, int stock, string imageUrl)
    {
        Guard.Against.NullOrEmpty<Product>(name);
        Guard.Against.LengthBetween<Product>(name, ProductRules.MinNameLength, ProductRules.NameMaxLength);

        Guard.Against.NegativeOrZero<Product>(price);
        Guard.Against.Negative<Product>(stock);

        Guard.Against.LengthBetween<Product>(description, ProductRules.MinDescriptionLength,
            ProductRules.MaxDescriptionMaxLength);

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