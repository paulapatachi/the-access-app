namespace MyApp.Domain.Hotels;

public class HotelDto
{
    public string? Name { get; set; }
    public List<RoomTypeDto> RoomTypes { get; set; } = [];
}