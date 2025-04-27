using MyApp.Domain.Enums;
using MyApp.Infrastructure.Commands;
using MyApp.Infrastructure.Helpers;

namespace MyApp.Infrastructure.Services.Management;

public class ManagementService : IManagementService
{
    private readonly IEnumerable<IManagementCommand> _commands;

    public ManagementService(IEnumerable<IManagementCommand> commands)
    {
        _commands = commands ?? throw new ArgumentNullException(nameof(commands));
    }

    public async Task ExecuteCommandAsync(string? userInputCommand, CancellationToken cancellationToken)
    {
        var generalCommand = CommandHelper.Parse(userInputCommand);
        if (generalCommand == null)
        {
            Console.WriteLine("Invalid command format.");
            return;
        }
        
        // Check if the command name is valid
        if (!Enum.TryParse(generalCommand.Name, out CommandType command))
        {
            Console.WriteLine($"Invalid command type: {generalCommand.Name}");
            return;
        }
        
        // Check if the command is registered
        var commandToExecute = _commands.FirstOrDefault(s => s.Command == command);
        if(commandToExecute == null)
        {
            Console.WriteLine($"Command not found: {command}");
            return;
        }

        try
        {
            // Execute the command
            await commandToExecute.ExecuteAsync(generalCommand.Parameters, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error executing command type {generalCommand.Name}: {ex.Message}");
        }
    }
}