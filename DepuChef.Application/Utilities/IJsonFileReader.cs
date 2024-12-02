namespace DepuChef.Application.Utilities;

public interface IJsonFileReader
{
    Task<T?> ReadJsonFileAsync<T>(string filePath);
}
