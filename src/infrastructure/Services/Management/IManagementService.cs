namespace MyApp.Infrastructure.Services.Management;

public interface IManagementService
{
    Task ExecuteCommandAsync(string? input, CancellationToken cancellationToken);
}