using Api.Common.Composition.Configs;
using Api.Common.Composition.Configs.OpenApi;
using Api.Common.Composition.Options;
using ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddOptions<Auth0Config>()
    .BindConfiguration(nameof(Auth0Config))
    .ValidateDataAnnotations();

builder.Services.AddOpenApi(OpenApiConfig.Config);

builder.Services.AddProblemDetails(ProblemDetailsConfig.Config);
builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthenticationAndAuthorization(builder.Configuration);

builder.Services.RegisterServices(builder);

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalar();
}

app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();
app.MapEndpoints();

app.Run();

namespace Api
{
    public class Program; // for integration testing purposes
}