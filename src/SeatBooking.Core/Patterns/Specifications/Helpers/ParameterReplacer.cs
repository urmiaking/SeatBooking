using System.Linq.Expressions;

namespace SeatBooking.Core.Patterns.Specifications.Helpers;

/// <summary>
/// Replaces one parameter expression with another in an expression tree.
/// </summary>
/// <param name="oldParameter"></param>
/// <param name="newParameter"></param>
internal class ParameterReplacer(ParameterExpression oldParameter, ParameterExpression newParameter)
    : ExpressionVisitor
{
    protected override Expression VisitParameter(ParameterExpression node)
    {
        return node == oldParameter ? newParameter : base.VisitParameter(node);
    }
}