using Microsoft.Extensions.Configuration;
using Moq;
using MyApp.Domain.Hotels;
using MyApp.Infrastructure.Services.Files;
using MyApp.Infrastructure.Services.Hotels;

namespace MyApp.Tests.Services;

public class HotelServiceUnitTests
{
    private readonly Mock<IFileService> _fileServiceMock;
    private readonly HotelService _hotelService;

    public HotelServiceUnitTests()
    {
        _fileServiceMock = new Mock<IFileService>();
        var mockConfiguration = new Mock<IConfiguration>();
        _hotelService = new HotelService(_fileServiceMock.Object, mockConfiguration.Object);
    }

    [Fact]
    public async Task GetNumberOfAvailableHotelRoomAsync_ShouldReturnCorrectRoomCount_WhenHotelAndRoomTypeExist()
    {
        // Arrange
        var hotelId = "H1";
        var roomTypeCode = "DBL";
        var hotels = new List<Hotel>
        {
            new()
            {
                Id = hotelId,
                Name = "Hotel Test",
                RoomTypes = new List<RoomType>(),
                Rooms = new List<Room>
                {
                    new Room { RoomId = "101", RoomType = "SGL" },
                    new Room { RoomId = "201", RoomType = "DBL" },
                    new Room { RoomId = "202", RoomType = "DBL" }
                }
            }
        };

        _fileServiceMock
            .Setup(fs => fs.ReadJsonFileAsync<IEnumerable<Hotel>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(hotels);

        // Act
        var count = await _hotelService.GetNumberOfAvailableHotelRoomAsync(hotelId, roomTypeCode, CancellationToken.None);

        // Assert
        Assert.Equal(2, count);
    }

    [Fact]
    public async Task GetNumberOfAvailableHotelRoomAsync_ShouldReturnZero_WhenHotelNotFound()
    {
        // Arrange
        _fileServiceMock
            .Setup(fs => fs.ReadJsonFileAsync<IEnumerable<Hotel>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Hotel>());

        // Act
        var count = await _hotelService.GetNumberOfAvailableHotelRoomAsync("invalid", "DBL", CancellationToken.None);

        // Assert
        Assert.Equal(0, count);
    }

    [Fact]
    public async Task GetRoomsPerHotel_ShouldReturnCorrectRoomTypeDtoList_WhenHotelExists()
    {
        // Arrange
        var hotelId = "H1";
        var hotels = new List<Hotel>
        {
            new()
            {
                Id = hotelId,
                Name = "Hotel Test",
                RoomTypes = new List<RoomType>
                {
                    new() { Code = "SGL", Size = 1 },
                    new() { Code = "DBL", Size = 2 }
                },
                Rooms = new List<Room>
                {
                    new() { RoomType = "SGL", RoomId = "101" },
                    new() { RoomType = "SGL", RoomId = "102" },
                    new() { RoomType = "DBL", RoomId = "201" }
                }
            }
        };

        _fileServiceMock
            .Setup(fs => fs.ReadJsonFileAsync<IEnumerable<Hotel>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(hotels);

        // Act
        var result = await _hotelService.GetRoomsPerHotel(hotelId, CancellationToken.None);

        // Assert
        Assert.Equal("Hotel Test", result.Name);
        Assert.Collection(result.RoomTypes,
            rt =>
            {
                Assert.Equal("SGL", rt.Code);
                Assert.Equal(1, rt.Size);
                Assert.Equal(2, rt.Units);
            },
            rt =>
            {
                Assert.Equal("DBL", rt.Code);
                Assert.Equal(2, rt.Size);
                Assert.Equal(1, rt.Units);
            });
    }

    [Fact]
    public async Task GetRoomsPerHotel_ShouldReturnEmptyDto_WhenHotelNotFound()
    {
        // Arrange
        _fileServiceMock
            .Setup(fs => fs.ReadJsonFileAsync<IEnumerable<Hotel>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Hotel>());

        // Act
        var result = await _hotelService.GetRoomsPerHotel("nonexistent", CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.Name);
        Assert.Empty(result.RoomTypes);
    }
}
