using Api.Common.Shared.Pagination;
using Mediator;

namespace Api.Features.Orders.Features.GetOrders;

public sealed record GetOrdersQuery(int PageIndex, int PageSize)
    : PaginationQuery(PageIndex, PageSize), IQuery<PaginationResult<OrdersResponse>>;