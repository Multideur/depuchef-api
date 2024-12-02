using DepuChef.Application.Models.OpenAI.Thread;

namespace DepuChef.Application.Services.OpenAi;

public interface IThreadManager
{
    public Task<ThreadResponse?> CreateThread(ThreadRequest threadRequest, CancellationToken cancellationToken);
    public Task<RunResponse?> CreateRun(RunRequest runRequest, CancellationToken cancellationToken);
    public Task<RunResponse?> CreateThreadAndRun(ThreadRequest threadRequest, CancellationToken cancellationToken);
    public Task<RunResponse?> CheckRunStatus(string threadId, string runId, CancellationToken cancellationToken);
    public Task<DeleteThreadResponse?> DeleteThread(string threadId, CancellationToken cancellationToken);
}