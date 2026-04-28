using Shoply.WebApi.Common.Infrastructure.Services.Pagination;
using Shoply.WebApi.Features.Products.Endpoints.GetProducts;

namespace Shoply.WebApi.Features.Products.Endpoints.SearchProducts;

public record class SearchProductQuery(int PageIndex, int PageSize, string Name) : PaginationQuery(PageIndex, PageSize), IQuery<PaginationResult<ProductsResponse>>;