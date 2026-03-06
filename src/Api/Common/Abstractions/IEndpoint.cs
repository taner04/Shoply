namespace Api.Common.Abstractions;

public interface IEndpoint
{
    void MapEndpoint(WebApplication app);
}