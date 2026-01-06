using SeatBooking.Core.Patterns.DDD;

namespace SeatBooking.Core.Patterns.Repositories;

public interface IUpdateRepository<TEntity> where TEntity : EntityBase
{
    void Update(TEntity entity);
    Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
    void UpdateRange(IEnumerable<TEntity> entities);
    Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
}