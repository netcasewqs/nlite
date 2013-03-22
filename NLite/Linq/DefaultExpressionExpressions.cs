using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using NLite.Reflection;

namespace NLite.Linq
{
#if !SDK35
    public static class DefaultExpressionExpressions
    {
        public static Expression GetDefaultExpression(Type type)
        {
            return Expression.Default(type);
        }
    }
#else


    public static class DefaultExpressionExpressions
    {
        public static Expression GetDefaultExpression(Type type)
        {
            if (type.IsNullable())
                return Expression.Constant(null, type);

            if (type.IsEnum)
                return Expression.Constant(Activator.CreateInstance(type), type);

           return  GetDefaultValueExpression(type);
        }

        private static Expression GetDefaultValueExpression(Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Empty:
                case TypeCode.DBNull:
                case TypeCode.String:
                    return Expression.Constant(null, type);
                case TypeCode.DateTime:
                    return Expression.Constant(DateTime.MinValue, type);
                case TypeCode.Boolean:
                    return Expression.Constant(false, type);
                case TypeCode.Char:
                    return Expression.Constant('\0', type);
                case TypeCode.SByte:
                    return Expression.Constant((sbyte)0, type);
                case TypeCode.Byte:
                    return Expression.Constant((byte)0, type);
                case TypeCode.Int16:
                    return Expression.Constant((Int16)0, type);
                case TypeCode.UInt16:
                    return Expression.Constant((UInt16)0, type);
                case TypeCode.Int32:
                    return Expression.Constant(0, type);
                case TypeCode.UInt32:
                    return Expression.Constant((uint)0, type);
                case TypeCode.Int64:
                    return Expression.Constant((long)0, type);
                case TypeCode.UInt64:
                    return Expression.Constant((ulong)0, type);
                case TypeCode.Single:
                    return Expression.Constant(0f, type);
                case TypeCode.Double:
                    return Expression.Constant(0.0, type);
                case TypeCode.Decimal:
                    return Expression.Constant(0m, type);
                case TypeCode.Object:
                    if (type == Types.Guid)
                        return Expression.Constant(Guid.Empty, type);
                    break;
            }
            return Expression.Constant(null, type);
        }
    }
#endif

}
