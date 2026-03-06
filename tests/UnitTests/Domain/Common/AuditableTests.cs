using Api.Common.Domain;
using Api.Common.Domain.Products;

namespace UnitTests.Domain.Common;

public sealed class AuditableTests
{
    [Fact]
    public void SetCreated_WithoutUserParameter_ShouldSetCreatedByToSystem()
    {
        var auditable = new AuditableTestEntity();

        auditable.SetCreated();

        Assert.Equal("System", auditable.CreatedBy);
    }

    [Fact]
    public void SetCreated_WithUserParameter_ShouldSetCreatedByToUser()
    {
        var auditable = new AuditableTestEntity();
        const string userId = "user123";

        auditable.SetCreated(userId);

        Assert.Equal(userId, auditable.CreatedBy);
    }

    [Fact]
    public void SetCreated_ShouldSetCreatedAtToCurrentUtcTime()
    {
        var auditable = new AuditableTestEntity();
        var beforeCreation = DateTimeOffset.UtcNow;

        auditable.SetCreated("user123");

        var afterCreation = DateTimeOffset.UtcNow;
        Assert.InRange(auditable.CreatedAt, beforeCreation, afterCreation.AddSeconds(1));
    }

    [Fact]
    public void SetCreated_ShouldSetCreatedAtWithUtcOffset()
    {
        var auditable = new AuditableTestEntity();

        auditable.SetCreated("user123");

        // DateTimeOffset should be in UTC (offset of 00:00)
        Assert.Equal(TimeSpan.Zero, auditable.CreatedAt.Offset);
    }

    [Fact]
    public void SetUpdated_WithoutUserParameter_ShouldSetUpdatedByToSystem()
    {
        var auditable = new AuditableTestEntity();
        auditable.SetCreated("creator");

        auditable.SetUpdated();

        Assert.Equal("System", auditable.UpdatedBy);
    }

    [Fact]
    public void SetUpdated_WithUserParameter_ShouldSetUpdatedByToUser()
    {
        var auditable = new AuditableTestEntity();
        auditable.SetCreated("creator");
        const string updatedBy = "updater123";

        auditable.SetUpdated(updatedBy);

        Assert.Equal(updatedBy, auditable.UpdatedBy);
    }

    [Fact]
    public void SetUpdated_ShouldSetUpdatedAtToCurrentUtcTime()
    {
        var auditable = new AuditableTestEntity();
        auditable.SetCreated("creator");
        var beforeUpdate = DateTimeOffset.UtcNow;

        auditable.SetUpdated("updater");

        var afterUpdate = DateTimeOffset.UtcNow;
        Assert.InRange(auditable.UpdatedAt!.Value, beforeUpdate, afterUpdate.AddSeconds(1));
    }

    [Fact]
    public void SetUpdated_ShouldSetUpdatedAtWithUtcOffset()
    {
        var auditable = new AuditableTestEntity();
        auditable.SetCreated("creator");

        auditable.SetUpdated("updater");

        Assert.Equal(TimeSpan.Zero, auditable.UpdatedAt!.Value.Offset);
    }

    [Fact]
    public void SetUpdated_CanBeCalledMultipleTimes()
    {
        var auditable = new AuditableTestEntity();
        auditable.SetCreated("creator");
        auditable.SetUpdated("updater1");
        var firstUpdateTime = auditable.UpdatedAt;

        // Add delay to ensure different timestamps
        Thread.Sleep(10);

        auditable.SetUpdated("updater2");

        Assert.True(auditable.UpdatedAt > firstUpdateTime, "UpdatedAt should be newer");
        Assert.Equal("updater2", auditable.UpdatedBy);
    }

    [Fact]
    public void CreatedAt_ShouldNotChangeAfterSetUpdated()
    {
        var auditable = new AuditableTestEntity();
        auditable.SetCreated("creator");
        var originalCreatedAt = auditable.CreatedAt;

        auditable.SetUpdated("updater");

        Assert.Equal(originalCreatedAt, auditable.CreatedAt);
    }

    [Fact]
    public void CreatedBy_ShouldNotChangeAfterSetUpdated()
    {
        var auditable = new AuditableTestEntity();
        auditable.SetCreated("creator");
        var originalCreatedBy = auditable.CreatedBy;

        auditable.SetUpdated("updater");

        Assert.Equal(originalCreatedBy, auditable.CreatedBy);
    }

    [Fact]
    public void UpdatedAt_ShouldBeNullBeforeFirstUpdate()
    {
        var auditable = new AuditableTestEntity();
        auditable.SetCreated("creator");

        Assert.Null(auditable.UpdatedAt);
    }

    [Fact]
    public void UpdatedBy_ShouldBeNullBeforeFirstUpdate()
    {
        var auditable = new AuditableTestEntity();
        auditable.SetCreated("creator");

        Assert.Null(auditable.UpdatedBy);
    }

    [Fact]
    public void CreatedByMaxLengthConstant_ShouldBe256()
    {
        Assert.Equal(256, Auditable.MaxCreatedByLength);
    }

    [Fact]
    public void UpdatedByMaxLengthConstant_ShouldBe256()
    {
        Assert.Equal(256, Auditable.MaxUpdatedByLength);
    }

    [Fact]
    public void IntegrationWithProduct_ShouldTrackAuditInfo()
    {
        var product = Product.Create(
            "Test Product",
            10.00m,
            "Test description",
            5,
            "https://example.com/image.jpg");

        // Product is created without audit info initially
        // In real scenario, SetCreated would be called by repository
        product.SetCreated("testuser");

        Assert.Equal("testuser", product.CreatedBy);
        Assert.NotEqual(default, product.CreatedAt);
        Assert.Null(product.UpdatedAt);
        Assert.Null(product.UpdatedBy);
    }

    private class AuditableTestEntity : Auditable
    {
        // Test implementation of abstract Auditable class
    }
}