using SeatBooking.Core.Patterns.Specifications;
using SeatBooking.Domain.ReservationAggregate;
using SeatBooking.Domain.SeatAggregate;

namespace SeatBooking.Infrastructure.Specifications.Reservations;

public class ReservationsBySeatSpecification : SpecificationBase<Reservation>
{
    public ReservationsBySeatSpecification(SeatId seatId, ReservationStatus? status = null)
    {
        AddCriteria(x => x.SeatId == seatId);

        if (status.HasValue)
        {
            AddCriteria(x => x.Status == status.Value);
        }
    }
}