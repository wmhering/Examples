using System.Linq.Expressions;

namespace EFCoreSpecificationExample.BLL;

abstract public class Specification<T>
{
    public abstract Expression<Func<T, bool>> ToExpression();

    public bool IsSatisfiedBy(T entity) =>
        ToExpression().Compile()(entity);

    public Specification<T> And(Specification<T> other) =>
        new AndSpecification<T>(this, other ?? throw new ArgumentNullException(nameof(other)));

    public Specification<T> AndNot(Specification<T> other) =>
        new AndSpecification<T>(this, new NotSpecification<T>(other ?? throw new ArgumentNullException(nameof(other))));

    public Specification<T> Or(Specification<T> other) =>
        new OrSpecification<T>(this, other ?? throw new ArgumentNullException(nameof(other)));

    public Specification<T> OrNot(Specification<T> other) =>
        new OrSpecification<T>(this, new NotSpecification<T>(other ?? throw new ArgumentNullException(nameof(other))));

    public Specification<T> Not() =>
        new NotSpecification<T>(this);

    public Specification<T> Not(Specification<T> operand) =>
        new NotSpecification<T>(operand ?? throw new ArgumentNullException(nameof(operand)));
}

public sealed class AndSpecification<T> : Specification<T>
{
    private readonly Expression<Func<T, bool>> _expression;

    public AndSpecification(Specification<T> left, Specification<T> right)
    {
        var leftLambda = (left ?? throw new ArgumentNullException(nameof(left))).ToExpression();
        var rightLambda = (right ?? throw new ArgumentNullException(nameof(right))).ToExpression();
        var newParameter = Expression.Parameter(typeof(T));
        var parameterReplacer = new ReplaceParameterVisitor();
        var newBody = Expression.AndAlso(
            parameterReplacer.Modify(leftLambda.Body, leftLambda.Parameters[0], newParameter),
            parameterReplacer.Modify(rightLambda.Body, rightLambda.Parameters[0], newParameter));
        _expression = Expression.Lambda<Func<T, bool>>(newBody, newParameter);
    }

    public override Expression<Func<T, bool>> ToExpression() => _expression;
}

public sealed class OrSpecification<T> : Specification<T>
{
    private readonly Expression<Func<T, bool>> _expression;

    public OrSpecification(Specification<T> left, Specification<T> right)
    {
        var leftLambda = (left ?? throw new ArgumentNullException(nameof(left))).ToExpression();
        var rightLambda = (right ?? throw new ArgumentNullException(nameof(right))).ToExpression();
        var newParameter = Expression.Parameter(typeof(T));
        var parameterReplacer = new ReplaceParameterVisitor();
        var newBody = Expression.OrElse(
            parameterReplacer.Modify(leftLambda.Body, leftLambda.Parameters[0], newParameter),
            parameterReplacer.Modify(rightLambda.Body, rightLambda.Parameters[0], newParameter));
        _expression = Expression.Lambda<Func<T, bool>>(newBody, newParameter);
    }

    public override Expression<Func<T, bool>> ToExpression() => _expression;
}

public sealed class NotSpecification<T> : Specification<T>
{
    private readonly Expression<Func<T, bool>> _expression;

    public NotSpecification(Specification<T> operand)
    {
        var oldLambda = operand.ToExpression();
        var newParameter = Expression.Parameter(typeof(T));
        var parameterReplacer = new ReplaceParameterVisitor();
        var newBody = Expression.Not(
            parameterReplacer.Modify(oldLambda.Body, oldLambda.Parameters[0], newParameter));
        _expression = Expression.Lambda<Func<T, bool>>(newBody, newParameter);
    }

    public override Expression<Func<T, bool>> ToExpression() => _expression;
}

internal class ReplaceParameterVisitor : ExpressionVisitor
{
    private ParameterExpression _oldParameter;
    private ParameterExpression _newParameter;

    public Expression Modify(Expression expression, ParameterExpression oldParameter, ParameterExpression newParameter)
    {
        _oldParameter = oldParameter ?? throw new ArgumentNullException(nameof(oldParameter));
        _newParameter = newParameter ?? throw new ArgumentNullException(nameof(newParameter));
        return  Visit(expression ?? throw new ArgumentNullException(nameof(expression)));
    }

    protected override Expression VisitParameter(ParameterExpression node) =>
        node == _oldParameter ? _newParameter : node;
}