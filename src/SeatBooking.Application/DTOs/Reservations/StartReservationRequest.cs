namespace SeatBooking.Application.DTOs.Reservations;

public record StartReservationRequest(Guid SeatId, Guid ClientId);