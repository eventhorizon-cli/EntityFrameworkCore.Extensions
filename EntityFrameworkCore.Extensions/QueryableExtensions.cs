using EntityFrameworkCore.Extensions.Storages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace EntityFrameworkCore.Extensions
{
    public static class QueryableExtensions
    {
        public static IQueryable<TTarget> MapTo<TSource, TTarget>(this IQueryable<TSource> source)
            where TTarget : new() => source.Select(AutoMapperStorage<TSource, TTarget>
                .GetOrAdd(CreateMapperExpression<TSource, TTarget>));

        private static Expression<Func<TSource, TTarget>> CreateMapperExpression<TSource, TTarget>()
        {
            Type sourceType = typeof(TSource);
            Type targetType = typeof(TTarget);
            var paramExpr = Expression.Parameter(sourceType, "source");

            var memberBindings = new List<MemberBinding>();
            // 遍历目标对象的所有属性信息
            foreach (var targetPropInfo in targetType.GetProperties())
            {
                // 从源对象获取同名的属性信息
                var sourcePropInfo = sourceType.GetProperty(targetPropInfo.Name);

                Type sourcePropType = sourcePropInfo?.PropertyType;
                Type targetPropType = targetPropInfo.PropertyType;

                // 只在满足以下三个条件的情况下进行拷贝
                // 1.源属性类型和目标属性类型一致
                // 2.源属性可读
                // 3.目标属性可写
                if (sourcePropType == targetPropType
                    && sourcePropInfo.CanRead
                    && targetPropInfo.CanWrite)
                {
                    // 获取属性值的表达式
                    Expression expression = Expression.Property(paramExpr, sourcePropInfo);
                    memberBindings.Add(Expression.Bind(targetPropInfo, expression));
                }
            }

            Expression<Func<TSource, TTarget>> lambdaExpr
                = Expression.Lambda<Func<TSource, TTarget>>(
                    Expression.MemberInit(Expression.New(targetType), memberBindings),
                    paramExpr);

            return lambdaExpr;
        }

    }
}