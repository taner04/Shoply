using System.Net.Mail;
using Api.Common.Domain.Products;
using Azure.Storage.Blobs;

namespace IntegrationTests.Infrastructure.TestContainers.Azure;

internal sealed class AzureTestBlobStorage : IAsyncDisposable
{
    private readonly AzureContainer _azureContainer = new();
    private BlobContainerClient _blobContainerClient = null!;

    public string ConnectionString => _azureContainer.ConnectionString;

    public async ValueTask DisposeAsync()
    {
        await _azureContainer.DisposeAsync();
    }

    public async Task InitializeContainerAsync()
    {
        await _azureContainer.InitializeAsync();

        var options = new BlobClientOptions(BlobClientOptions.ServiceVersion.V2024_11_04);
        var blobServiceClient = new BlobServiceClient(_azureContainer.ConnectionString, options);

        _blobContainerClient = blobServiceClient.GetBlobContainerClient(Product.DefaultContainer);
        if (!await _blobContainerClient.ExistsAsync())
        {
            await _blobContainerClient.CreateAsync();
        }
    }

    public async Task ResetContainerAsync()
    {
        await foreach (var blob in _blobContainerClient.GetBlobsAsync(
                           cancellationToken: TestsContext.CurrentCancellationToken))
        {
            var blobClient = _blobContainerClient.GetBlobClient(blob.Name);
            await blobClient.DeleteIfExistsAsync(cancellationToken: TestsContext.CurrentCancellationToken);
        }
    }
}