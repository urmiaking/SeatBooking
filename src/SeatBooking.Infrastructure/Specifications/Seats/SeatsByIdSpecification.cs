using SeatBooking.Core.Patterns.Specifications;
using SeatBooking.Domain.SeatAggregate;

namespace SeatBooking.Infrastructure.Specifications.Seats;

public class SeatsByIdSpecification(SeatId id) : SpecificationBase<Seat>(x => x.Id == id);