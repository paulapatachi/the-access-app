using MyApp.Domain.Enums;
using MyApp.Domain.Filters;
using MyApp.Infrastructure.Helpers;
using MyApp.Infrastructure.Services.Bookings;
using MyApp.Infrastructure.Services.Hotels;

namespace MyApp.Infrastructure.Commands;

public class AvailabilityCommand : IManagementCommand
{
    public CommandType Command => CommandType.Availability;
    
    private readonly IHotelService _hotelService;
    private readonly IBookingService _bookingService;

    public AvailabilityCommand(IHotelService hotelService, IBookingService bookingService)
    {
        _hotelService = hotelService ?? throw new ArgumentNullException(nameof(hotelService));
        _bookingService = bookingService ?? throw new ArgumentNullException(nameof(bookingService));
    }

    public async Task ExecuteAsync(string[] args, CancellationToken cancellationToken)
    {
        if(args == null || args.Length == 0)
        {
            throw new ArgumentException("No arguments provided.");
        }
        
        if(args.Length < 3)
        {
            throw new ArgumentException("Insufficient arguments provided.");
        }
        
        // Create filter based on input arguments
        var availabilityFilter = GetAvailabilityFilter(args);

        // Get availability based on filter
        var availabilityResult = await GetAvailabilityAsync(availabilityFilter, cancellationToken).ConfigureAwait(false);
        
        Console.WriteLine(availabilityResult);
    }

    private async Task<int> GetAvailabilityAsync(AvailabilityFilterDto filter, CancellationToken cancellationToken)
    {
        // Get the total number of available rooms at hotel level for the specified hotel and room type
        var hotelTotalRooms = await _hotelService
            .GetNumberOfAvailableHotelRoomAsync(filter.HotelId, filter.RoomTypeCode,
                cancellationToken).ConfigureAwait(false);

        // Get the number of bookings done for the specified hotel and room type in the given date range
        var bookingsDoneForRoom = await _bookingService
            .GetBookingsDoneInInterval(filter, cancellationToken)
            .ConfigureAwait(false);

        // Calculate the number of available rooms by subtracting the bookings from the total available rooms
        var availability = hotelTotalRooms - bookingsDoneForRoom;

        return availability;
    }
    
    private static AvailabilityFilterDto GetAvailabilityFilter(string[] args)
    {
        var filter = new AvailabilityFilterDto
        {
            HotelId = args[0],
            RoomTypeCode = args[2]
        };

        var dates = GeneralHelper.GetDatesFromRange(args[1]);
        filter.StartDate = dates.Item1;
        filter.EndDate = dates.Item2;

        return filter;
    } 
}