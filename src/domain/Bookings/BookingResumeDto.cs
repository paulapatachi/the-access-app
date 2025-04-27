namespace MyApp.Domain.Bookings;

public class BookingResumeDto
{
    public string RoomTypeCode { get; set; } = string.Empty;
    public int NumberOfBookings { get; set; }
}