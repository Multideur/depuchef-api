using Azure.Identity;
using Azure.Storage.Blobs;
using DepuChef.Application;
using DepuChef.Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DepuChef.Infrastructure.Services;

public class AzureStorageService : IStorageService
{
    private readonly IOptions<StorageOptions> _options;
    private readonly ILogger<AzureStorageService> _logger;

    public AzureStorageService(
        IOptions<StorageOptions> options,
        ILogger<AzureStorageService> logger
    )
    {
        _options = options;
        ArgumentException.ThrowIfNullOrWhiteSpace(options.Value.AccountName, nameof(options.Value.AccountName));
        ArgumentException.ThrowIfNullOrWhiteSpace(options.Value.ContainerName, nameof(options.Value.ContainerName));
        _logger = logger;
    }

    public async Task<string> UploadFile(byte[] file, string fileName, CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
        {
            throw new ArgumentException("File is empty", nameof(file));
        }

        var client = GetBlobContainerClient(_options.Value.AccountName!, _options.Value.ContainerName!);
        var blob = client.GetBlobClient(fileName);
        using var stream = new MemoryStream(file);
        await blob.UploadAsync(stream, true, cancellationToken);
        
        return blob.Uri.AbsoluteUri;
    }

    public async Task<string?> UploadFile(IFormFile file, string fileName, CancellationToken cancellationToken)
    {
        if (!_options.Value.Enabled)
        {
            _logger.LogWarning("Azure Storage is not enabled. Skipping upload.");
            return null;
        }
        if (file == null || file.Length == 0)
        {
            throw new ArgumentException("File is empty", nameof(file));
        }

        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new ArgumentException("File name is empty", nameof(fileName));
        }

        if (fileName.Contains(".."))
        {
            throw new ArgumentException("File name cannot contain '..'", nameof(fileName));
        }

        var client = GetBlobContainerClient(_options.Value.AccountName!, _options.Value.ContainerName!);
        var blob = client.GetBlobClient(fileName);
        using var stream = file.OpenReadStream();
        await blob.UploadAsync(stream, true, cancellationToken);
        return blob.Uri.AbsoluteUri;
    }

    public async Task<string?> UploadFileFromStream(Stream fileStream, string fileName, CancellationToken cancellationToken)
    {
        if (!_options.Value.Enabled)
        {
            _logger.LogWarning("Azure Storage is not enabled. Skipping upload.");
            return null;
        }

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
