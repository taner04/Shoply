using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using System.Text.Json;
using Refit;
using Shoply.WebApi.Features.Users.Models;
using Shoply.WebApi.IntegrationTests.Infrastructure.Mocks.Jwt;
using Shoply.WebApi.IntegrationTests.Infrastructure.TestContainers.Postgres;

namespace Shoply.WebApi.IntegrationTests.Infrastructure.Fixtures;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public sealed class TestingFixture : IAsyncLifetime
{
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly PostgresTestDatabase _postgresTestDatabase = new();
    private readonly RefitSettings _refitSettings;

    private string _adminJwtToken = null!;
    private IServiceScopeFactory _serviceScopeFactory = null!;
    private string _userJwtToken = null!;
    private WebApiFactory _webApiFactory = null!;

    public TestingFixture()
    {
        _refitSettings = new RefitSettings(new SystemTextJsonContentSerializer(_jsonOptions));
    }

    public async ValueTask InitializeAsync()
    {
        InitilizeTokens();

        await _postgresTestDatabase.InitializeContainerAsync();

        _webApiFactory = new WebApiFactory(_postgresTestDatabase.DbConnection);
        _serviceScopeFactory = _webApiFactory.Services.GetRequiredService<IServiceScopeFactory>();
    }

    public async ValueTask DisposeAsync()
    {
        await _webApiFactory.DisposeAsync();
        await _postgresTestDatabase.DisposeAsync();
    }

    public async Task SetUpAsync()
    {
        await _postgresTestDatabase.ResetContainerAsync();
    }

    public async Task<UserId> CreateForeignUserAsync() => await _postgresTestDatabase.CreateForeignUserAsync();

    public IServiceScope CreateScope() => _serviceScopeFactory.CreateScope();

    public IApiClient CreateAuthenticatedClient(
        string role)
    {
        var client = _webApiFactory.CreateClient();

        var token = role switch
        {
            Security.Policies.User => _userJwtToken,
            Security.Policies.Admin => _adminJwtToken,
            _ => throw new ArgumentOutOfRangeException(nameof(role), "Invalid role specified.")
        };

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        return RestService.For<IApiClient>(client, _refitSettings);
    }

    public IApiClient CreateUnauthenticatedClient() =>
        RestService.For<IApiClient>(_webApiFactory.CreateClient(), _refitSettings);

    private void InitilizeTokens()
    {
        _userJwtToken = JwtTokenMock.CreateToken(UserFactory.Sub, UserRole.User);
        _adminJwtToken = JwtTokenMock.CreateToken(UserFactory.Sub, UserRole.Admin);
    }
}