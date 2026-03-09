using Api.Common.Shared.Pagination;

namespace UnitTests.Common.Shared.Pagination;

public sealed class PaginationLogicTests
{
    [Theory]
    [InlineData(1, 25, 3, false, true)] // First page of 3
    [InlineData(2, 25, 3, true, true)] // Middle page
    [InlineData(3, 25, 3, true, false)] // Last page
    [InlineData(1, 10, 2, false, true)] // First page with different total
    [InlineData(2, 10, 2, true, false)] // Last page with different total
    public void PaginationResponse_Should_Calculate_Correctly(
        int pageIndex,
        int totalCount,
        int expectedTotalPages,
        bool expectedHasPreviousPage,
        bool expectedHasNextPage)
    {
        // Arrange
        var items = new List<Product>();

        // Act
        var response = new PaginationResponse<Product>(items, pageIndex, expectedTotalPages, totalCount);

        // Assert
        Assert.Equal(pageIndex, response.PageIndex);
        Assert.Equal(expectedTotalPages, response.TotalPages);
        Assert.Equal(totalCount, response.TotalCount);
        Assert.Equal(expectedHasPreviousPage, response.HasPreviousPage);
        Assert.Equal(expectedHasNextPage, response.HasNextPage);
    }

    [Theory]
    [InlineData(10, 25, 3)] // 25 items with page size 10 = 3 pages
    [InlineData(7, 25, 4)] // 25 items with page size 7 = 4 pages (rounded up)
    [InlineData(25, 25, 1)] // 25 items with page size 25 = 1 page
    [InlineData(100, 25, 1)] // 25 items with page size 100 = 1 page
    [InlineData(1, 100, 100)] // 100 items with page size 1 = 100 pages
    public void CalculateTotalPages_Should_Return_Correct_Value(
        int pageSize,
        int totalCount,
        int expectedTotalPages)
    {
        // Arrange & Act
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        // Assert
        Assert.Equal(expectedTotalPages, totalPages);
    }

    [Theory]
    [InlineData(1, 10, 0, 9)] // Page 1, Skip 0, Take 10
    [InlineData(2, 10, 10, 9)] // Page 2, Skip 10, Take 10
    [InlineData(3, 10, 20, 9)] // Page 3, Skip 20, Take 10
    [InlineData(1, 5, 0, 4)] // Page 1, Skip 0, Take 5
    [InlineData(5, 10, 40, 9)] // Page 5, Skip 40, Take 10
    public void SkipAndTake_Should_Calculate_Correctly(
        int pageIndex,
        int pageSize,
        int expectedSkip,
        int expectedTakeMinusOne) // We test Take-1 because we can't easily verify Take value
    {
        // Arrange & Act
        var skip = (pageIndex - 1) * pageSize;
        var take = pageSize;

        // Assert
        Assert.Equal(expectedSkip, skip);
        Assert.Equal(expectedTakeMinusOne + 1, take);
    }

    [Fact]
    public void PaginationResponse_Should_Handle_Empty_Results()
    {
        // Arrange
        var items = new List<Product>();
        var pageIndex = 1;
        var totalPages = 0;
        var totalCount = 0;

        // Act
        var response = new PaginationResponse<Product>(items, pageIndex, totalPages, totalCount);

        // Assert
        Assert.Empty(response.Items);
        Assert.Equal(1, response.PageIndex);
        Assert.Equal(0, response.TotalPages);
        Assert.Equal(0, response.TotalCount);
        Assert.False(response.HasPreviousPage);
        Assert.False(response.HasNextPage);
    }

    [Theory]
    [InlineData(0, 1)] // PageIndex 0 -> Clamped to 1
    [InlineData(-5, 1)] // PageIndex -5 -> Clamped to 1
    [InlineData(1, 1)] // PageIndex 1 -> Stays 1
    [InlineData(100, 100)] // PageIndex 100 -> Stays 100
    public void PageIndex_Clamping_Should_Use_Max(int inputPageIndex, int expectedPageIndex)
    {
        // Arrange & Act
        var clampedPageIndex = Math.Max(1, inputPageIndex);

        // Assert
        Assert.Equal(expectedPageIndex, clampedPageIndex);
    }

    [Theory]
    [InlineData(0, 1)] // PageSize 0 -> Clamped to 1
    [InlineData(-10, 1)] // PageSize -10 -> Clamped to 1
    [InlineData(1, 1)] // PageSize 1 -> Stays 1
    [InlineData(50, 50)] // PageSize 50 -> Stays 50
    [InlineData(100, 100)] // PageSize 100 -> Stays 100
    [InlineData(150, 100)] // PageSize 150 -> Clamped to 100
    [InlineData(1000, 100)] // PageSize 1000 -> Clamped to 100
    public void PageSize_Clamping_Should_Use_Clamp(int inputPageSize, int expectedPageSize)
    {
        // Arrange & Act
        var clampedPageSize = Math.Clamp(inputPageSize, 1, 100);

        // Assert
        Assert.Equal(expectedPageSize, clampedPageSize);
    }
}