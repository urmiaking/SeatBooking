using SeatBooking.Core.Patterns.Specifications;
using SeatBooking.Domain.ReservationAggregate;

namespace SeatBooking.Infrastructure.Specifications.Reservations;

public class ReservationsByIdSpecification(ReservationId id) : SpecificationBase<Reservation>(x => x.Id == id);