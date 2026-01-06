using SeatBooking.Domain.ReservationAggregate;
using SeatBooking.Domain.SeatAggregate;

namespace SeatBooking.Application.DTOs.Reservations;

public record StartReservationResponse(
    Guid ReservationId,
    Guid SeatId,
    Guid ClientId,
    DateTime CreatedAtUtc,
    ReservationStatus ReservationStatus,
    SeatStatus SeatStatus);