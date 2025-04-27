using Moq;
using MyApp.Domain.Bookings;
using MyApp.Domain.Filters;
using MyApp.Domain.Hotels;
using MyApp.Infrastructure.Commands;
using MyApp.Infrastructure.Services.Bookings;
using MyApp.Infrastructure.Services.Hotels;

namespace MyApp.Tests.Commands;

public class RoomTypesCommandUnitTests
{
    private readonly Mock<IHotelService> _mockHotelService;
    private readonly Mock<IBookingService> _mockBookingService;
    private readonly RoomTypesCommand _roomTypesCommand;

    public RoomTypesCommandUnitTests()
    {
        _mockHotelService = new Mock<IHotelService>();
        _mockBookingService = new Mock<IBookingService>();

        _roomTypesCommand = new RoomTypesCommand(_mockHotelService.Object, _mockBookingService.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrowArgumentException_WhenArgumentsAreNull()
    {
        // Arrange
        string[] args = [];

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _roomTypesCommand.ExecuteAsync(args, CancellationToken.None));
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrowArgumentException_WhenInsufficientArguments()
    {
        // Arrange
        string[] args = ["H1", "20240901-20240902"]; // Missing guests number

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _roomTypesCommand.ExecuteAsync(args, CancellationToken.None));
    }

    [Fact]
    public async Task ExecuteAsync_ShouldOutputAllocation_WhenSufficientRoomsAvailable()
    {
        // Arrange
        string[] args = ["H1", "20240901-20240902", "6"];
        var hotelRooms = new HotelDto
        {
            RoomTypes = new List<RoomTypeDto>
            {
                new() { Code = "SGL", Size = 1, Units = 5, FreeUnits = 5 },
                new() { Code = "DBL", Size = 2, Units = 5, FreeUnits = 5 },
                new() { Code = "TPL", Size = 3, Units = 5, FreeUnits = 5 }
            }
        };
        var bookingsResume = new List<BookingResumeDto>
        {
            new() { RoomTypeCode = "SGL", NumberOfBookings = 1 },
            new() { RoomTypeCode = "DBL", NumberOfBookings = 2 },
            new() { RoomTypeCode = "TPL", NumberOfBookings = 1 }
        };

        _mockHotelService.Setup(s => s.GetRoomsPerHotel(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(hotelRooms);
        _mockBookingService.Setup(s => s.GetBookingsDoneForHotel(It.IsAny<RoomTypeFilterDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(bookingsResume);

        // Set up the StringWriter to capture Console output
        var stringWriter = new StringWriter();
        Console.SetOut(stringWriter);

        // Act
        await _roomTypesCommand.ExecuteAsync(args, CancellationToken.None);

        // Assert
        var output = stringWriter.ToString();
        Assert.Contains("H1:", output);
        Assert.Contains("TPL, TPL", output);

        // Clean up: Reset Console output
        Console.SetOut(new StreamWriter(Console.OpenStandardOutput()));
    }
    
    [Fact]
    public async Task ExecuteAsync_ShouldOutputPartialAllocation_WhenSufficientRoomsAvailable()
    {
        // Arrange
        string[] args = ["H1", "20240901-20240902", "3"];
        var hotelRooms = new HotelDto
        {
            RoomTypes = new List<RoomTypeDto>
            {
                new() { Code = "SGL", Size = 1, Units = 2, FreeUnits = 0 },
                new() { Code = "DBL", Size = 2, Units = 5, FreeUnits = 3 },
            }
        };
        var bookingsResume = new List<BookingResumeDto>
        {
            new() { RoomTypeCode = "SGL", NumberOfBookings = 2 },
            new() { RoomTypeCode = "DBL", NumberOfBookings = 2 }
        };

        _mockHotelService.Setup(s => s.GetRoomsPerHotel(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(hotelRooms);
        _mockBookingService.Setup(s => s.GetBookingsDoneForHotel(It.IsAny<RoomTypeFilterDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(bookingsResume);

        // Set up the StringWriter to capture Console output
        var stringWriter = new StringWriter();
        Console.SetOut(stringWriter);

        // Act
        await _roomTypesCommand.ExecuteAsync(args, CancellationToken.None);

        // Assert
        var output = stringWriter.ToString();
        Assert.Contains("H1:", output);
        Assert.Contains("DBL, DBL!", output);

        // Clean up: Reset Console output
        Console.SetOut(new StreamWriter(Console.OpenStandardOutput()));
    }

    [Fact]
    public async Task ExecuteAsync_ShouldOutputError_WhenCannotAllocateEnoughRooms()
    {
        // Arrange
        string[] args = ["H1", "20240901-20240902", "100"];
        var hotelRooms = new HotelDto
        {
            RoomTypes = new List<RoomTypeDto>
            {
                new() { Code = "SGL", Size = 1, Units = 5, FreeUnits = 5 },
                new() { Code = "DBL", Size = 2, Units = 5, FreeUnits = 5 }
            }
        };
        var bookingsResume = new List<BookingResumeDto>
        {
            new() { RoomTypeCode = "SGL", NumberOfBookings = 1 },
            new() { RoomTypeCode = "DBL", NumberOfBookings = 2 }
        };

        _mockHotelService.Setup(s => s.GetRoomsPerHotel(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(hotelRooms);
        _mockBookingService.Setup(s => s.GetBookingsDoneForHotel(It.IsAny<RoomTypeFilterDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(bookingsResume);

        // Set up the StringWriter to capture Console output
        var stringWriter = new StringWriter();
        Console.SetOut(stringWriter);

        // Act
        await _roomTypesCommand.ExecuteAsync(args, CancellationToken.None);

        // Assert
        var output = stringWriter.ToString();
        Assert.Contains("Cannot allocate enough rooms for the requested number of guests", output);

        // Clean up: Reset Console output
        Console.SetOut(new StreamWriter(Console.OpenStandardOutput()));
    }
}
