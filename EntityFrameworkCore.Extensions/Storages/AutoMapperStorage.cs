using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace EntityFrameworkCore.Extensions.Storages
{
    internal class AutoMapperStorage<TSource, TTarget>
    {
        private static Expression<Func<TSource, TTarget>> _expression;

        public static Expression<Func<TSource, TTarget>> GetOrAdd(
            Func<Expression<Func<TSource, TTarget>>> expressionFactory) =>
            _expression ?? (_expression = expressionFactory());
    }
}
