using Projects;

var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<MigrationService>("migrationservice");
builder.AddProject<Api>("api");

builder.Build().Run();