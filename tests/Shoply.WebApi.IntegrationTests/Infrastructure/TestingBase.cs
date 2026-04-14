using Shoply.WebApi.Features.Users.Models;
using UserId = Shoply.WebApi.Features.Users.Models.UserId;

namespace Shoply.WebApi.IntegrationTests.Infrastructure;

[Collection("TestingFixtureCollection")]
public abstract class TestingBase(TestingFixture fixture) : IAsyncLifetime
{
    private IServiceScope _scope = null!;

    protected UserId CurrentUserId { get; private set; }

    protected static CancellationToken CurrentCancellationToken => TestContext.Current.CancellationToken;

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

    protected ShoplyDbContext GetDbContext() => _scope.ServiceProvider.GetRequiredService<ShoplyDbContext>();

    protected IApiClient CreateAuthenticatedUserClient() => fixture.CreateAuthenticatedClient(Policies.User);

    protected IApiClient CreateAuthenticatedAdminClient() => fixture.CreateAuthenticatedClient(Policies.Admin);

    protected IApiClient CreateUnauthenticatedClient() => fixture.CreateUnauthenticatedClient();

    protected async Task<UserId> CreateForeignUserAsync() => await fixture.CreateForeignUserAsync();

    protected T GetService<T>() where T : class => _scope.ServiceProvider.GetRequiredService<T>();
}