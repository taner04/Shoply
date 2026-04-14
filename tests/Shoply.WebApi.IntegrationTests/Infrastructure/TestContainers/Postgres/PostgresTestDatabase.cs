using Npgsql;
using Shoply.WebApi.Features.Users.Models;
using UserId = Shoply.WebApi.Features.Users.Models.UserId;

namespace Shoply.WebApi.IntegrationTests.Infrastructure.TestContainers.Postgres;

public sealed class PostgresTestDatabase : IAsyncDisposable
{
    private readonly PostgresContainer _postgresContainer = new();
    private DbContextOptions<ShoplyDbContext> _dbContextOptions = null!;

    public DbConnection DbConnection => new NpgsqlConnection(_postgresContainer.ConnectionString);

    public async ValueTask DisposeAsync()
    {
        await _postgresContainer.DisposeAsync();
    }

    public async Task InitializeContainerAsync()
    {
        await _postgresContainer.InitializeAsync();

        _dbContextOptions = new DbContextOptionsBuilder<ShoplyDbContext>()
            .UseNpgsql(_postgresContainer.ConnectionString)
            .Options;

        await using var context = new ShoplyDbContext(_dbContextOptions);
        await context.Database.MigrateAsync(TestContext.Current.CancellationToken);

        await InitUserAsync(context);
    }

    public async Task ResetContainerAsync()
    {
        await using var context = new ShoplyDbContext(_dbContextOptions);

        const string sql = """
                           DELETE FROM "OrderItems";
                           DELETE FROM "Payments";
                           DELETE FROM "Orders";
                           DELETE FROM "BasketItems";
                           DELETE FROM "Baskets";
                           DELETE FROM "Products";
                           DELETE FROM "Users";
                           """;

        await context.Database.ExecuteSqlRawAsync(sql, TestContext.Current.CancellationToken);

        await InitUserAsync(context);
    }

    public async Task<UserId> CreateForeignUserAsync()
    {
        await using var context = new ShoplyDbContext(_dbContextOptions);

        var user = User.Create("otheruser@mail.com", "auth0|otheruserid123");
        user.SetCreated("auth0|otheruserid123");
        user.Basket.SetCreated("auth0|otheruserid123");

        context.Users.Add(user);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        return user.Id;
    }

    private static async Task InitUserAsync(
        ShoplyDbContext context)
    {
        var user = UserFactory.Create();
        user.SetCreated(UserFactory.Sub);
        user.Basket.SetCreated(UserFactory.Sub);

        context.Users.Add(user);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);
    }
}