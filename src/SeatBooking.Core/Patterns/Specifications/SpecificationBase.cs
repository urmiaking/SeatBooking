using SeatBooking.Core.Patterns.DDD;
using SeatBooking.Core.Patterns.Specifications.Helpers;
using System.Linq.Expressions;

namespace SeatBooking.Core.Patterns.Specifications;

public abstract class SpecificationBase<TEntity> : ISpecification<TEntity>
    where TEntity : EntityBase
{
    public Expression<Func<TEntity, bool>>? Criteria { get; private set; }
    public List<Expression<Func<TEntity, object>>> Includes { get; } = [];
    public Expression<Func<TEntity, object>>? OrderBy { get; private set; }
    public Expression<Func<TEntity, object>>? OrderByDescending { get; private set; }

    public List<Expression<Func<TEntity, object>>> ThenBy { get; } = [];
    public List<Expression<Func<TEntity, object>>> ThenByDescending { get; } = [];

    public int? Take { get; private set; }
    public int? Skip { get; private set; }
    public bool IsPagingEnabled { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SpecificationBase{TEntity}"/> class.
    /// </summary>
    /// <param name="initialCriteria">The initial criteria for the specification.</param>
    protected SpecificationBase(Expression<Func<TEntity, bool>>? initialCriteria = null)
    {
        Criteria = initialCriteria;
    }

    /// <summary>
    /// Adds an include expression to the specification.
    /// </summary>
    /// <param name="includeExpression">The expression representing the navigation property to include.</param>
    protected void AddInclude(Expression<Func<TEntity, object>> includeExpression)
    {
        Includes.Add(includeExpression);
    }

    /// <summary>
    /// Adds another criteria to the specification.
    /// If existing criteria are present, the new criteria will be combined using an AND operator.
    /// </summary>
    /// <param name="newCriteria">The new criteria to add.</param>
    protected void AddCriteria(Expression<Func<TEntity, bool>> newCriteria)
    {
        if (newCriteria == null) throw new ArgumentNullException(nameof(newCriteria));

        if (Criteria == null)
        {
            Criteria = newCriteria;
        }
        else
        {
            // Combine the existing criteria with the new one using 'AND'
            // We need to rewrite the expression to use the same parameter
            // for both expressions.

            var originalParameter = Criteria.Parameters[0];
            var newParameter = newCriteria.Parameters[0];

            // Create a visitor to replace the parameter in the newCriteria
            // with the parameter from the original Criteria.
            var visitor = new ParameterReplacer(newParameter, originalParameter);
            var rewrittenNewCriteriaBody = visitor.Visit(newCriteria.Body);

            if (rewrittenNewCriteriaBody == null)
            {
                // This should ideally not happen if newCriteria.Body is valid.
                // Handle error or throw a more specific exception.
                throw new InvalidOperationException("Failed to rewrite new criteria body.");
            }

            var andAlsoExpression = Expression.AndAlso(Criteria.Body, rewrittenNewCriteriaBody);
            Criteria = Expression.Lambda<Func<TEntity, bool>>(andAlsoExpression, originalParameter);
        }
    }


    /// <summary>
    /// Applies paging to the specification.
    /// </summary>
    /// <param name="skip">The number of items to skip.</param>
    /// <param name="take">The number of items to take.</param>
    protected void ApplyPaging(int skip, int take)
    {
        Skip = skip;
        Take = take;
        IsPagingEnabled = true;
    }

    /// <summary>
    /// Applies ordering by a specific property in ascending order.
    /// </summary>
    /// <param name="orderByExpression">The expression representing the property to order by.</param>
    protected void ApplyOrderBy(Expression<Func<TEntity, object>> orderByExpression)
    {
        // If no primary ordering yet, set it; otherwise, chain as ThenBy
        if (OrderBy == null && OrderByDescending == null)
        {
            OrderBy = orderByExpression;
        }
        else
        {
            ThenBy.Add(orderByExpression);
        }
    }

    /// <summary>
    /// Applies ordering by a specific property in descending order.
    /// </summary>
    /// <param name="orderByDescendingExpression">The expression representing the property to order by descending.</param>
    protected void ApplyOrderByDescending(Expression<Func<TEntity, object>> orderByDescendingExpression)
    {
        // If no primary ordering yet, set it; otherwise, chain as ThenByDescending
        if (OrderBy == null && OrderByDescending == null)
        {
            OrderByDescending = orderByDescendingExpression;
        }
        else
        {
            ThenByDescending.Add(orderByDescendingExpression);
        }
    }
}