namespace MyApp.Domain.Filters;

public class AvailabilityFilterDto
{
    public string HotelId { get; set; } = string.Empty;
    public string RoomTypeCode { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}