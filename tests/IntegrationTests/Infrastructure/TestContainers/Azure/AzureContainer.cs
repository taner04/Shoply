using DotNet.Testcontainers.Builders;
using Testcontainers.Azurite;

namespace IntegrationTests.Infrastructure.TestContainers.Azure;

public sealed class AzureContainer : ContainerBase<AzuriteContainer>
{
    public string ConnectionString => Container.GetConnectionString();

    protected override AzuriteContainer BuildContainer()
    {
        return new AzuriteBuilder("mcr.microsoft.com/azure-storage/azurite:latest")
            .WithNetwork(new NetworkBuilder().Build())
            .WithNetworkAliases("azurite")
            .WithCommand(
                "azurite --blobHost 0.0.0.0 --queueHost 0.0.0.0 --tableHost 0.0.0.0 --loose --skipApiVersionCheck")
            .WithPortBinding(10000, 10000) // Blob service
            .WithPortBinding(10001, 10001) // Queue service
            .WithPortBinding(10002, 10002) // Table service
            .WithWaitStrategy(
                Wait.ForUnixContainer()
                    .UntilMessageIsLogged("Azurite Blob service is starting at http://0.0.0.0:10000")
            )
            .Build();
    }
}