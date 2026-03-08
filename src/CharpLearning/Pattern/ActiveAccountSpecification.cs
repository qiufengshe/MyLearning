using System;
using System.Linq.Expressions;

namespace CharpLearning.Pattern
{
    public class ActiveAccountSpecification : SpecificationBase<Account>
    {
        public override Expression<Func<Account, bool>> AsExpression()
        {
            return x => x.IsActive;
        }
    }

    public class AccountAmoutSpecification : SpecificationBase<Account>
    {
        private readonly decimal _amount;
        public AccountAmoutSpecification(decimal amount)
        {
            _amount = amount;
        }
        public override Expression<Func<Account, bool>> AsExpression()
        {
            return e => e.Amount > _amount;
        }
    }

    public static class SpecificationExtension
    {
        public static ISpecification<T> And<T>(this ISpecification<T> left, ISpecification<T> right)
        {
            return new AndSpecification<T>(left, right);
        }

        public static ISpecification<T> Or<T>(this ISpecification<T> left, ISpecification<T> right)
        {
            return new OrSpecification<T>(left, right);
        }
    }
}