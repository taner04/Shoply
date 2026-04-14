using Shoply.MigrationService;
using Shoply.ServiceDefaults;
using Shoply.WebApi.Common.Infrastructure.Persistence;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddHostedService<Worker>();

builder.Services.AddOpenTelemetry().WithTracing(t => { t.AddSource(Worker.ActivitySourceName); });

builder.AddNpgsqlDbContext<ShoplyDbContext>(AppHostConstants.Database);

var host = builder.Build();

host.Run();