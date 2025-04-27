using MyApp.Domain.Bookings;
using MyApp.Domain.Enums;
using MyApp.Domain.Filters;
using MyApp.Domain.Hotels;
using MyApp.Infrastructure.Helpers;
using MyApp.Infrastructure.Services.Bookings;
using MyApp.Infrastructure.Services.Hotels;

namespace MyApp.Infrastructure.Commands;

public class RoomTypesCommand : IManagementCommand
{
    public CommandType Command => CommandType.RoomTypes;
    private readonly IHotelService _hotelService;
    private readonly IBookingService _bookingService;

    public RoomTypesCommand(IHotelService hotelService, IBookingService bookingService)
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
        var roomTypeFilter = GetRoomTypeFilter(args);

        // Get allocation based on filter
        var allocatedRooms = await GetRoomTypesAllocationAsync(roomTypeFilter, cancellationToken)
            .ConfigureAwait(false);
        
        // Create response based on allocated rooms
        // Note: in the requirement says that the program should return a hotel name, but the example output displays a hotel id
        // I displayed the hotel id to match the example, but it can be easily changed to display the hotel name
        var response = allocatedRooms.Count == 0
            ? "Cannot allocate enough rooms for the requested number of guests."
            : $"{roomTypeFilter.HotelId}: {string.Join(", ", allocatedRooms)}";
        
        Console.WriteLine(response);
    }

    private async Task<List<string>> GetRoomTypesAllocationAsync(RoomTypeFilterDto filter, CancellationToken cancellationToken)
    {
        // Get hotel rooms and capacities
        var hotelRooms = await _hotelService.GetRoomsPerHotel(filter.HotelId, cancellationToken)
            .ConfigureAwait(false);

        // Get bookings information for the hotel based on provided date range
        var bookings = await _bookingService.GetBookingsDoneForHotel(filter, cancellationToken).ConfigureAwait(false);
        var bookingsResume = bookings as BookingResumeDto[] ?? bookings.ToArray();

        // Calculate the number of free units for each room type
        foreach (var room in hotelRooms.RoomTypes.ToList())
        {
            var bookedUnits = bookingsResume?
                .FirstOrDefault(x => x.RoomTypeCode == room.Code)?
                .NumberOfBookings ?? 0;
            room.FreeUnits = room.Units - bookedUnits;
        }
        
        // Filter rooms with available units
        var availableRooms = hotelRooms.RoomTypes.Where(x => x.FreeUnits > 0);
        
        // Get the allocation of rooms based on the number of guests
        var allocatedRooms = AllocateRoomsForGuests(availableRooms, filter.GuestsNumber);

        return allocatedRooms;
    }
    
    private static List<string> AllocateRoomsForGuests(IEnumerable<RoomTypeDto> availableRoomTypes, int guestsNumber)
    {
        // Sort room types descending by size to use larger rooms first
        var sortedRoomTypes = availableRoomTypes
            .OrderByDescending(x => x.Size).ToList();

        // Create the allocation result
        var allocation = new List<string>();

        foreach (var roomType in sortedRoomTypes)
        {
            // Determine how many rooms of this type are needed to fit the guests (floor division)
            var roomsNeededToFitGuests = guestsNumber / roomType.Size;
            // If there are not enough rooms available, use all available free rooms
            var roomsToUse = Math.Min(roomsNeededToFitGuests, roomType.FreeUnits);

            // Save room to allocation and determine how many guests are left to be placed
            for (var i = 0; i < roomsToUse; i++)
            {
                allocation.Add(roomType.Code);
                guestsNumber -= roomType.Size;
            }
            
            // Update the number of free units for the room type
            roomType.FreeUnits -= roomsToUse;

            // If all guests have been placed, exit the loop
            if (guestsNumber == 0)
                break;
        }

        // If people still need to be placed, try to find one room that can fit them partially
        if (guestsNumber > 0)
        {
            // Find the smallest room that can fit the remaining guests
            var partialRoom = sortedRoomTypes.LastOrDefault(x => x.Size >= guestsNumber && x.FreeUnits > 0);
            if (partialRoom != null)
            {
                allocation.Add(partialRoom.Code + "!");
                guestsNumber = 0;
            }
        }

        // Cancel the allocation made if there are still guests left without rooms
        if (guestsNumber > 0)
        {
            allocation.Clear();
        }
        
        return allocation;
    }
    
    private static RoomTypeFilterDto GetRoomTypeFilter(string[] args)
    {
        var filter = new RoomTypeFilterDto
        {
            HotelId = args[0]
        };

        var dates = GeneralHelper.GetDatesFromRange(args[1]);
        filter.StartDate = dates.Item1;
        filter.EndDate = dates.Item2;
        
        filter.GuestsNumber = int.TryParse(args[2], out var guestsNumber) ? guestsNumber : 0;

        return filter;
    } 
}