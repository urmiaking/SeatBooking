using SeatBooking.Core.Exceptions;
using SeatBooking.Domain.Common;
using SeatBooking.Domain.SeatAggregate;

namespace SeatBooking.Domain.ReservationAggregate;

public readonly record struct ReservationId(Guid Value);
public readonly record struct ClientId(Guid Value);
public sealed class Reservation : EntityBase<ReservationId>
{
    public SeatId SeatId { get; private init; }
    public ClientId ClientId { get; private init; }

    public ReservationStatus Status { get; private set; }
    public DateTime CreatedAtUtc { get; private init; }

    private Reservation() { }

    private Reservation(
        ReservationId id,
        SeatId seatId,
        ClientId clientId,
        DateTime createdAtUtc)
    {
        Id = id;
        SeatId = seatId;
        ClientId = clientId;
        CreatedAtUtc = createdAtUtc;
        Status = ReservationStatus.Pending;
    }

    public static Reservation Start(
        SeatId seatId,
        ClientId clientId,
        DateTime createdAtUtc)
    {
        return new Reservation(
            new ReservationId(Guid.NewGuid()),
            seatId,
            clientId,
            createdAtUtc);
    }

    public void CompletePayment(bool success)
    {
        EnsurePending();

        Status = success
            ? ReservationStatus.Completed
            : ReservationStatus.Failed;
    }

    public void Expire(DateTime utcNow, TimeSpan expirationWindow)
    {
        EnsurePending();

        if (utcNow - CreatedAtUtc < expirationWindow)
            return;

        Status = ReservationStatus.Expired;
    }

    private void EnsurePending()
    {
        if (Status != ReservationStatus.Pending)
            throw new DomainException(
                $"Reservation {Id.Value} is in invalid state {Status}");
    }
}