using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.ComponentModel;

namespace NLite.Binding
{
    internal static class ParameterInfoUtil
    {
        public static bool TryGetDefaultValue(ParameterInfo parameterInfo, out object value)
        {
            object defaultValue = parameterInfo.DefaultValue;
            if (defaultValue != DBNull.Value)
            {
                value = defaultValue;
                return true;
            }

            DefaultValueAttribute[] attrs = (DefaultValueAttribute[])parameterInfo.GetCustomAttributes(typeof(DefaultValueAttribute), false);
            if (attrs == null || attrs.Length == 0)
            {
                value = default(object);
                return false;
            }
            else
            {
                value = attrs[0].Value;
                return true;
            }
        }

        public static bool IsNullableValueType(Type type)
        {
            return Nullable.GetUnderlyingType(type) != null;
        }

        public static bool TypeAllowsNullValue(Type type)
        {
            return (!type.IsValueType || IsNullableValueType(type));
        }

        public static object GetDefaultValue(Type type)
        {
            return (TypeAllowsNullValue(type)) ? null : Activator.CreateInstance(type);
        }
    }
}
