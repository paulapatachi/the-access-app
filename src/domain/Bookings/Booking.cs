using System.Text.Json.Serialization;
using MyApp.Domain.Helpers;

namespace MyApp.Domain.Bookings;

public class Booking
{
    [JsonPropertyName("hotelId")]
    public string? HotelId { get; set; }
    
    [JsonPropertyName("arrival")]
    public string? Arrival { get; set; }
    public DateTime ArrivalDateTime => Arrival.GetDate();

    [JsonPropertyName("departure")]
    public string? Departure { get; set; }
    public DateTime DepartureDateTime => Departure.GetDate();
    
    [JsonPropertyName("roomType")]
    public string? RoomType { get; set; }
    
    [JsonPropertyName("roomRate")]
    public string? RoomRate { get; set; }
}