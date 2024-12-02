using DepuChef.Application.Models.OpenAI.Message;

namespace DepuChef.Application.Services.OpenAi;

public interface IMessageManager
{
    public Task<List<Message>?> GetMessages(string threadId, CancellationToken cancellationToken);
}
