using Api.Common.Shared.Pagination;

namespace Api.Features.Orders.Endpoints.GetOrders;

public sealed record GetOrdersQuery(int PageIndex, int PageSize)
    : PaginationQuery(PageIndex, PageSize), IQuery<PaginationResult<OrdersResponse>>, IUserRequest;