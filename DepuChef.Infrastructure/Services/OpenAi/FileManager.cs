using DepuChef.Application.Models.OpenAI;
using DepuChef.Application.Models.OpenAI.File;
using DepuChef.Application.Services;
using DepuChef.Application.Services.OpenAi;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text.Json;

namespace DepuChef.Infrastructure.Services.OpenAi;

public class FileManager(IHttpService httpService,
    IOptions<OpenAiOptions> options,
    ILogger<FileManager> logger) : BaseOpenAiService(options), IFileManager
{
    public async Task<FileUploadResponse?> UploadFile(FileUploadRequest fileUploadRequest, CancellationToken cancellationToken)
    {
        logger.LogInformation("Uploading file with Purpose: {purpose}", fileUploadRequest.Purpose);

        if (fileUploadRequest.File is null)
            throw new Exception("File is required");

        if (fileUploadRequest.Stream is null)
            throw new Exception("Stream is required");

        var fileUrl = $"{_openAiOptions.BaseUrl}/files";
        var response = await PostFile(fileUrl,
            fileUploadRequest,
            cancellationToken: cancellationToken);

        if (!response.IsSuccessStatusCode)
            throw new Exception("Failed to upload file");

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<FileUploadResponse>(responseContent);
    }

    public async Task<DeleteFileResponse?> DeleteFile(string fileId, CancellationToken cancellationToken)
    {
        var fileUrl = $"{_openAiOptions.BaseUrl}/files/{fileId}";
        var response = await httpService.DeleteAsync(fileUrl, 
            cancellationToken: cancellationToken);

        if (!response.IsSuccessStatusCode)
            return null;

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<DeleteFileResponse>(responseContent);
    }

    private async Task<HttpResponseMessage> PostFile(string url, FileUploadRequest fileUploadRequest, CancellationToken cancellationToken = default)
    {
        var file = fileUploadRequest.File
            ?? throw new Exception("File is required");

        logger.LogInformation("Opening read stream for file: {fileName}", file.FileName);

        var fileStream = new StreamContent(fileUploadRequest.Stream!);
        fileStream.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);

        using var content = new MultipartFormDataContent
        {
            { fileStream, "file", file.FileName },
            { new StringContent(fileUploadRequest.Purpose!), "purpose" }
        };

        logger.LogInformation("Posting file to URL: {Url}", url);

        return await httpService.PostAsync(url, 
            content, 
            cancellationToken: cancellationToken);
    }
}
