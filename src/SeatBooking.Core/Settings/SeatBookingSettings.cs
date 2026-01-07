namespace SeatBooking.Core.Settings;

public class SeatBookingSettings
{
    public int MaxSeats { get; set; } = 10;
    public int BookingTimeoutMinutes { get; set; } = 5;
}