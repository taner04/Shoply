using Shoply.WebApi.Common.Infrastructure.Services;
using Shoply.WebApi.Common.Infrastructure.Services.Pagination;

namespace Shoply.WebApi.Features.Orders.Endpoints.GetOrders;

public sealed class GetOrdersQueryHandler(
    PaginationService paginationService,
    IMapper<Order, OrdersResponse> mapper,
    CurrentUserService currentUserService)
    : IQueryHandler<GetOrdersQuery, PaginationResult<OrdersResponse>>
{
    public async ValueTask<PaginationResult<OrdersResponse>> Handle(
        GetOrdersQuery query,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetCurrentUserId();
        return await paginationService.GetPaginationResultAsync(
            query,
            mapper,
            q => q.Include(o => o.Payment).Where(o => o.UserId == userId),
            cancellationToken);
    }
}