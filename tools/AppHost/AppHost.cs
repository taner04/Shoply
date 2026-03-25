using Projects;
using ServiceDefaults;

var builder = DistributedApplication.CreateBuilder(args);

var db = builder.AddPostgres("database").WithPgAdmin();

var shoplyDb = db.AddDatabase(AppHostConstants.Database);

var migration = builder.AddProject<MigrationService>(AppHostConstants.MigrationService)
    .WithReference(shoplyDb)
    .WaitFor(shoplyDb);

var papercut = builder.AddPapercutSmtp(AppHostConstants.Papercut, 80, 25);

builder.AddProject<Api>(AppHostConstants.Api)
    .WithReference(shoplyDb)
    .WaitFor(shoplyDb)
    .WithReference(papercut)
    .WaitFor(papercut)
    .WaitForCompletion(migration);

builder.Build().Run();