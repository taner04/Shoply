using DotNet.Testcontainers.Builders;
using Testcontainers.PostgreSql;

namespace IntegrationTests.Infrastructure.TestContainers.Postgres;

public class PostgresContainer : ContainerBase<PostgreSqlContainer>
{
    public string ConnectionString => Container.GetConnectionString();

    protected override PostgreSqlContainer BuildContainer()
    {
        return new PostgreSqlBuilder("postgres:latest")
            .WithDatabase("db")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .WithWaitStrategy(Wait.ForUnixContainer().UntilCommandIsCompleted("pg_isready"))
            .WithCleanUp(true)
            .Build();
    }
}