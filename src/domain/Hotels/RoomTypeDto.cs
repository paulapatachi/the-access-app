namespace MyApp.Domain.Hotels;

public class RoomTypeDto
{
    public string Code { get; set; } = string.Empty;
    public int Size { get; set; }
    public int Units { get; set; }
    public int FreeUnits { get; set; }
}