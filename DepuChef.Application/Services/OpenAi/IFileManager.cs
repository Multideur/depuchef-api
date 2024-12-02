using DepuChef.Application.Models.OpenAI.File;

namespace DepuChef.Application.Services.OpenAi;

public interface IFileManager
{
    public Task<FileUploadResponse?> UploadFile(FileUploadRequest fileUploadRequest, CancellationToken cancellationToken);
    public Task<DeleteFileResponse?> DeleteFile(string fileId, CancellationToken cancellationToken);
}
