using SeatBooking.Core.Exceptions;
using SeatBooking.Domain.Common;

namespace SeatBooking.Domain.SeatAggregate;

public readonly record struct SeatId(Guid Value);
public class Seat : EntityBase<SeatId>
{
    public SeatStatus Status { get; private set; }
    public byte[] RowVersion { get; set; }

#pragma warning disable CS8618
    private Seat() { }
#pragma warning restore CS8618

    public static Seat Create()
    {
        return new Seat
        {
            Id = new SeatId(Guid.NewGuid()),
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