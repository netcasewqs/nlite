//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using NLite.Reflection;
//using NLite.Reflection.Internal;

//namespace NLite.Mapping.Internal
//{
//    class FlagsEnumMapper:MapperBase
//    {
//        Type enumDestType;
//        public FlagsEnumMapper(Type fromType, Type toType)
//            : base(fromType, toType)
//        {
//            enumDestType = TypeHelper.GetEnumType(toType);
//        }

//        public override void Map(ref object from, ref object to)
//        {
//            if (from == null)
//                return;
//            to =  Enum.Parse(enumDestType, from.ToString());

//            if (TypeHelper.IsFlagsEnum(_Info.To))
//                to = ConvertToEnum(_Info.From, _Info.To, ref from);
//            else
//            {

//            }
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
//                return ConvertToEnum(fromType, toType, ref from);
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
//                    return ConvertToEnum(fromType, ut,ref  from);
//                else
//                    return ChangeType(from, fromType, ut);
//            }
//            return Convert.ChangeType(from, toType);
//        }


//        private static object ConvertToEnum(Type fromType, Type toType, ref object from)
//        {
//            if (!fromType.IsEnum)
//                if (fromType == Types.String)
//                    return Enum.Parse(toType, (string)from);

//            return Enum.ToObject(toType, Convert.ChangeType(from, Enum.GetUnderlyingType(toType)));
//        }
//    }
//}
