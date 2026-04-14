namespace Shoply.WebApi.Common.Abstractions;

public interface IMapper<TEntity, TTarget>
{
    List<TTarget> Map(List<TEntity> source);
}