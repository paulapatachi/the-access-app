using System.Text.Json;

namespace MyApp.Infrastructure.Services.Files;

public class FileService : IFileService
{
    public async Task<T?> ReadJsonFileAsync<T>(string filePath, CancellationToken cancellationToken)
    {
        try
        {
            var stream = ReadFile(filePath);
            if (stream == null)
                return default;
            
            return await JsonSerializer.DeserializeAsync<T>(stream, cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading or deserializing JSON: {ex.Message}");
            return default;
        }
    }
    
    private static FileStream? ReadFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Console.WriteLine($"File not found at path: {filePath}");
            return null;
        }
        
        try
        {
            return File.OpenRead(filePath);
        }
        catch (IOException  ex)
        {
            Console.WriteLine($"Error reading file: {ex.Message}");
            return null;
        }
    }
}