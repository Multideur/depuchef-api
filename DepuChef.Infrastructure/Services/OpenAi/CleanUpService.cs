using DepuChef.Application.Models.OpenAI;
using DepuChef.Application.Models.OpenAI.CleanUp;
using DepuChef.Application.Services.OpenAi;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DepuChef.Infrastructure.Services.OpenAi;

public class CleanUpService(IFileManager fileManager,
    IThreadManager threadManager,
    IOptions<OpenAiOptions> options,
    ILogger<CleanUpService> logger) : BaseOpenAiService(options), ICleanUpService
{
    public async Task CleanUp(CleanUpRequest cleanUpRequest, CancellationToken cancellationToken)
    {
        logger.LogInformation("Clean up started.");

        if (!string.IsNullOrEmpty(cleanUpRequest.FileId))
        {
            await DeleteFile(cleanUpRequest.FileId, cancellationToken); 
        }
        await DeleteThread(cleanUpRequest.ThreadId, cancellationToken);
    }

    private async Task DeleteFile(string fileId, CancellationToken cancellationToken)
    {
        logger.LogInformation("Deleting file with Id: {fileId}", fileId);
        var fileDeleteResponse = await fileManager.DeleteFile(fileId, cancellationToken);

        if (fileDeleteResponse?.Deleted ?? false)
            logger.LogInformation("File deleted successfully");
        else
            logger.LogError("Failed to delete file");
    }

    private async Task DeleteThread(string threadId, CancellationToken cancellationToken)
    {
        logger.LogInformation("Deleting thread with Id: {threadId}", threadId);
        var threadDeleteResponse = await threadManager.DeleteThread(threadId, cancellationToken);

        if (threadDeleteResponse?.Deleted ?? false)
            logger.LogInformation("Thread deleted successfully");
        else
            logger.LogError("Failed to delete thread");
    }
}
