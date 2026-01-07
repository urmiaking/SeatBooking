using SeatBooking.Application.DTOs.Seats;

namespace SeatBooking.Application.Abstractions;

public interface ISeatBookingNotifier
{
    Task SeatsUpdatedAsync(IReadOnlyList<GetSeatResponse> seats, CancellationToken cancellationToken = default);
    Task SeatUpdatedAsync(GetSeatResponse seat, CancellationToken cancellationToken = default);
}