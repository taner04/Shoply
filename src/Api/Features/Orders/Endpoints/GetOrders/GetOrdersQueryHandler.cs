using Api.Common.Infrastructure.Services;
using Api.Common.Shared.Pagination;
using Mediator;

namespace Api.Features.Orders.Endpoints.GetOrders;

public sealed class GetOrdersQueryHandler(
    PaginationService paginationService,
    GetOrdersMapper mapper,
    CurrentUserService currentUserService)
    : IQueryHandler<GetOrdersQuery, PaginationResult<OrdersResponse>>
{
    public async ValueTask<PaginationResult<OrdersResponse>> Handle(GetOrdersQuery query,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetCurrentUserId();
        return await paginationService.GetPaginationResultAsync(
            query,
            mapper,
            q => q.Where(o => o.UserId == userId),
            cancellationToken);
    }
}