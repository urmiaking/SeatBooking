using SeatBooking.Core.Patterns.Specifications;
using SeatBooking.Domain.ReservationAggregate;

namespace SeatBooking.Infrastructure.Specifications.Reservations;

public sealed class ExpiredReservationsSpecification : SpecificationBase<Reservation>
{
    public ExpiredReservationsSpecification(DateTime utcNow, TimeSpan expirationWindow)
    {
        var cutoffUtc = utcNow - expirationWindow;

        AddCriteria(x => x.Status == ReservationStatus.Pending);
        AddCriteria(x => x.CreatedAtUtc <= cutoffUtc);
    }
}