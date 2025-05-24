using Microsoft.AspNetCore.Http;

namespace DepuChef.Application.Services;

public interface IStorageService
{
    Task<string> UploadFile(byte[] image, string fileName, CancellationToken cancellationToken);
    Task<string?> UploadFile(IFormFile file, string fileName, CancellationToken cancellationToken);
    Task<string?> UploadFileFromStream(Stream imageStream, string fileName, CancellationToken cancellationToken);
    Task<string> GetFileUrl(string fileName, CancellationToken cancellationToken);
    Task DeleteFile(string fileName, CancellationToken cancellationToken);
}
