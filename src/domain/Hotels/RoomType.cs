using System.Text.Json.Serialization;

namespace MyApp.Domain.Hotels;

public class RoomType
{
    [JsonPropertyName("code")]
    public string? Code { get; set; }
    
    [JsonPropertyName("size")]
    public int Size { get; set; }
    
    [JsonPropertyName("description")]
    public string? Description { get; set; }
    
    [JsonPropertyName("amenities")]
    public List<string> Amenities { get; set; } = [];
    
    [JsonPropertyName("features")]
    public List<string> Features { get; set; } = new List<string>();
}