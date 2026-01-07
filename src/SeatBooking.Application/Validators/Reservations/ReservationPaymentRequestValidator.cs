using FluentValidation;
using SeatBooking.Application.DTOs.Payments;
using SeatBooking.Core.DependencyInjections;

namespace SeatBooking.Application.Validators.Reservations;

[ScopedService]
internal sealed class ReservationPaymentRequestValidator : AbstractValidator<ReservationPaymentRequest>
{
    public ReservationPaymentRequestValidator()
    {
        RuleFor(x => x.ClientId)
            .NotEmpty().WithMessage("ClientId must not be empty.");

        RuleFor(x => x.ReservationId)
            .NotEmpty().WithMessage("ReservationId must not be empty.");
    }
}