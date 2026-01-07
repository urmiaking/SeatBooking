using SeatBooking.Application.DTOs.Seats;

namespace SeatBooking.API.Hubs;

public interface ISeatBookingClient
{
    Task SeatUpdated(GetSeatResponse seat);
    Task SeatsUpdated(IReadOnlyList<GetSeatResponse> seat);
}