using DotNet.Testcontainers.Containers;

namespace IntegrationTests.Infrastructure.TestContainers;

public abstract class ContainerBase<T> : IAsyncLifetime where T : DockerContainer
{
    protected T Container = null!;

    public async ValueTask InitializeAsync()
    {
        Container = BuildContainer();
        await Container.StartAsync(TestsContext.CurrentCancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        await Container.StopAsync(TestsContext.CurrentCancellationToken);
        await Container.DisposeAsync();

        GC.SuppressFinalize(this);
    }

    protected abstract T BuildContainer();
}