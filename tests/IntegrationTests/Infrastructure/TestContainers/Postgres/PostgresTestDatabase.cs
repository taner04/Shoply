using System.Data.Common;
using Api.Common.Domain.Users;
using Api.Common.Infrastructure.Persistence;
using IntegrationTests.Factories;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace IntegrationTests.Infrastructure.TestContainers.Postgres;

public sealed class PostgresTestDatabase : IAsyncDisposable
{
    private readonly PostgresContainer _postgresContainer = new();
    private DbContextOptions<ApplicationDbContext> _dbContextOptions = null!;

    public DbConnection DbConnection => new NpgsqlConnection(_postgresContainer.ConnectionString);

    public async ValueTask DisposeAsync()
    {
        await _postgresContainer.DisposeAsync();
    }

    public async Task InitializeContainerAsync()
    {
        await _postgresContainer.InitializeAsync();

        _dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(_postgresContainer.ConnectionString)
            .Options;

        await using var context = new ApplicationDbContext(_dbContextOptions);
        await context.Database.MigrateAsync(TestsContext.CurrentCancellationToken);

        await InitUserAsync(context);
    }

    public async Task ResetContainerAsync()
    {
        await using var context = new ApplicationDbContext(_dbContextOptions);

        const string sql = $"""
                            DELETE FROM "Orders";
                            DELETE FROM "OrderItems";
                            DELETE FROM "Users";
                            """;

        await context.Database.ExecuteSqlRawAsync(sql, TestsContext.CurrentCancellationToken);

        await InitUserAsync(context);
    }

    public async Task<UserId> CreateForeignUserAsync()
    {
        await using var context = new ApplicationDbContext(_dbContextOptions);

        var user = User.Create("otheruser@mail.com", "auth0|otheruserid123");
        user.SetCreated("auth0|otheruserid123");
        user.Basket.SetCreated("auth0|otheruserid123");

        context.Users.Add(user);
        await context.SaveChangesAsync(TestsContext.CurrentCancellationToken);

        return user.Id;
    }

    private static async Task InitUserAsync(
        ApplicationDbContext context)
    {
        var user = UserFactory.Create();
        user.SetCreated(UserFactory.Sub);
        user.Basket.SetCreated(UserFactory.Sub);

        context.Users.Add(user);
        await context.SaveChangesAsync(TestsContext.CurrentCancellationToken);
    }
}