//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Reflection;
//using NLite.Reflection;

//namespace NLite.Mapping.Internal
//{
//    class NullableMapper:MapperBase
//    {
       
//        public NullableMapper(Type fromType, Type toType)
//            : base(fromType, toType)
//        {
//        }

//        public override void Map(ref object from, ref object to)
//        {
//            to = ChangeType(from, _Info.From, _Info.To);
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

//            var isFromNullable = fromType.IsNullable();
//            var isToNullable = toType.IsNullable();

//            if (from == null)
//            {
//                if (isToNullable)
//                    return null;
//                return ObjectCreator.Create(toType);
//            }

//            if (isFromNullable && !isToNullable)
//                return ChangeType(from, Nullable.GetUnderlyingType(fromType), toType);

//            if (fromType.IsClass)
//            {
//                if (fromType != Types.String)
//                {
//                    //if (isToNullable)
//                    //    return null;
//                    return ObjectCreator.Create(toType);
//                }

//                 var str = from as string;
//                 if (string.IsNullOrEmpty(str))
//                     return ObjectCreator.Create(toType);

//                var ut = toType;
//                if (isToNullable)
//                    ut = Nullable.GetUnderlyingType(toType);

               
//                switch (Type.GetTypeCode(ut))
//                {
//                    case TypeCode.Int16: return Int16.Parse(str);
//                    case TypeCode.Int32: return Int32.Parse(str);
//                    case TypeCode.Boolean: return Boolean.Parse(str);
//                    case TypeCode.Byte: return Byte.Parse(str);
//                    case TypeCode.Char: return Char.Parse(str);
//                    case TypeCode.DateTime: return DateTime.Parse(str);
//                    case TypeCode.Decimal: return Decimal.Parse(str);
//                    case TypeCode.Double: return Double.Parse(str);
//                    case TypeCode.Empty: return null;
//                    case TypeCode.Int64: return Int64.Parse(str);
//                    case TypeCode.SByte: return SByte.Parse(str);
//                    case TypeCode.Single: return Single.Parse(str);
//                    case TypeCode.UInt16: return UInt16.Parse(str);
//                    case TypeCode.UInt32: return UInt32.Parse(str);
//                    case TypeCode.UInt64: return UInt64.Parse(str);
//                    case TypeCode.DBNull: return DBNull.Value;
//                    case TypeCode.Object:
//                            if (isToNullable)
//                                return null;
//                            if (ut == Types.Guid)
//                                return new Guid(str);
//                            throw new InvalidCastException(string.Format("{0}->{1}", fromType.Name, ut.Name));
//                }
//            }

//            if (isToNullable)
//            {
//                var ut = Nullable.GetUnderlyingType(toType);
//                return ChangeType(from, fromType, ut);
//            }
//            return Mapper.Map(from,fromType,toType);
//        }

//    }
//}
