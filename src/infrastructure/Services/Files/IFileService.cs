namespace MyApp.Infrastructure.Services.Files;

public interface IFileService
{
    Task<T?> ReadJsonFileAsync<T>(string filePath, CancellationToken cancellationToken);
}