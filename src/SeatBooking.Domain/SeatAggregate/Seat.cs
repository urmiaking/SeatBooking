using SeatBooking.Core.Exceptions;
using SeatBooking.Core.Patterns.DDD;

namespace SeatBooking.Domain.SeatAggregate;

public readonly record struct SeatId(Guid Value);
public class Seat : EntityBase<SeatId>
{
    public int Number { get; private set; }
    public SeatStatus Status { get; private set; }
    public byte[] RowVersion { get; private set; }

#pragma warning disable CS8618
    private Seat() { }
#pragma warning restore CS8618

    public static Seat Create(int seatNumber)
    {
        return new Seat
        {
            Id = new SeatId(Guid.NewGuid()),
            Number = seatNumber,
            Status = SeatStatus.Available
        };
    }

    public void MarkAsPending()
    {
        if (Status != SeatStatus.Available)
            throw new DomainException("Seat is not available.");

        Status = SeatStatus.Pending;
    }

    public void Reserve()
    {
        if (Status != SeatStatus.Pending)
            throw new DomainException("Seat must be pending.");

        Status = SeatStatus.Reserved;
    }

    public void Release()
    {
        if (Status == SeatStatus.Available)
            return;

        Status = SeatStatus.Available;
    }
}