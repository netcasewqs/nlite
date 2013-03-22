using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using NLite.Reflection;

namespace NLite.Linq
{
    public static partial class Expressions
    {
        public static ConditionalExpression Condition(this Expression test, Expression ifTrue, Expression ifFalse)
        {
            return Expression.Condition(test, ifTrue, ifFalse);
        }

#if SDK4
        public static BinaryExpression Assign(this Expression left, Expression right)
        {
            return Expression.Assign(left, right);
        }
#endif

        public static MemberExpression Property(this Expression expression, string propertyName)
        {
            return Expression.Property(expression, propertyName);
        }

        public static NewExpression New(this Type type)
        {
            return Expression.New(type);
        }

        public static UnaryExpression TypeAs(this Expression expression, Type type)
        {
            return Expression.TypeAs(expression, type);
        }


        public static UnaryExpression Convert(this Expression expression, Type type)
        {
            return Expression.Convert(expression, type);
        }


        public static Expression Equal(this Expression expression1, Expression expression2)
        {
            ConvertExpressions(ref expression1, ref expression2);
            return Expression.Equal(expression1, expression2);
        }

        public static Expression NotEqual(this Expression expression1, Expression expression2)
        {
            ConvertExpressions(ref expression1, ref expression2);
            return Expression.NotEqual(expression1, expression2);
        }

        public static Expression GreaterThan(this Expression expression1, Expression expression2)
        {
            ConvertExpressions(ref expression1, ref expression2);
            return Expression.GreaterThan(expression1, expression2);
        }

        public static Expression GreaterThanOrEqual(this Expression expression1, Expression expression2)
        {
            ConvertExpressions(ref expression1, ref expression2);
            return Expression.GreaterThanOrEqual(expression1, expression2);
        }

        public static Expression LessThan(this Expression expression1, Expression expression2)
        {
            ConvertExpressions(ref expression1, ref expression2);
            return Expression.LessThan(expression1, expression2);
        }

        public static Expression LessThanOrEqual(this Expression expression1, Expression expression2)
        {
            ConvertExpressions(ref expression1, ref expression2);
            return Expression.LessThanOrEqual(expression1, expression2);
        }

        public static Expression And(this Expression expression1, Expression expression2)
        {
            ConvertExpressions(ref expression1, ref expression2);
            return Expression.And(expression1, expression2);
        }

        public static Expression Or(this Expression expression1, Expression expression2)
        {
            ConvertExpressions(ref expression1, ref expression2);
            return Expression.Or(expression1, expression2);
        }

        public static Expression Binary(this Expression expression1, ExpressionType op, Expression expression2)
        {
            ConvertExpressions(ref expression1, ref expression2);
            return Expression.MakeBinary(op, expression1, expression2);
        }

        private static void ConvertExpressions(ref Expression expression1, ref Expression expression2)
        {
            var leftType = expression1.Type;
            var rightType = expression2.Type;
            if (leftType != rightType)
            {
                var isNullable1 = leftType.IsNullable();
                var isNullable2 = rightType.IsNullable();
                if (isNullable1 || isNullable2)
                {
                    var type1 = isNullable1 ? Nullable.GetUnderlyingType(leftType) : leftType;
                    var type2 = isNullable2 ? Nullable.GetUnderlyingType(rightType) : rightType;

                    if (type1 == type2)
                    {
                        if (!isNullable1)
                            expression1 = Expression.Convert(expression1, rightType);
                        else if (!isNullable2)
                            expression2 = Expression.Convert(expression2, leftType);
                    }
                }
            }
        }

        public static Expression[] Split(this Expression expression, params ExpressionType[] binarySeparators)
        {
            var list = new List<Expression>();
            Split(expression, list, binarySeparators);
            return list.ToArray();
        }

        private static void Split(Expression expression, List<Expression> list, ExpressionType[] binarySeparators)
        {
            if (expression != null)
            {
                if (binarySeparators.Contains(expression.NodeType))
                {
                    var bex = expression as BinaryExpression;
                    if (bex != null)
                    {
                        Split(bex.Left, list, binarySeparators);
                        Split(bex.Right, list, binarySeparators);
                    }
                }
                else
                {
                    list.Add(expression);
                }
            }
        }

        public static Expression Join(this IEnumerable<Expression> list, ExpressionType binarySeparator)
        {
            if (list != null)
            {
                var array = list.ToArray();
                if (array.Length > 0)
                {
                    return array.Aggregate((x1, x2) => Expression.MakeBinary(binarySeparator, x1, x2));
                }
            }
            return null;
        }
    }
}
