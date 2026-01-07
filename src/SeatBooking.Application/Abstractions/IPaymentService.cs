using SeatBooking.Application.DTOs.Payments;
using SeatBooking.Domain.ReservationAggregate;

namespace SeatBooking.Application.Abstractions;

public interface IPaymentService
{
    Task<PaymentStatus> PayAsync(ReservationId reservationId, PaymentOutcome paymentOutcome, CancellationToken cancellationToken = default);
}