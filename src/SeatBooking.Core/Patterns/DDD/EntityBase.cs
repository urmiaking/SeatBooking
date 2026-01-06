namespace SeatBooking.Core.Patterns.DDD;

public abstract class EntityBase;

public abstract class EntityBase<TId> : EntityBase
    where TId : notnull
{
    public TId Id { get; protected set; }

    protected EntityBase(TId id)
    {
        Id = id;
    }

#pragma warning disable CS8618 
    protected EntityBase() { }
#pragma warning restore CS8618 
}