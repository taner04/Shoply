using Shoply.WebApi.Common.Infrastructure.Services.Pagination;

namespace Shoply.WebApi.Features.Orders.Endpoints.GetOrders;

public sealed record GetOrdersQuery(int PageIndex, int PageSize)
    : PaginationQuery(PageIndex, PageSize), IQuery<PaginationResult<OrdersResponse>>, IUserRequest;