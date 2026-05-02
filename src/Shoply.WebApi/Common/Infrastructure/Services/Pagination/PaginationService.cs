using Shoply.WebApi.Common.Infrastructure.Services.Pagination.Exceptions;

namespace Shoply.WebApi.Common.Infrastructure.Services.Pagination;

public sealed class PaginationService(ShoplyDbContext context)
{
    public const int MaxPageSize = 100;

    public async Task<PaginationResult<TTarget>> GetPaginationResultAsync<TEntity, TTarget>(
        PaginationQuery paginationQuery,
        IMapper<TEntity, TTarget> mapper,
        CancellationToken cancellationToken) where TEntity : class
    {
        var result =
            await ExecutePaginationAsync(context.Set<TEntity>().AsNoTracking(), paginationQuery, cancellationToken);
        return MapResult(result, mapper.Map);
    }

    public async Task<PaginationResult<TTarget>> GetPaginationResultAsync<TEntity, TTarget>(
        PaginationQuery paginationQuery,
        IMapper<TEntity, TTarget> mapper,
        Func<IQueryable<TEntity>, IQueryable<TEntity>> filter,
        CancellationToken cancellationToken) where TEntity : class
    {
        var set = context.Set<TEntity>().AsNoTracking();
        set = filter(set);

        var result = await ExecutePaginationAsync(set, paginationQuery, cancellationToken);
        return MapResult(result, mapper.Map);
    }

    private static async Task<PaginationResult<T>> ExecutePaginationAsync<T>(
        IQueryable<T> queryable,
        PaginationQuery paginationQuery,
        CancellationToken cancellationToken) where T : class
    {
        PaginationQueryException.ThrowIfInvalidPaginationQuery(paginationQuery);

        var pageIndex = Math.Max(1, paginationQuery.PageIndex);
        var pageSize = Math.Clamp(paginationQuery.PageSize, 1, MaxPageSize);

        var totalCount = await queryable.AsNoTracking().CountAsync(cancellationToken);

        var items = await queryable
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        return new PaginationResult<T>(items, pageIndex, totalPages, totalCount);
    }

    private static PaginationResult<TTarget> MapResult<TSource, TTarget>(
        PaginationResult<TSource> source,
        Func<List<TSource>, List<TTarget>> mapFunc)
    {
        return new PaginationResult<TTarget>(
            mapFunc(source.Items),
            source.PageIndex,
            source.TotalPages,
            source.TotalCount);
    }
}