using System.Text.Json.Serialization;

namespace MyApp.Domain.Hotels;

public class Hotel
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }
    
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    
    [JsonPropertyName("roomTypes")]
    public List<RoomType> RoomTypes { get; set; } = [];
    
    [JsonPropertyName("rooms")]
    public List<Room> Rooms { get; set; } = [];
}