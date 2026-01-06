using SeatBooking.Domain.SeatAggregate;

namespace SeatBooking.Application.DTOs.Seats;

public record GetSeatResponse(Guid Id, int Number, SeatStatus Status);