using MyApp.Domain.Hotels;

namespace MyApp.Infrastructure.Services.Hotels;

public interface IHotelService
{
    Task<int> GetNumberOfAvailableHotelRoomAsync(string hotelId, string roomTypeCode,
        CancellationToken cancellationToken);
    Task<HotelDto> GetRoomsPerHotel(string hotelId, CancellationToken cancellationToken);
}