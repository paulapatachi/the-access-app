using Microsoft.Extensions.Configuration;
using Moq;
using MyApp.Domain.Bookings;
using MyApp.Domain.Filters;
using MyApp.Infrastructure.Services.Bookings;
using MyApp.Infrastructure.Services.Files;

namespace MyApp.Tests.Services;

public class BookingServiceUnitTests
{
    private readonly Mock<IFileService> _fileServiceMock;
    private readonly BookingService _bookingService;

    public BookingServiceUnitTests()
    {
        _fileServiceMock = new Mock<IFileService>();
        var mockConfiguration = new Mock<IConfiguration>();
        _bookingService = new BookingService(_fileServiceMock.Object, mockConfiguration.Object);
    }

    [Fact]
    public async Task GetBookingsDoneInIntervalAsync_ShouldReturnCorrectCount_WhenBookingsMatch()
    {
        // Arrange
        var filter = new AvailabilityFilterDto
        {
            HotelId = "H1",
            RoomTypeCode = "SGL",
            StartDate = new DateTime(2024, 9, 1),
            EndDate = new DateTime(2024, 9, 4)
        };

        var bookings = new List<Booking>
        {
            new() { HotelId = "H1", RoomType = "SGL", Arrival = "20240901", Departure = "20240902" },
            new() { HotelId = "H1", RoomType = "SGL", Arrival = "20240903", Departure = "20240905" },
            new() { HotelId = "H1", RoomType = "SGL", Arrival = "20240921", Departure = "20240922" },
            new() { HotelId = "H1", RoomType = "DBL", Arrival = "20240901", Departure = "20240902" }
        };

        _fileServiceMock
            .Setup(fs => fs.ReadJsonFileAsync<IEnumerable<Booking>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(bookings);

        // Act
        var result = await _bookingService.GetBookingsDoneInInterval(filter, CancellationToken.None);

        // Assert
        Assert.Equal(2, result);
    }

    [Fact]
    public async Task GetBookingsDoneInIntervalAsync_ShouldReturnZero_WhenNoBookingsMatch()
    {
        // Arrange
        var filter = new AvailabilityFilterDto
        {
            HotelId = "H1",
            RoomTypeCode = "SGL",
            StartDate = new DateTime(2024, 9, 1),
            EndDate = new DateTime(2024, 9, 2)
        };

        var bookings = new List<Booking>();

        _fileServiceMock
            .Setup(fs => fs.ReadJsonFileAsync<IEnumerable<Booking>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(bookings);

        // Act
        var result = await _bookingService.GetBookingsDoneInInterval(filter, CancellationToken.None);

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public async Task GetBookingsDoneForHotelAsync_ShouldReturnCorrectSummary_WhenBookingMatch()
    {
        // Arrange
        var filter = new RoomTypeFilterDto
        {
            HotelId = "H1",
            StartDate = new DateTime(2024, 9, 1),
            EndDate = new DateTime(2024, 9, 5)
        };

        var bookings = new List<Booking>
        {
            new() { HotelId = "H1", RoomType = "SGL", Arrival = "20240901", Departure = "20240903" },
            new() { HotelId = "H1", RoomType = "SGL", Arrival = "20240904", Departure = "20240905" },
            new() { HotelId = "H1", RoomType = "DBL", Arrival = "20240901", Departure = "20240902" },
            new() { HotelId = "H2", RoomType = "SGL", Arrival = "20240901", Departure = "20240902" }
        };

        _fileServiceMock
            .Setup(fs => fs.ReadJsonFileAsync<IEnumerable<Booking>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(bookings);

        // Act
        var result = (await _bookingService.GetBookingsDoneForHotel(filter, CancellationToken.None)).ToList();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(result, r => r.RoomTypeCode == "SGL" && r.NumberOfBookings == 2);
        Assert.Contains(result, r => r.RoomTypeCode == "DBL" && r.NumberOfBookings == 1);
    }

    [Fact]
    public async Task GetBookingsDoneForHotelAsync_ShouldReturnEmptyList_WhenNoBookingsMatch()
    {
        // Arrange
        var filter = new RoomTypeFilterDto
        {
            HotelId = "H1",
            StartDate = new DateTime(2024, 9, 1),
            EndDate = new DateTime(2024, 9, 2)
        };

        var bookings = new List<Booking>
        {
            new() { HotelId = "H2", RoomType = "DBL", Arrival = "20240901", Departure = "20240902" }
        };

        _fileServiceMock
            .Setup(fs => fs.ReadJsonFileAsync<IEnumerable<Booking>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(bookings);

        // Act
        var result = await _bookingService.GetBookingsDoneForHotel(filter, CancellationToken.None);

        // Assert
        Assert.Empty(result);
    }
}
