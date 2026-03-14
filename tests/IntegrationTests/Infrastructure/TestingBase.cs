using Api.Features.Users.Models;
using UserId = Api.Features.Users.Models.UserId;

namespace IntegrationTests.Infrastructure;

[Collection("TestingFixtureCollection")]
public abstract class TestingBase(TestingFixture fixture) : IAsyncLifetime
{
    private IServiceScope _scope = null!;

    protected UserId CurrentUserId { get; private set; }

    protected static CancellationToken CurrentCancellationToken => TestsContext.CurrentCancellationToken;

    public async ValueTask InitializeAsync()
    {
        await fixture.SetUpAsync();

        _scope = fixture.CreateScope();

        CurrentUserId = await GetDbContext().Users
            .Select(u => u.Id)
            .FirstAsync();
    }

    public ValueTask DisposeAsync()
    {
        _scope.Dispose();
        GC.SuppressFinalize(this);

        return ValueTask.CompletedTask;
    }

    protected ApplicationDbContext GetDbContext()
    {
        return _scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    }

    protected IApiClient CreateAuthenticatedUserClient()
    {
        return fixture.CreateAuthenticatedClient(Policies.User);
    }

    protected IApiClient CreateAuthenticatedAdminClient()
    {
        return fixture.CreateAuthenticatedClient(Policies.Admin);
    }

    protected IApiClient CreateUnauthenticatedClient()
    {
        return fixture.CreateUnauthenticatedClient();
    }

    protected async Task<UserId> CreateForeignUserAsync()
    {
        return await fixture.CreateForeignUserAsync();
    }

    protected T GetService<T>() where T : class
    {
        return _scope.ServiceProvider.GetRequiredService<T>();
    }
}