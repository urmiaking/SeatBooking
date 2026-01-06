using Microsoft.EntityFrameworkCore.Storage;
using SeatBooking.Core.Patterns.DDD;
using SeatBooking.Core.Patterns.Specifications;
using System.Data;
using System.Data.Common;

namespace SeatBooking.Core.Patterns.Repositories;

public interface IRepository<TEntity> where TEntity : EntityBase
{
    void AsNoTracking();

    void AsTracking();

    Task<IDbContextTransaction> BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, CancellationToken cancellationToken = default);

    IDbContextTransaction? GetCurrentTransaction();

    Task<int> SaveAsync(CancellationToken cancellationToken = default);

    Task<IDbContextTransaction?> UseTransactionAsync(DbTransaction transaction, CancellationToken cancellationToken = default);

    IQueryable<TEntity> Get(ISpecification<TEntity> specification);

    Task<int> CountAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);
}