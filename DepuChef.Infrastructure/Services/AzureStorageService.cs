using Azure.Identity;
using Azure.Storage.Blobs;
using DepuChef.Application;
using DepuChef.Application.Services;
using Microsoft.Extensions.Options;

namespace DepuChef.Infrastructure.Services;

public class AzureStorageService : IStorageService
{
    private readonly IOptions<StorageOptions> _options;

    public AzureStorageService(
        IOptions<StorageOptions> options
    )
    {
        _options = options;
        ArgumentException.ThrowIfNullOrWhiteSpace(options.Value.AccountName, nameof(options.Value.AccountName));
        ArgumentException.ThrowIfNullOrWhiteSpace(options.Value.ContainerName, nameof(options.Value.ContainerName));
    }

    public async Task<string> UploadFile(byte[] image, string fileName, CancellationToken cancellationToken)
    {
        if (image == null || image.Length == 0)
        {
            throw new ArgumentException("Image is empty", nameof(image));
        }

        var client = GetBlobContainerClient(_options.Value.AccountName!, _options.Value.ContainerName!);
        var blob = client.GetBlobClient(fileName);
        using var stream = new MemoryStream(image);
        await blob.UploadAsync(stream, true, cancellationToken);
        
        return blob.Uri.AbsoluteUri;
    }
    public async Task<string> UploadFileFromStream(Stream fileStream, string fileName, CancellationToken cancellationToken)
    {
        if (fileStream == null || fileStream.Length == 0)
        {
            throw new ArgumentException("File stream is empty", nameof(fileStream));
        }

        var client = GetBlobContainerClient(_options.Value.AccountName!, _options.Value.ContainerName!);
        var blob = client.GetBlobClient(fileName);
        await blob.UploadAsync(fileStream, true, cancellationToken);

        return blob.Uri.AbsoluteUri;
    }
    public Task<string> GetFileUrl(string fileName, CancellationToken cancellationToken)
    {
        var client = GetBlobContainerClient(_options.Value.AccountName!, _options.Value.ContainerName!);
        var blob = client.GetBlobClient(fileName);

        return Task.FromResult(blob.Uri.AbsoluteUri);
    }
    public Task DeleteFile(string fileName, CancellationToken cancellationToken)
    {
        var client = GetBlobContainerClient(_options.Value.AccountName!, _options.Value.ContainerName!);
        var blob = client.GetBlobClient(fileName);
        
        return blob.DeleteIfExistsAsync(cancellationToken: cancellationToken);
    }

    private static BlobServiceClient GetBlobServiceClient(string accountName)
    {
        BlobServiceClient client = new(
            new Uri($"https://{accountName}.blob.core.windows.net"),
            new DefaultAzureCredential());

        return client;
    }

    private static BlobContainerClient GetBlobContainerClient(string accountName, string containerName)
    {
        BlobServiceClient client = GetBlobServiceClient(accountName);
        BlobContainerClient container = client.GetBlobContainerClient(containerName);
        return container;
    }
}
