using MyApp.Infrastructure.Services.Management;

namespace MyApp;

public class MainApp
{
    private readonly IManagementService _managementService;

    public MainApp(IManagementService managementService)
    {
        _managementService = managementService ?? throw new ArgumentNullException(nameof(managementService));
    }

    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        Console.WriteLine("Application started...");
        
        while (true)
        {
            var input = Console.ReadLine();

            // Exit if the input is blank
            if (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine("Exiting...");
                break;
            }
            
            // Execute the command
            await _managementService.ExecuteCommandAsync(input, cancellationToken).ConfigureAwait(false);
        }
    }
}