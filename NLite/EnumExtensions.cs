using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NLite.ComponentModel;
using NLite.Log;
using NLite.Internal;
using NLite.Reflection;

namespace NLite
{
    /// <summary>
    /// Author:qswang
    /// Create:2008-11-28
    /// Function:
    /// </summary>
    public static class EnumHelper
    {
        private static Dictionary<Type, EnumAttribute[]> cache = new Dictionary<Type,EnumAttribute[]>();
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T Parse<T>(string value)
        {
            if (string.IsNullOrEmpty(value))
                throw new System.ArgumentNullException("value", "value== null");
            var enumType = typeof(T);
            Guard.NotNull(enumType, "enumType");
            if (!enumType.IsEnum)
                throw new InvalidOperationException("Invalid enum type:" + enumType.Name);

            return (T)Enum.Parse(enumType, value, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T Parse<T>(int value)
        {
            return (T)Enum.ToObject(typeof(T), value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        public static string GetDescription(this Enum enumValue)
        {
            if (enumValue== null)
                throw new System.ArgumentNullException("enumValue", "enumValue== null");

            var enumType = enumValue.GetType();

            var descriptions = GetEnumDescriptions(enumType);
            if (descriptions != null && descriptions.Length > 0)
            {
                foreach (var item in descriptions)
                    if (enumValue.Equals(item.OriginalValue))
                        return StringFormatter.Format(item.Description);
            }
            return enumValue.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="enumType"></param>
        /// <returns></returns>
        public static EnumAttribute[] GetEnumDescriptions(Type enumType)
        {
            if (enumType == null)
                throw new System.ArgumentNullException("enumType", "enumType== null");

            if (!enumType.IsEnum)
                throw new System.ArgumentException("enumType", "Invalid enumType");
          

            if (!cache.ContainsKey(enumType))
            {
                var fields = enumType.GetFields();
                var attrs = new List<EnumAttribute>();


                foreach (FieldInfo fi in fields)
                {
                    object[] tmpAttrs = fi.GetCustomAttributes(typeof(EnumAttribute), false);
                    if (tmpAttrs.Length != 1) continue;

                    var attr = tmpAttrs[0] as EnumAttribute;
                    attr.Name = fi.Name;
                    if (string.IsNullOrEmpty(attr.Description))
                        attr.DefaultDescription = attr.Name;

                   
                    attr.OriginalValue = (Enum)fi.GetValue(null);
                    attr.Value = Convert.ToInt32(attr.OriginalValue);
                    attrs.Add(attr);
                }

                lock(cache)
                    cache[enumType] = attrs.ToArray();
            }

            var descriptions = cache[enumType];
            if (descriptions == null || descriptions.Length <= 0)
                return null;

            return descriptions;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class Enum<T> where T:struct
    {
        public static IEnumerable<T> AsEnumerable()
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }

        public static EnumAttribute[] GetEnumAttributes()
        {
            return EnumHelper.GetEnumDescriptions(typeof(T));
        }

        public static string GetDescription(T value)
        {
            return EnumHelper.GetDescription((Enum)(object)value);
        }

    }

    /// <summary>
    /// Format strings for commonly-used date formats
    /// </summary>
    public struct DateFormat
    {
        /// <summary>
        /// .NET format string for ISO 8601 date format
        /// </summary>
        public const string Iso8601 = "s";
        /// <summary>
        /// .NET format string for roundtrip date format
        /// </summary>
        public const string RoundTrip = "u";
    }
}
