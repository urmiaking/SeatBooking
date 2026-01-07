using ErrorOr;
using SeatBooking.Application.DTOs.Payments;
using SeatBooking.Application.DTOs.Reservations;

namespace SeatBooking.Application.Abstractions;

public interface IReservationService
{
    Task<ErrorOr<StartReservationResponse>> StartReservationAsync(StartReservationRequest request, CancellationToken cancellationToken = default);
    Task<ErrorOr<ReservationPaymentResponse>> ProcessPaymentAsync(ReservationPaymentRequest request, CancellationToken cancellationToken = default);
    Task ExpirePendingReservationsAsync(CancellationToken cancellationToken = default);
}