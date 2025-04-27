using MyApp.Domain.Enums;

namespace MyApp.Infrastructure.Commands;

public interface IManagementCommand
{
    CommandType Command { get; }
    Task ExecuteAsync(string[] args, CancellationToken cancellationToken);
}