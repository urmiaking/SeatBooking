using SeatBooking.Application.DTOs.Seats;

namespace SeatBooking.Application.Abstractions;

public interface ISeatService
{
    Task<IReadOnlyList<GetSeatResponse>> GetSeatsAsync(CancellationToken cancellationToken = default);
}