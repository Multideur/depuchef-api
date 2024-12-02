using System.Text.Json;

namespace DepuChef.Application.Utilities;

public class JsonFileReader : IJsonFileReader
{
    public async Task<T?> ReadJsonFileAsync<T>(string filePath)
    {
        try
        {
            using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            using var streamReader = new StreamReader(fileStream);
            var json = await streamReader.ReadToEndAsync();
            return JsonSerializer.Deserialize<T>(json);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return default;
        }
    }
}
