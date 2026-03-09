using DotNet.Testcontainers.Containers;

namespace IntegrationTests.Infrastructure.TestContainers;

public abstract class ContainerBase<T> : IAsyncLifetime where T : DockerContainer
{
    private const int MaxRetryAttempts = 5;
    protected T Container = null!;

    public async ValueTask InitializeAsync()
    {
        Container = BuildContainer();


        var attempts = 0;
        while (true)
        {
            try
            {
                await Container.StartAsync(TestsContext.CurrentCancellationToken);
                break; // Exit the loop if the container starts successfully
            }
            catch (Exception)
            {
                attempts++;
                if (attempts >= MaxRetryAttempts)
                {
                    throw;
                }

                await Task.Delay(TimeSpan.FromSeconds(5), TestsContext.CurrentCancellationToken);
            }
        }
    }

    public async ValueTask DisposeAsync()
    {
        await Container.StopAsync(TestsContext.CurrentCancellationToken);
        await Container.DisposeAsync();

        GC.SuppressFinalize(this);
    }

    protected abstract T BuildContainer();
}