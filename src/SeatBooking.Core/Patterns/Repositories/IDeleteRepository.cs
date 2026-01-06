using SeatBooking.Core.Patterns.DDD;

namespace SeatBooking.Core.Patterns.Repositories;

public interface IDeleteRepository<TEntity> where TEntity : EntityBase
{
    void Delete(TEntity entity);
    Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);
    void DeleteRange(IEnumerable<TEntity> entities);
    Task DeleteRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
}
