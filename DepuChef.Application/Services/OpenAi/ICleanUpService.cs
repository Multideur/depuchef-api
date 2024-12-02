using DepuChef.Application.Models.OpenAI.CleanUp;

namespace DepuChef.Application.Services.OpenAi;

public interface ICleanUpService
{
    Task CleanUp(CleanUpRequest cleanUpRequest, CancellationToken cancellationToken);
}
