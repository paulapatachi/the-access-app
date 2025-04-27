namespace MyApp.Infrastructure.Helpers;

public static class FileHelper
{
    public static string GetFullPath(string fileName)
    {
        var basePath = Directory.GetCurrentDirectory();
        var relativePath = Path.Combine(basePath, @$"..\..\..\..\..\files\{fileName}");
        return Path.GetFullPath(relativePath);
    }
}