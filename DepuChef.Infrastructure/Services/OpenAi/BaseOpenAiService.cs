using DepuChef.Application.Models.OpenAI;
using DepuChef.Application.Utilities;
using Microsoft.Extensions.Options;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace DepuChef.Infrastructure.Services.OpenAi;

public abstract class BaseOpenAiService(IOptions<OpenAiOptions> options)
{
    protected readonly OpenAiOptions _openAiOptions = options.Value;
    protected readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
        Converters = { new UnixTimestampToDateTimeConverter() }
    };
}
