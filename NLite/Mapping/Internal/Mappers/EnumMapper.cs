//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using NLite.Reflection;
//using System.Reflection;

//namespace NLite.Mapping.Internal
//{
//     enum EnumMappingFlags
//    {
//        EnumToEnum,
//        EnumToUnderlying,
//        EnumToString,
//    }

//    sealed class EnumMapper:MapperBase
//    {

//        public EnumMapper(Type fromType, Type toType)
//            : base(fromType, toType)
//        {
//        }

      
//        public override void Map(ref object from, ref object to)
//        {
//            if(from != null)
//                to = ChangeType(from, _Info.From, _Info.To);
//        }

//        private static object ChangeType(object from, Type fromType, Type toType)
//        {
//            if (toType == Types.String)
//            {
//                object to = null;
//                new StringMapper(fromType, toType).Map(ref from, ref to);
//                return to;
//            }

//            if (fromType == toType)
//                return from;

//            if (toType.IsEnum)
//                return ConvertToEnum(fromType, toType, from);
//            if (fromType.IsEnum)
//                return ChangeType(Convert.ChangeType(from, Enum.GetUnderlyingType(fromType)), Enum.GetUnderlyingType(fromType), toType);

//            var isFromNullable = fromType.IsNullable();
//            var isToNullable = toType.IsNullable();

//            if (isFromNullable && !isToNullable)
//                return ChangeType(from, Nullable.GetUnderlyingType(fromType), toType);

//            if (isToNullable)
//            {
//                var ut = Nullable.GetUnderlyingType(toType);
//                if (ut.IsEnum)
//                    return ConvertToEnum(fromType, ut, from);
//                else
//                    return ChangeType(from, fromType, ut);
//            }
//            return Convert.ChangeType(from, toType);
//        }

//        private static object ConvertToEnum(Type fromType, Type toType, object from)
//        {
//            if (!fromType.IsEnum)
//                if (fromType == Types.String)
//                    return Enum.Parse(toType, (string)from);

//            return Enum.ToObject(toType, Convert.ChangeType(from, Enum.GetUnderlyingType(toType)));
//        }

       
//    }
//}
