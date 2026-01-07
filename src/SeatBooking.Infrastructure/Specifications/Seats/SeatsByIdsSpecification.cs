using SeatBooking.Core.Patterns.Specifications;
using SeatBooking.Domain.SeatAggregate;

namespace SeatBooking.Infrastructure.Specifications.Seats;

public class SeatsByIdsSpecification : SpecificationBase<Seat>
{
    public SeatsByIdsSpecification(IEnumerable<SeatId> seatIds)
    {
        AddCriteria(x => seatIds.Contains(x.Id));
    }
}