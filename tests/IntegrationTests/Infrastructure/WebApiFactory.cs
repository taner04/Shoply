using System.Data.Common;
using Api;
using IntegrationTests.Infrastructure.Mocks;
using IntegrationTests.Infrastructure.Mocks.Database;
using IntegrationTests.Infrastructure.Mocks.Jwt;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Logging;
using Refit;
using ServiceDefaults;

namespace IntegrationTests.Infrastructure;

public class WebApiFactory(DbConnection dbConnection, string azuriteConnectionString)
    : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(
        IWebHostBuilder builder)
    {
        builder.ConfigureLogging(opt =>
        {
            opt.ClearProviders();
            opt.AddConsole();
        });

        builder.ConfigureServices(services =>
        {
            services.AddMockDbContext(dbConnection);
            services.AddMockJwtBearerOptions();
            services.AddRefitClient<IApiClient>();
        });

        builder.UseSetting($"ConnectionStrings:{AppHostConstants.Database}", dbConnection.ConnectionString);
        builder.UseSetting($"ConnectionStrings:{AppHostConstants.AzureBlobStorage}", azuriteConnectionString);
    }
}