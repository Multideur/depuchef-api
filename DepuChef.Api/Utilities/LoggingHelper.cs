using DepuChef.Application.Constants;

namespace DepuChef.Api.Utilities;

public static class LoggingHelper
{
    public static void LogCollectionValues<T>(ICollection<string[]> collection, ILogger<T> logger)
    {
        foreach (var item in collection)
        {
            string concatenatedValues = string.Join(", ", item);
            logger.LogWarning($"Validation errors: {{{LogToken.ValidationErrors}}}", concatenatedValues);
        }
    }
}