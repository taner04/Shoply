using Projects;
using Shoply.ServiceDefaults;

var builder = DistributedApplication.CreateBuilder(args);

var db = builder.AddPostgres("database").WithPgAdmin();

var shoplyDb = db.AddDatabase(AppHostConstants.Database);

var migration = builder.AddProject<Shoply_MigrationService>(AppHostConstants.MigrationService)
    .WithReference(shoplyDb)
    .WaitFor(shoplyDb);

var papercut = builder.AddPapercutSmtp(AppHostConstants.Papercut, 80, 25);

builder.AddProject<Shoply_WebApi>(AppHostConstants.Api)
    .WithReference(shoplyDb)
    .WaitFor(shoplyDb)
    .WithReference(papercut)
    .WaitFor(papercut)
    .WaitForCompletion(migration);

builder.Build().Run();