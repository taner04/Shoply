using Projects;
using ServiceDefaults;

var builder = DistributedApplication.CreateBuilder(args);

var db = builder.AddPostgres("database").WithPgAdmin();

var shoplyDb = db.AddDatabase(AppHostConstants.Database);

var migration = builder.AddProject<MigrationService>(AppHostConstants.MigrationService)
    .WithReference(shoplyDb)
    .WaitFor(shoplyDb);

var blobStorage = builder.AddContainer(AppHostConstants.Azure, "mcr.microsoft.com/azure-storage/azurite:latest")
    .WithVolume("/data")
    .WithEntrypoint("azurite")
    .WithArgs("--blobHost", "0.0.0.0",
        "--queueHost", "0.0.0.0",
        "--tableHost", "0.0.0.0",
        "--loose",
        "--skipApiVersionCheck")
    .WithHttpEndpoint(10000, 10000, "blob") // Blob service 
    .WithHttpEndpoint(10001, 10001, "queue") // Queue service
    .WithHttpEndpoint(10002, 10002, "table"); // Table service


builder.AddProject<Api>(AppHostConstants.Api)
    .WithReference(shoplyDb)
    .WaitFor(shoplyDb)
    .WaitFor(blobStorage)
    .WaitForCompletion(migration);

builder.Build().Run();