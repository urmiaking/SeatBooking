using SeatBooking.Application.DTOs.Reservations;

namespace SeatBooking.Application.Abstractions;

public interface IReservationService
{
    Task<StartReservationResponse> StartReservationAsync(StartReservationRequest request, CancellationToken cancellationToken = default);
    Task ProcessPaymentAsync(Guid reservationId, bool paymentSuccess, CancellationToken cancellationToken = default);
}