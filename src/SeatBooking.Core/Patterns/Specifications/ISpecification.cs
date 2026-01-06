using SeatBooking.Core.Patterns.DDD;
using System.Linq.Expressions;

namespace SeatBooking.Core.Patterns.Specifications;

public interface ISpecification<TEntity> : ISpecification where TEntity : EntityBase
{
    Expression<Func<TEntity, bool>>? Criteria { get; }
    List<Expression<Func<TEntity, object>>> Includes { get; }
    Expression<Func<TEntity, object>>? OrderBy { get; }
    Expression<Func<TEntity, object>>? OrderByDescending { get; }
    List<Expression<Func<TEntity, object>>> ThenBy { get; }
    List<Expression<Func<TEntity, object>>> ThenByDescending { get; }
}

public interface ISpecification
{
    int? Take { get; }
    int? Skip { get; }
    bool IsPagingEnabled { get; }
}