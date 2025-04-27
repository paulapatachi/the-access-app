using Microsoft.Extensions.Configuration;
using MyApp.Domain.Bookings;
using MyApp.Domain.Filters;
using MyApp.Infrastructure.Helpers;
using MyApp.Infrastructure.Services.Files;

namespace MyApp.Infrastructure.Services.Bookings;

public class BookingService : IBookingService
{
    private readonly IFileService _fileService;
    
    private readonly string _bookingsFilePath;
    private const string DefaultBookingsFileName = "bookings.json";

    public BookingService(IFileService fileService, IConfiguration configuration)
    {
        _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
        _bookingsFilePath = configuration["bookings"] ?? FileHelper.GetFullPath(DefaultBookingsFileName);
    }

    public async Task<int> GetBookingsDoneInInterval(AvailabilityFilterDto filter, CancellationToken cancellationToken)
    {
        // Get all bookings for the specified hotel and room type
        var bookingsDoneForRoomType = await GetFilteredBookingsAsync(filter.HotelId, filter.RoomTypeCode, cancellationToken).ConfigureAwait(false);
        var bookingsDoneList = bookingsDoneForRoomType as Booking[] ?? bookingsDoneForRoomType.ToArray();
        if(!bookingsDoneList.Any())
        {
            return 0;
        }

        // Count bookings made in the provided date range
        var totalBookings = bookingsDoneList.Count(x =>
            filter.StartDate < x.DepartureDateTime &&
            filter.EndDate > x.ArrivalDateTime
        );
        
        return totalBookings;
    }
    
    public async Task<IEnumerable<BookingResumeDto>> GetBookingsDoneForHotel(RoomTypeFilterDto filter, CancellationToken cancellationToken)
    {
        // Get all bookings for the specified hotel
        var bookings = (await GetAllAsync(cancellationToken).ConfigureAwait(false)).Where(x => x.HotelId == filter.HotelId);
        var availableBookings = bookings as Booking[] ?? bookings.ToArray();

        // Extract a resume of bookings done for each room type
        var bookingsResume = availableBookings.GroupBy(x => x.RoomType)
            .Select(x => new BookingResumeDto
            {
                RoomTypeCode = x.Key ?? string.Empty,
                NumberOfBookings = x.Count(y => filter.StartDate < y.DepartureDateTime &&
                                        filter.EndDate > y.ArrivalDateTime)
            });
        
        return bookingsResume;
    }
    
    private async Task<IEnumerable<Booking>> GetFilteredBookingsAsync(string hotelId, string roomTypeCode, CancellationToken cancellationToken)
    {
        var allBookings = await GetAllAsync(cancellationToken).ConfigureAwait(false);
        var filteredBookings = allBookings
            .Where(x => x.HotelId == hotelId && x.RoomType == roomTypeCode)
            .ToList();

        return filteredBookings;
    }

    private async Task<IEnumerable<Booking>> GetAllAsync(CancellationToken cancellationToken)
    {
        var bookings = await _fileService.ReadJsonFileAsync<IEnumerable<Booking>>(_bookingsFilePath, cancellationToken);
        return bookings ?? new List<Booking>();
    }
}