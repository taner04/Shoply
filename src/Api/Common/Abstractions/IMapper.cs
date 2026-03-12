namespace Api.Common.Abstractions;

public interface IMapper<TEntity, TTarget>
{
    List<TTarget> Map(List<TEntity> source);
}