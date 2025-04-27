using System.Text.Json.Serialization;

namespace MyApp.Domain.Hotels;

public class Room
{
    [JsonPropertyName("roomType")]
    public string? RoomType { get; set; }
    
    [JsonPropertyName("roomId")]
    public string? RoomId { get; set; }
}