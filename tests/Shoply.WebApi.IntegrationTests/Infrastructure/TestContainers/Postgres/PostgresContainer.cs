using DotNet.Testcontainers.Builders;
using Testcontainers.PostgreSql;

namespace Shoply.WebApi.IntegrationTests.Infrastructure.TestContainers.Postgres;

public class PostgresContainer : ContainerBase<PostgreSqlContainer>
{
    public string ConnectionString => Container.GetConnectionString();

    protected override PostgreSqlContainer BuildContainer() =>
        new PostgreSqlBuilder("postgres:latest")
            .WithDatabase("db")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .WithWaitStrategy(Wait.ForUnixContainer().UntilCommandIsCompleted("pg_isready"))
            .WithCleanUp(true)
            .Build();
}