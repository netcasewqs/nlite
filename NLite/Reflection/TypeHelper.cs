using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections;
using System.Collections.Specialized;
using NLite.Internal;
using NLite.Mapping.Internal;

namespace NLite.Reflection
{
    /// <summary>
    /// 类型助手
    /// </summary>
    public static class TypeHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static bool IsNullable(this Type type)
        {
            Guard.NotNull(type, "type");
            return type.IsGenericType && type.GetGenericTypeDefinition() == Types.Nullable;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Type GetNonNullableType(Type type)
        {
            if (TypeHelper.IsNullable(type))
                return Nullable.GetUnderlyingType(type);
            return type;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object GetDefault(Type type)
        {
            Guard.NotNull(type, "type");
            return ObjectCreator.Create(type);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public static bool IsReadOnly(this MemberInfo member)
        {
            Guard.NotNull(member, "member");
            switch (member.MemberType)
            {
                case MemberTypes.Field:
                    return (((FieldInfo)member).Attributes & FieldAttributes.InitOnly) != 0;
                case MemberTypes.Property:
                    PropertyInfo pi = (PropertyInfo)member;
                    return !pi.CanWrite || pi.GetSetMethod() == null;
                default:
                    return true;
            }
        }
        /// <summary>
        /// 得到集合元素类型
        /// </summary>
        /// <param name="enumerableType">集合类型</param>
        /// <returns>返回集合元素类型</returns>
        internal static Type GetElementType(Type enumerableType)
        {
            Guard.NotNull(enumerableType, "enumerableType");
            return GetElementType(enumerableType, null);
        }

        /// <summary>
        /// 得到枚举类型
        /// </summary>
        /// <param name="enumType">枚举类型或Nullable枚举类型</param>
        /// <returns>返回枚举类型</returns>
        public static Type GetEnumType(Type enumType)
        {
            Guard.NotNull(enumType, "enumType");
            if (enumType.IsNullable())
                enumType = enumType.GetGenericArguments()[0];
            if (enumType.IsEnum)
                return enumType;
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static bool CanConvertToEnum(Type type)
        {
            Guard.NotNull(type, "type");
            if (TypeHelper.IsEnumType(type))
                return true;

            if (type.IsNullable())
                type = Nullable.GetUnderlyingType(type);

            return
                                TypeHelper.IsEnumType(type)
                                || Types.String == type
                                || Types.Decimal == type
                                || Types.Double == type
                                || Types.Int16 == type
                                || Types.Int32 == type
                                || Types.Int64 == type
                                || Types.Single == type
                                || Types.UInt16 == type
                                || Types.UInt32 == type
                                || Types.UInt64 == type;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="enumerableType"></param>
        /// <param name="enumerable"></param>
        /// <returns></returns>
        internal static Type GetElementType(Type enumerableType, IEnumerable enumerable)
        {
            Guard.NotNull(enumerableType, "enumerableType");
            if (enumerableType.HasElementType)
            {
                return enumerableType.GetElementType();
            }
            if (enumerableType.IsGenericType && enumerableType.GetGenericTypeDefinition().Equals(Types.IEnumerableofT))
            {
                return enumerableType.GetGenericArguments()[0];
            }
            Type iEnumerableType = GetIEnumerableType(enumerableType);
            if (iEnumerableType != null)
            {
                return iEnumerableType.GetGenericArguments()[0];
            }
            if (!Types.IEnumerable.IsAssignableFrom(enumerableType))
            {
                throw new ArgumentException(string.Format("Unable to find the element type for type '{0}'.", enumerableType), "enumerableType");
            }
            if (enumerable != null)
            {
                object obj2 = enumerable.Cast<object>().FirstOrDefault<object>();
                if (obj2 != null)
                {
                    return obj2.GetType();
                }
            }
            return typeof(object);
        }

        private static Type GetIEnumerableType(Type enumerableType)
        {
            try
            {
                return enumerableType.GetInterface("IEnumerable`1", false);
            }
            catch (AmbiguousMatchException)
            {
                if (enumerableType.BaseType != typeof(object))
                {
                    return GetIEnumerableType(enumerableType.BaseType);
                }
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static bool IsGuidType(Type type)
        {
            Guard.NotNull(type, "type");
            if (type == Types.Guid)
                return true;

            if (type.IsNullable()
                && type.GetGenericArguments()[0] == typeof(Guid))
                return true;

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static bool IsEnumType(Type type)
        {
            Guard.NotNull(type, "type");
            return GetEnumType(type) != null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static bool IsFlagsEnum(Type type)
        {
            Guard.NotNull(type, "type");
            return TypeHelper.IsEnumType(type) && type.GetCustomAttributes(typeof(FlagsAttribute), false).Any();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static bool IsGenericDictionaryType(this Type type)
        {
            Guard.NotNull(type, "type");
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IDictionary<,>))
                return true;

            var genericInterfaces = type.GetInterfaces().Where(t => t.IsGenericType);
            var baseDefinitions = genericInterfaces.Select(t => t.GetGenericTypeDefinition());
            return baseDefinitions.Any(t => t == typeof(IDictionary<,>));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static bool IsIDictionaryType(this Type type)
        {
            Guard.NotNull(type, "type");
            return Types.IDictionary.IsAssignableFrom(type);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static bool IsDictionaryType(this Type type)
        {
            Guard.NotNull(type, "type");
            return IsIDictionaryType(type) || IsGenericDictionaryType(type);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static Type GetGenericDictionaryType(this Type type)
        {
            Guard.NotNull(type, "type");
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IDictionary<,>))
                return type;

            var genericInterfaces = type.GetInterfaces().Where(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IDictionary<,>));
            return genericInterfaces.FirstOrDefault();
        }

        static bool IsNullableValueType(Type type)
        {
            return Nullable.GetUnderlyingType(type) != null;
        }

        static bool TypeAllowsNullValue(Type type)
        {
            return (!type.IsValueType || IsNullableValueType(type));
        }

       
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsCollectionTypeExcludeStringAndDictionaryType(this Type type)
        {
            Guard.NotNull(type, "type");
            return Types.IEnumerable.IsAssignableFrom(type)
                && type != Types.String
                && !Types.IDictionaryOfStringAndObject.IsAssignableFrom(type);
        }
    }

   
}


namespace NLite
{
    /// <summary>
    /// 常用类型
    /// </summary>
    public static class Types
    {
        /// <summary>
        /// Object 类型
        /// </summary>
        public static readonly Type Object = typeof(Object);

        /// <summary>
        /// Type 类型
        /// </summary>
        public static readonly Type Type = typeof(Type);

        /// <summary>
        /// Stirng 类型
        /// </summary>
        public static readonly Type String = typeof(String);

        /// <summary>
        /// Char 类型
        /// </summary>
        public static readonly Type Char = typeof(Char);

        /// <summary>
        /// Boolean 类型
        /// </summary>
        public static readonly Type Boolean = typeof(Boolean);

        /// <summary>
        /// Byte 类型
        /// </summary>
        public static readonly Type Byte = typeof(Byte);


        /// <summary>
        /// Byte 数组类型
        /// </summary>
        public static readonly Type ByteArray = typeof(Byte[]);

        /// <summary>
        /// SByte 类型
        /// </summary>
        public static readonly Type SByte = typeof(SByte);

        /// <summary>
        /// Int16 类型
        /// </summary>
        public static readonly Type Int16 = typeof(Int16);

        /// <summary>
        /// UInt16 类型
        /// </summary>
        public static readonly Type UInt16 = typeof(UInt16);

        /// <summary>
        /// Int32 类型
        /// </summary>
        public static readonly Type Int32 = typeof(Int32);

        /// <summary>
        /// UInt32 类型
        /// </summary>
        public static readonly Type UInt32 = typeof(UInt32);

        /// <summary>
        /// Int64 类型
        /// </summary>
        public static readonly Type Int64 = typeof(Int64);

        /// <summary>
        /// UInt64 类型
        /// </summary>
        public static readonly Type UInt64 = typeof(UInt64);

        /// <summary>
        /// Double 类型
        /// </summary>
        public static readonly Type Double = typeof(Double);

        /// <summary>
        /// Single 类型
        /// </summary>
        public static readonly Type Single = typeof(Single);

        /// <summary>
        /// Decimal 类型
        /// </summary>
        public static readonly Type Decimal = typeof(Decimal);

        /// <summary>
        /// Guid 类型
        /// </summary>
        public static readonly Type Guid = typeof(Guid);

        /// <summary>
        /// DateTime 类型
        /// </summary>
        public static readonly Type DateTime = typeof(DateTime);

        /// <summary>
        /// TimeSpan 类型
        /// </summary>
        public static readonly Type TimeSpan = typeof(TimeSpan);

        /// <summary>
        /// Nullable 类型
        /// </summary>
        public static readonly Type Nullable = typeof(Nullable<>);

        /// <summary>
        /// ValueType 类型
        /// </summary>
        public static readonly Type ValueType = typeof(ValueType);

        /// <summary>
        /// void 类型
        /// </summary>
        public static readonly Type Void = typeof(void);

        /// <summary>
        /// DBNull 类型
        /// </summary>
        public static readonly Type DBNull = typeof(DBNull);

        /// <summary>
        /// Delegate 类型
        /// </summary>
        public static readonly Type Delegate = typeof(Delegate);

        /// <summary>
        /// ByteEnumerable 类型
        /// </summary>
        public static readonly Type ByteEnumerable = typeof(IEnumerable<Byte>);

        /// <summary>
        /// IEnumerable 类型
        /// </summary>
        public static readonly Type IEnumerableofT = typeof(System.Collections.Generic.IEnumerable<>);

        /// <summary>
        /// IEnumerable 类型
        /// </summary>
        public static readonly Type IEnumerable = typeof(System.Collections.IEnumerable);

        /// <summary>
        /// IListSource 类型
        /// </summary>
        public static readonly Type IListSource = typeof(System.ComponentModel.IListSource);

        /// <summary>
        /// IDictionary 类型
        /// </summary>
        public static readonly Type IDictionary = typeof(System.Collections.IDictionary);

        /// <summary>
        /// IDictionary 类型
        /// </summary>
        public static readonly Type IDictionaryOfT = typeof(IDictionary<,>);
        /// <summary>
        /// Dictionary 类型
        /// </summary>
        public static readonly Type DictionaryOfT = typeof(Dictionary<,>);

        /// <summary>
        /// StringDictionary 类型
        /// </summary>
        public static readonly Type StringDictionary = typeof(StringDictionary);

        /// <summary>
        /// NameValueCollection 类型
        /// </summary>
        public static readonly Type NameValueCollection = typeof(NameValueCollection);

        /// <summary>
        /// IDataReader 类型
        /// </summary>
        public static readonly Type IDataReader = typeof(System.Data.IDataReader);

        /// <summary>
        /// DataTable 类型
        /// </summary>
        public static readonly Type DataTable = typeof(System.Data.DataTable);

        /// <summary>
        /// DataRow 类型
        /// </summary>
        public static readonly Type DataRow = typeof(System.Data.DataRow);

        /// <summary>
        /// IDictionary 类型
        /// </summary>
        public static readonly Type IDictionaryOfStringAndObject = typeof(IDictionary<string, object>);

        /// <summary>
        /// IDictionary 类型
        /// </summary>
        public static readonly Type IDictionaryOfStringAndString = typeof(IDictionary<string, string>);

    }
}