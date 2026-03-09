namespace UnitTests.Domain.Products;

public sealed class ProductTests
{
    private const decimal ValidPrice = 12.99m;
    private const int ValidStock = 10;
    private const string ValidImageUrl = "https://example.com/image.jpg";
    private static readonly string ValidName = new('a', ProductRules.MinNameLength);
    private static readonly string ValidDescription = new('b', ProductRules.MinDescriptionLength);

    [Fact]
    public void Create_WithValidValues_ShouldReturnProduct()
    {
        var product = Product.Create(ValidName, ValidPrice, ValidDescription, ValidStock, ValidImageUrl);

        Assert.Equal(ValidName, product.Name);
        Assert.Equal(ValidPrice, product.Price);
        Assert.Equal(ValidDescription, product.Description);
        Assert.Equal(ValidStock, product.Quantity);
        Assert.Equal(ValidImageUrl, product.ImageUrl);

        Assert.NotEqual(Guid.Empty, product.Id.Value);
    }

    [Fact]
    public void Create_ShouldTrimAndNormalizeValues()
    {
        var product = Product.Create(
            $"  {ValidName}  ",
            ValidPrice,
            $"  {ValidDescription}  ",
            ValidStock,
            $"  {ValidImageUrl}  ");

        Assert.Equal(ValidName, product.Name);
        Assert.Equal(ValidDescription, product.Description);
        Assert.Equal(ValidImageUrl, product.ImageUrl);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyName_ShouldThrow(string name)
    {
        var ex = Assert.Throws<GuardException>(() =>
            Product.Create(name, ValidPrice, ValidDescription, ValidStock, ValidImageUrl));

        Assert.Equal("Product.Name.InvalidEmpty", ex.ErrorCode);
    }

    [Fact]
    public void Create_WithNameTooShort_ShouldThrow()
    {
        var name = new string('a', ProductRules.MinNameLength - 1);

        var ex = Assert.Throws<GuardException>(() =>
            Product.Create(name, ValidPrice, ValidDescription, ValidStock, ValidImageUrl));

        Assert.Equal("Product.Name.InvalidLength", ex.ErrorCode);
    }

    [Fact]
    public void Create_WithNameTooLong_ShouldThrow()
    {
        var name = new string('a', ProductRules.NameMaxLength + 1);

        var ex = Assert.Throws<GuardException>(() =>
            Product.Create(name, ValidPrice, ValidDescription, ValidStock, ValidImageUrl));

        Assert.Equal("Product.Name.InvalidLength", ex.ErrorCode);
    }

    [Fact]
    public void Create_WithZeroPrice_ShouldThrow()
    {
        var ex = Assert.Throws<GuardException>(() =>
            Product.Create(ValidName, 0m, ValidDescription, ValidStock, ValidImageUrl));

        Assert.Equal("Product.Price.InvalidPositive", ex.ErrorCode);
    }

    [Fact]
    public void Create_WithNegativePrice_ShouldThrow()
    {
        var ex = Assert.Throws<GuardException>(() =>
            Product.Create(ValidName, -1m, ValidDescription, ValidStock, ValidImageUrl));

        Assert.Equal("Product.Price.InvalidPositive", ex.ErrorCode);
    }

    [Fact]
    public void Create_WithNegativeStock_ShouldThrow()
    {
        var ex = Assert.Throws<GuardException>(() =>
            Product.Create(ValidName, ValidPrice, ValidDescription, -1, ValidImageUrl));

        Assert.Equal("Product.Stock.InvalidNonNegative", ex.ErrorCode);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithNullOrWhitespaceDescription_ShouldNormalizeToNull(string? description)
    {
        var product = Product.Create(ValidName, ValidPrice, description, ValidStock, ValidImageUrl);

        Assert.Null(product.Description);
    }

    [Fact]
    public void Create_WithDescriptionTooShort_ShouldThrow()
    {
        var desc = new string('b', ProductRules.MinDescriptionLength - 1);

        var ex = Assert.Throws<GuardException>(() =>
            Product.Create(ValidName, ValidPrice, desc, ValidStock, ValidImageUrl));

        Assert.Equal("Product.Description.InvalidLength", ex.ErrorCode);
    }

    [Fact]
    public void Create_WithDescriptionTooLong_ShouldThrow()
    {
        var desc = new string('b', ProductRules.MaxDescriptionMaxLength + 1);

        var ex = Assert.Throws<GuardException>(() =>
            Product.Create(ValidName, ValidPrice, desc, ValidStock, ValidImageUrl));

        Assert.Equal("Product.Description.InvalidLength", ex.ErrorCode);
    }

    [Theory]
    [InlineData("not-a-url")]
    [InlineData("/relative/path.jpg")]
    [InlineData("ftp://example.com/image.jpg")]
    public void Create_WithInvalidImageUrl_ShouldThrow(string url)
    {
        var ex = Assert.Throws<GuardException>(() =>
            Product.Create(ValidName, ValidPrice, ValidDescription, ValidStock, url));

        Assert.Equal("Product.ImageUrl.InvalidAbsoluteUri", ex.ErrorCode);
    }

    [Fact]
    public void Update_WithValidValues_ShouldUpdateFields()
    {
        var product = Product.Create(ValidName, ValidPrice, ValidDescription, ValidStock, ValidImageUrl);

        var newName = "New Name"; // length 8 -> ok
        var newDesc = "New description"; // length >= 10 -> ok
        var newUrl = "https://example.com/new.jpg";

        product.Update(newName, 9.50m, newDesc, 3, newUrl);

        Assert.Equal(newName, product.Name);
        Assert.Equal(9.50m, product.Price);
        Assert.Equal(newDesc, product.Description);
        Assert.Equal(3, product.Quantity);
        Assert.Equal(newUrl, product.ImageUrl);
    }

    [Fact]
    public void Update_WithInvalidPrice_ShouldThrowAndNotChangeState()
    {
        var product = Product.Create(ValidName, ValidPrice, ValidDescription, ValidStock, ValidImageUrl);

        var oldName = product.Name;
        var oldPrice = product.Price;
        var oldDesc = product.Description;
        var oldQty = product.Quantity;
        var oldUrl = product.ImageUrl;

        var ex = Assert.Throws<GuardException>(() =>
            product.Update("New Name", 0m, "New description", 3, "https://example.com/new.jpg"));

        Assert.Equal("Product.Price.InvalidPositive", ex.ErrorCode);

        Assert.Equal(oldName, product.Name);
        Assert.Equal(oldPrice, product.Price);
        Assert.Equal(oldDesc, product.Description);
        Assert.Equal(oldQty, product.Quantity);
        Assert.Equal(oldUrl, product.ImageUrl);
    }
}