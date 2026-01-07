using FluentValidation;
using SeatBooking.Application.DTOs.Reservations;
using SeatBooking.Core.DependencyInjections;

namespace SeatBooking.Application.Validators.Reservations;

[ScopedService]
internal class StartReservationRequestValidator : AbstractValidator<StartReservationRequest>
{
    public StartReservationRequestValidator()
    {
        RuleFor(x => x.ClientId)
            .NotEmpty().WithMessage("ClientId must not be empty.");

        RuleFor(x => x.SeatId)
            .NotEmpty().WithMessage("SeatId must not be empty.");
    }
}