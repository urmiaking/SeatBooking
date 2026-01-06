using SeatBooking.Core.Patterns.Specifications;
using SeatBooking.Domain.ReservationAggregate;

namespace SeatBooking.Infrastructure.Specifications.Reservations;

public class ReservationsByClientIdSpecification : SpecificationBase<Reservation>
{
    public ReservationsByClientIdSpecification(ClientId clientId)
    {
        AddCriteria(x => x.ClientId == clientId);
    }
}