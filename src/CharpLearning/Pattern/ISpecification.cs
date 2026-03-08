using System;
using System.Linq.Expressions;

namespace CharpLearning.Pattern
{
    public interface ISpecification<T>
    {
        bool IsSatisfiedBy(T entity);

        Expression<Func<T, bool>> AsExpression();
    }

    public abstract class SpecificationBase<T> : ISpecification<T>
    {
        public abstract Expression<Func<T, bool>> AsExpression();
        public bool IsSatisfiedBy(T entity)
        {
            var exp = AsExpression().Compile();

            return exp(entity);
        }
    }

    public class AndSpecification<T> : SpecificationBase<T>
    {
        private readonly ISpecification<T> leftSpec;
        private readonly ISpecification<T> rigthSpec;

        public AndSpecification(ISpecification<T> left, ISpecification<T> rightSpec)
        {
            this.leftSpec = left;
            this.rigthSpec = rightSpec;
        }

        public override Expression<Func<T, bool>> AsExpression()
        {
            var left = leftSpec.AsExpression();
            var right = rigthSpec.AsExpression();

            var param = Expression.Parameter(typeof(T));
            var body = Expression.AndAlso(Expression.Invoke(left, param), Expression.Invoke(right, param));
            return Expression.Lambda<Func<T, bool>>(body, param);
        }
    }

    public class OrSpecification<T> : SpecificationBase<T>
    {
        private readonly ISpecification<T> leftSpec;
        private readonly ISpecification<T> rigthSpec;

        public OrSpecification(ISpecification<T> left, ISpecification<T> rightSpec)
        {
            this.leftSpec = left;
            this.rigthSpec = rightSpec;
        }

        public override Expression<Func<T, bool>> AsExpression()
        {
            var left = leftSpec.AsExpression();
            var right = rigthSpec.AsExpression();

            var param = Expression.Parameter(typeof(T));
            var body = Expression.OrElse(Expression.Invoke(left, param), Expression.Invoke(right, param));
            return Expression.Lambda<Func<T, bool>>(body, param);
        }
    }

    public class Account
    {
        public bool IsActive { get; set; }

        public decimal Amount { get; set; }
    }
}
