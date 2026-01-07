using SeatBooking.Application.Abstractions;
using SeatBooking.Application.DTOs.Payments;
using SeatBooking.Core.DependencyInjections;
using SeatBooking.Domain.ReservationAggregate;

namespace SeatBooking.Application.Services;

[ScopedService]
internal sealed class PaymentService : IPaymentService
{
    public async Task<PaymentStatus> PayAsync(ReservationId reservationId, PaymentOutcome paymentOutcome,
        CancellationToken cancellationToken = default)
    {
        await Task.Delay(2000, cancellationToken);

        return paymentOutcome switch
        {
            PaymentOutcome.Success => PaymentStatus.Succeeded,
            PaymentOutcome.Failed => PaymentStatus.Failed,
            PaymentOutcome.Random => Random.Shared.NextDouble() <= 0.7
                ? PaymentStatus.Succeeded
                : PaymentStatus.Failed,
            _ => throw new ArgumentOutOfRangeException(nameof(paymentOutcome), paymentOutcome, null)
        };
    }
}