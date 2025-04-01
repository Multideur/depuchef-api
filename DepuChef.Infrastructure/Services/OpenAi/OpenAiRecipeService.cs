using DepuChef.Application.Models;
using DepuChef.Application.Models.OpenAI.File;
using DepuChef.Application.Models.OpenAI;
using DepuChef.Application.Models.OpenAI.Thread;
using Microsoft.Extensions.Options;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using DepuChef.Application.Models.OpenAI.Message;
using DepuChef.Application.Models.OpenAI.CleanUp;
using DepuChef.Application.Services;
using DepuChef.Application.Services.OpenAi;
using DepuChef.Application.Repositories;
using DepuChef.Application.Constants;

namespace DepuChef.Infrastructure.Services.OpenAi;

public class OpenAiRecipeService(IFileManager fileManager,
    IThreadManager threadManager,
    IMessageManager messageManager,
    ICleanUpService cleanUpService,
    IClientNotifier clientNotifier,
    IProcessRepository processRepository,
    IUserRepository userRepository,
    IRecipeRepository recipeRepository,
    IOptions<OpenAiOptions> options,
    ILogger<OpenAiRecipeService> logger) : IAiRecipeService
{
    private readonly OpenAiOptions _openAiOptions = options.Value;
    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task CreateRecipeFromImage(BackgroundRecipeRequest recipeRequest, CancellationToken cancellationToken = default)
    {
        try
        {
            if (recipeRequest.ConnectionId is null)
            {
                logger.LogError("ConnectionId is required. Cannot notify client.");
                return;
            }

            if (recipeRequest.Image is null)
            {
                logger.LogError("Image is required.");
                return;
            }

            if (recipeRequest.Stream is null)
            {
                logger.LogError("Stream is required.");
                return;
            }

            var image = recipeRequest.Image;
            logger.LogInformation("Processing image with FileName: {fileName}", image.FileName);

            var fileUploadRequest = new FileUploadRequest
            {
                Purpose = "vision",
                File = image,
                Stream = recipeRequest.Stream
            };

            var fileUploadResponse = await UploadFile(fileUploadRequest, cancellationToken);
            if (fileUploadResponse is null || fileUploadResponse.Id is null)
            {
                logger.LogError("Failed to upload file.");
                return;
            }

            logger.LogInformation($"File uploaded successfully with Id: {{{LogToken.FileId}}}", fileUploadResponse.Id);

            var threadRequest = CreateThreadRequest(fileUploadResponse);
            var runResponse = await threadManager.CreateThreadAndRun(threadRequest, cancellationToken);
            if (runResponse is null || runResponse.ThreadId is null || runResponse.Id is null)
            {
                logger.LogError("Failed to create run");
                return;
            }

            var runStatusResponse = await threadManager.CheckRunStatus(runResponse.ThreadId,
                runResponse.Id,
                cancellationToken);

            if (runStatusResponse is null || runStatusResponse.Status is null)
            {
                logger.LogError("Failed to get run status");
                return;
            }

            runStatusResponse = await PollRunStatus(threadManager,
                runResponse,
                runStatusResponse,
                cancellationToken);

            logger.LogInformation($"Run completed successfully with ThreadId: {{{LogToken.ThreadId}}}", runResponse.ThreadId);

            var recipeProcess = await processRepository.SaveRecipeProcess(new RecipeProcess
            {
                ThreadId = runResponse.ThreadId,
                FileId = fileUploadResponse.Id,
                UserId = recipeRequest.UserId
            }, cancellationToken);

            if (recipeProcess is null)
            {
                logger.LogError($"Failed to save recipe process for {{{LogToken.ThreadId}}}", runResponse.ThreadId);
                logger.LogError("Failed to save recipe process");
                return;
            }

            var user = await userRepository.GetUser(recipeRequest.UserId, cancellationToken);
            if (user is null)
            {
                logger.LogError($"Failed to get user for UserId: {{{LogToken.UserId}}}", recipeRequest.UserId);
                return;
            }

            user.VirtualCoins -= 5;
            await userRepository.Update(user, cancellationToken);

            await clientNotifier.NotifyRecipeReady(recipeRequest.ConnectionId,
                recipeProcess.Id.ToString(),
                cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to create recipe from image");
            throw;
        }
    }

    public async Task CreateRecipeFromText(BackgroundRecipeRequest recipeRequest, CancellationToken cancellationToken = default)
    {
        try
        {
            if (recipeRequest.ConnectionId is null)
            {
                logger.LogError("ConnectionId is required. Cannot notify client.");
                return;
            }
            if (string.IsNullOrWhiteSpace(recipeRequest.Text))
            {
                logger.LogError("Text is required.");
                return;
            }
            var threadRequest = new ThreadRequest
            {
                AssistantId = _openAiOptions.AssistantId,
                Thread = new AiThread
                {
                    Messages =
                    [
                        new Message
                        {
                            Role = "user",
                            Content = recipeRequest.Text
                        }
                    ]
                }
            };
            var runResponse = await threadManager.CreateThreadAndRun(threadRequest, cancellationToken);
            if (runResponse is null || runResponse.ThreadId is null || runResponse.Id is null)
            {
                logger.LogError("Failed to create run");
                return;
            }
            var runStatusResponse = await threadManager.CheckRunStatus(runResponse.ThreadId,
                runResponse.Id,
                cancellationToken);
            if (runStatusResponse is null || runStatusResponse.Status is null)
            {
                logger.LogError("Failed to get run status");
                return;
            }
            runStatusResponse = await PollRunStatus(threadManager,
                runResponse,
                runStatusResponse,
                cancellationToken);
            logger.LogInformation($"Run completed successfully with ThreadId: {{{LogToken.ThreadId}}}", runResponse.ThreadId);
            var recipeProcess = await processRepository.SaveRecipeProcess(new RecipeProcess
            {
                ThreadId = runResponse.ThreadId,
                UserId = recipeRequest.UserId
            }, cancellationToken);
            if (recipeProcess is null)
            {
                logger.LogError($"Failed to save recipe process for {{{LogToken.ThreadId}}}", runResponse.ThreadId);
                logger.LogError("Failed to save recipe process");
                return;
            }
            await clientNotifier.NotifyRecipeReady(recipeRequest.ConnectionId,
                recipeProcess.Id.ToString(),
                cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to create recipe from search");
            throw;
        }
    }

    public async Task<Recipe?> GetRecipeByProcessId(Guid processId, CancellationToken cancellationToken = default)
    {
        var process = await processRepository.GetRecipeProcess(x => x.Id == processId, cancellationToken);
        if (process is null)
        {
            logger.LogWarning($"No recipe process found for process {{{LogToken.RecipeProcessId}}}.", processId);
            return null;
        }

        var threadId = process.ThreadId;
        var messages = await messageManager.GetMessages(threadId, cancellationToken);
        if (messages is null)
        {
            logger.LogWarning($"Failed to get messages for thread {{{LogToken.ThreadId}}}.", threadId);
            return null;
        }

        var recipe = ((messages
            .LastOrDefault(m => m.Role == "assistant")?
            .Content as ContentItem[] ?? [])
            .LastOrDefault(c => c.Type == "text")?
            .Text?
            .Value)
            ?? throw new Exception($"Failed to get recipe for thread {threadId}.");

        var deserializeRecipe = JsonSerializer.Deserialize<Recipe>(recipe, _jsonSerializerOptions);
        if (deserializeRecipe?.Title is null)
        {
            logger.LogWarning("Could not deserialize the recipe for thread {threadId}.", threadId);
            var error = JsonSerializer.Deserialize<RecipeError>(recipe, _jsonSerializerOptions);
            if (error is not null)
            {
                logger.LogError($"Error on generating recipe for thread {{{LogToken.ThreadId}}}. Message: {{{LogToken.Message}}}",
                    threadId,
                    error.Message);
            }
            else
            {
                logger.LogError($"Could not retrieve an error for why recipe was not generated on thread {{{LogToken.ThreadId}}}.",
                    threadId);
            }
            return null;
        }

        deserializeRecipe.UserId = process.UserId;
        var savedRecipe = await recipeRepository.Add(deserializeRecipe, cancellationToken);

        _ = CleanUp(threadId, cancellationToken);

        return savedRecipe;
    }

    private static async Task<RunResponse?> PollRunStatus(IThreadManager threadManager,
        RunResponse runResponse,
        RunResponse? runStatusResponse,
        CancellationToken cancellationToken)
    {
        int retries = 30;
        while (runStatusResponse?.Status != "completed")
        {
            await Task.Delay(1000, cancellationToken);
            var threadId = runResponse.ThreadId;
            var runResponseId = runResponse.Id;

            runStatusResponse = await threadManager.CheckRunStatus(threadId!,
                runResponseId!,
                cancellationToken);

            if (retries-- <= 0)
            {
                throw new Exception("Run status check retries exceeded");
            }
        }

        return runStatusResponse;
    }

    private ThreadRequest CreateThreadRequest(FileUploadResponse fileUploadResponse) =>
        new()
        {
            AssistantId = _openAiOptions.AssistantId,
            Thread = new AiThread
            {
                Messages =
                [
                    new Message
                    {
                        Role = "user",
                        Content = new ContentItem[]
                        {
                            new() {
                                Type = "image_file",
                                ImageFile = new ImageFileReference
                                {
                                    FileId = fileUploadResponse.Id
                                }
                            } 
                        }
                    }
                ]
            }
        };

    private async Task<FileUploadResponse?> UploadFile(FileUploadRequest fileUploadRequest, CancellationToken cancellationToken = default)
    {
        var fileUploadResponse = await fileManager.UploadFile(fileUploadRequest, cancellationToken);

        fileUploadRequest.Stream?.Dispose();

        return fileUploadResponse;
    }

    private async Task CleanUp(string threadId,
        CancellationToken cancellationToken)
    {
        var process = await processRepository.GetRecipeProcess(x => x.ThreadId == threadId, cancellationToken);
        if (process is null)
        {
            logger.LogWarning($"No process found for thread {{{LogToken.ThreadId}}}.", threadId);
            return;
        }

        var cleanUpRequest = new CleanUpRequest
        {
            FileId = process.FileId,
            ThreadId = threadId
        };

        await cleanUpService.CleanUp(cleanUpRequest, cancellationToken);
    }
}
