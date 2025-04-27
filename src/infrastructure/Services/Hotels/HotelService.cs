using Microsoft.Extensions.Configuration;
using MyApp.Domain.Hotels;
using MyApp.Infrastructure.Helpers;
using MyApp.Infrastructure.Services.Files;

namespace MyApp.Infrastructure.Services.Hotels;

public class HotelService : IHotelService
{
    private readonly IFileService _fileService;
    
    private readonly string _hotelsFilePath;
    private const string DefaultHotelsFileName = "hotels.json";

    public HotelService(IFileService fileService, IConfiguration configuration)
    {
        _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
        _hotelsFilePath = configuration["hotels"] ?? FileHelper.GetFullPath(DefaultHotelsFileName);
    }
    
    public async Task<int> GetNumberOfAvailableHotelRoomAsync(string hotelId, string roomTypeCode, CancellationToken cancellationToken)
    {
        // Get the hotel by ID
        var hotel = await GetOneAsync(hotelId, cancellationToken).ConfigureAwait(false);
        if (hotel == null)
        {
            Console.WriteLine("Hotel not found.");
            return 0;
        }

        // Count the number of rooms of the specified type
        var totalRooms = hotel.Rooms.Count(x => x.RoomType == roomTypeCode);
        if (totalRooms == 0) Console.WriteLine("No rooms of specified type.");

        return totalRooms;
    }
    
    public async Task<HotelDto> GetRoomsPerHotel(string hotelId, CancellationToken cancellationToken)
    {
        // Get the hotel by ID
        var hotel = await GetOneAsync(hotelId, cancellationToken).ConfigureAwait(false);
        if (hotel == null)
        {
            Console.WriteLine("The hotel does not exists.");
            return new HotelDto();
        }
        
        // Group rooms by room type and count the available units
        var roomTypeUnits = hotel.Rooms
            .GroupBy(x => x.RoomType)
            .ToDictionary(x => x.Key ?? string.Empty, y => y.Count());
        
        // Extract room type information
        var roomTypeData = hotel.RoomTypes
            .Select(x => new RoomTypeDto
            {
                Code = x.Code ?? string.Empty,
                Size = x.Size,
                Units = roomTypeUnits.GetValueOrDefault(x.Code ?? string.Empty)
            }).ToList();
        
        return new HotelDto
        {
            Name = hotel.Name,
            RoomTypes = roomTypeData
        };
    }
    
    private async Task<Hotel?> GetOneAsync(string hotelId, CancellationToken cancellationToken)
    {
        var allHotels = await GetAllAsync(cancellationToken).ConfigureAwait(false);
        return allHotels.FirstOrDefault(h => h.Id == hotelId);
    }

    private async Task<IEnumerable<Hotel>> GetAllAsync(CancellationToken cancellationToken)
    {
        var hotels = await _fileService.ReadJsonFileAsync<IEnumerable<Hotel>>(_hotelsFilePath, cancellationToken);
        return hotels ?? new List<Hotel>();
    }
}