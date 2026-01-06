namespace SeatBooking.Core.Settings;

public class SeatBookingSettings
{
    public int MaxSeats { get; set; } = 10;
    public TimeSpan BookingTimeoutMinutes { get; set; } = TimeSpan.FromMinutes(5);
}