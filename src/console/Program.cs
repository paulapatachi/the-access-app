using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MyApp.Infrastructure.Commands;
using MyApp.Infrastructure.Services.Bookings;
using MyApp.Infrastructure.Services.Files;
using MyApp.Infrastructure.Services.Hotels;
using MyApp.Infrastructure.Services.Management;

namespace MyApp;

public class Program
{
    public static async Task Main(string[] args)
    {
        using var host = CreateHostBuilder(args).Build();

        var app = host.Services.GetRequiredService<MainApp>();

        await app.RunAsync(new CancellationToken(false));
    }

    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, config) =>
            {
                config.AddCommandLine(args);
            })
            .ConfigureServices((_, services) =>
            {
                services.AddTransient<IFileService, FileService>();
                services.AddTransient<IHotelService, HotelService>();
                services.AddTransient<IBookingService, BookingService>();
                services.AddTransient<IManagementService, ManagementService>();
                services.AddTransient<IManagementCommand, AvailabilityCommand>();
                services.AddTransient<IManagementCommand, RoomTypesCommand>();
                services.AddTransient<MainApp>();
            });
}