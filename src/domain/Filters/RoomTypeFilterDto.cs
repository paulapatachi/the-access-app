namespace MyApp.Domain.Filters;

public class RoomTypeFilterDto
{
    public string HotelId { get; set; } = string.Empty;
    public int GuestsNumber { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}