using MyApp.Domain.Bookings;
using MyApp.Domain.Filters;

namespace MyApp.Infrastructure.Services.Bookings;

public interface IBookingService
{
    Task<int> GetBookingsDoneInInterval(AvailabilityFilterDto filter, CancellationToken cancellationToken);

    Task<IEnumerable<BookingResumeDto>> GetBookingsDoneForHotel(RoomTypeFilterDto filter, CancellationToken cancellationToken);
}