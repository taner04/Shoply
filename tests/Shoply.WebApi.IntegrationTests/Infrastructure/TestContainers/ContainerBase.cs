using DotNet.Testcontainers.Containers;

namespace Shoply.WebApi.IntegrationTests.Infrastructure.TestContainers;

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
                await Container.StartAsync(TestContext.Current.CancellationToken);
                break; // Exit the loop if the container starts successfully
            }
            catch (Exception)
            {
                attempts++;
                if (attempts >= MaxRetryAttempts)
                {
                    throw;
                }

                await Task.Delay(TimeSpan.FromSeconds(5), TestContext.Current.CancellationToken);
            }
        }
    }

    public async ValueTask DisposeAsync()
    {
        await Container.StopAsync(TestContext.Current.CancellationToken);
        await Container.DisposeAsync();

        GC.SuppressFinalize(this);
    }

    protected abstract T BuildContainer();
}