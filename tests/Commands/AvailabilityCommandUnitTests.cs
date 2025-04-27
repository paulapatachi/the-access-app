using Moq;
using MyApp.Domain.Filters;
using MyApp.Infrastructure.Commands;
using MyApp.Infrastructure.Services.Bookings;
using MyApp.Infrastructure.Services.Hotels;

namespace MyApp.Tests.Commands;

public class AvailabilityCommandUnitTests
{
    private readonly Mock<IHotelService> _mockHotelService;
    private readonly Mock<IBookingService> _mockBookingService;
    private readonly AvailabilityCommand _availabilityCommand;

    public AvailabilityCommandUnitTests()
    {
        _mockHotelService = new Mock<IHotelService>();
        _mockBookingService = new Mock<IBookingService>();

        _availabilityCommand = new AvailabilityCommand(_mockHotelService.Object, _mockBookingService.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrowArgumentException_WhenArgumentsAreNull()
    {
        // Arrange
        string[] args = [];

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _availabilityCommand.ExecuteAsync(args, CancellationToken.None));
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrowArgumentException_WhenInsufficientArguments()
    {
        // Arrange
        string[] args = ["H1", "20240901-20240902"]; // Missing room type

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _availabilityCommand.ExecuteAsync(args, CancellationToken.None));
    }

    [Fact]
    public async Task ExecuteAsync_ShouldOutputCorrectAvailability_WhenValidArguments()
    {
        // Arrange
        string[] args = ["H1", "20240901-20240902", "SGL"];
        var expectedAvailability = 5;

        _mockHotelService.Setup(s => s.GetNumberOfAvailableHotelRoomAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(10); // Total rooms available
        _mockBookingService.Setup(s => s.GetBookingsDoneInInterval(It.IsAny<AvailabilityFilterDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(5); // Booked rooms

        // Set up the StringWriter to capture Console output
        var stringWriter = new StringWriter();
        Console.SetOut(stringWriter);

        // Act
        await _availabilityCommand.ExecuteAsync(args, CancellationToken.None);

        // Assert
        var output = stringWriter.ToString();
        Assert.Contains(expectedAvailability.ToString(), output);

        // Clean up: Reset Console output
        Console.SetOut(new StreamWriter(Console.OpenStandardOutput()));
    }

    [Fact]
    public async Task ExecuteAsync_ShouldOutputZero_WhenNoRoomsAvailable()
    {
        // Arrange
        string[] args = ["H1", "20240901-20240902", "SGL"];
        var expectedAvailability = 0;

        _mockHotelService.Setup(s => s.GetNumberOfAvailableHotelRoomAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(5); // Total rooms available
        _mockBookingService.Setup(s => s.GetBookingsDoneInInterval(It.IsAny<AvailabilityFilterDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(5); // All rooms booked

        // Set up the StringWriter to capture Console output
        var stringWriter = new StringWriter();
        Console.SetOut(stringWriter);

        // Act
        await _availabilityCommand.ExecuteAsync(args, CancellationToken.None);

        // Assert
        var output = stringWriter.ToString();
        Assert.Contains(expectedAvailability.ToString(), output);

        // Clean up: Reset Console output
        Console.SetOut(new StreamWriter(Console.OpenStandardOutput()));
    }
    
    [Fact]
    public async Task ExecuteAsync_ShouldOutputNegativeAvailability_WhenOverbooking()
    {
        // Arrange
        string[] args = ["H1", "20240901-20240902", "SGL"];
        var expectedAvailability = -1;

        _mockHotelService.Setup(s => s.GetNumberOfAvailableHotelRoomAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(5); // Total rooms available
        _mockBookingService.Setup(s => s.GetBookingsDoneInInterval(It.IsAny<AvailabilityFilterDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(6); // All rooms booked

        // Set up the StringWriter to capture Console output
        var stringWriter = new StringWriter();
        Console.SetOut(stringWriter);

        // Act
        await _availabilityCommand.ExecuteAsync(args, CancellationToken.None);

        // Assert
        var output = stringWriter.ToString();
        Assert.Contains(expectedAvailability.ToString(), output);

        // Clean up: Reset Console output
        Console.SetOut(new StreamWriter(Console.OpenStandardOutput()));
    }
}
