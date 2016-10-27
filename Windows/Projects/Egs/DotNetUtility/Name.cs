namespace DotNetUtility
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Linq.Expressions;
    using System.ComponentModel;

    public static class Name
    {
        public static string Of<T, TResult>(this Expression<Func<T, TResult>> accessor)
        {
            return GetNameOfExpression(accessor.Body);
        }

        public static string Of<T>(this Expression<Func<T>> accessor)
        {
            return GetNameOfExpression(accessor.Body);
        }

        public static string Of<T, TResult>(this T obj, Expression<Func<T, TResult>> propertyAccessor)
        {
            return GetNameOfExpression(propertyAccessor.Body);
        }

        static string GetNameOfExpression(Expression expression)
        {
            if (expression.NodeType == ExpressionType.MemberAccess)
            {
                var memberExpression = expression as MemberExpression;
                if (memberExpression == null) { return ""; }
                return memberExpression.Member.Name;
            }
            return "";
        }
    }
}
