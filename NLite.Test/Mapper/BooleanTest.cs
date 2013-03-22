using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NLite;
using System.Reflection;
using System.ComponentModel;

namespace NLite.Mapping.Test
{
    [TestFixture]
    public class MapperTest
    {
        [Test]
        public void Test()
        {
            
            Assert.AreEqual("5", Mapper.Map<int, string>(5));

            Assert.AreEqual(true, Mapper.Map<int, bool>(1));
            Assert.AreEqual(true, Mapper.Map<int, bool>(3));
            Assert.AreEqual(false, Mapper.Map<int, bool>(0));
            Assert.AreEqual(5L, Mapper.Map<int, long>(5));
            Assert.AreEqual(5, Mapper.Map<string, int>("5"));

            #region Nullable

            #region Boolean

            Assert.AreEqual(null, Mapper.Map<object, bool?>(null));
            //Assert.AreEqual(null,Mapper.Map<object, bool?>(new object()));
            //Assert.AreEqual(null, Mapper.Map<object, bool?>(new List<int>()));

            Assert.AreEqual(true, Mapper.Map<bool, bool?>(true));
            Assert.AreEqual(false, Mapper.Map<bool, bool?>(false));

            Assert.AreEqual(true, Mapper.Map<bool?, bool>(true));
            Assert.AreEqual(false, Mapper.Map<bool?, bool>(false));
            Assert.AreEqual(false, Mapper.Map<bool?, bool>(null));

            Assert.AreEqual(true, Mapper.Map<sbyte, bool>(1));
            Assert.AreEqual(true, Mapper.Map<sbyte, bool>(2));
            Assert.AreEqual(true, Mapper.Map<sbyte, bool>(-1));
            Assert.AreEqual(true, Mapper.Map<sbyte, bool>(-2));
            Assert.AreEqual(false, Mapper.Map<sbyte, bool>(0));
            Assert.AreEqual(true, Mapper.Map<sbyte?, bool>(1));
            Assert.AreEqual(false, Mapper.Map<sbyte?, bool>(null));
            Assert.AreEqual(false, Mapper.Map<sbyte?, bool>(0));


            //Assert.AreEqual(true, Mapper.Map<char, bool>('A'));
            //Assert.AreEqual(false, Mapper.Map<char, bool>('0'));
            Assert.AreEqual(false, Mapper.Map<char?, bool>(null));


            Assert.AreEqual(true, Mapper.Map<Byte, bool>(1));
            Assert.AreEqual(false, Mapper.Map<Byte, bool>(0));
            Assert.AreEqual(false, Mapper.Map<Byte?, bool>(null));

            Assert.AreEqual(true, Mapper.Map<Int16, bool>(1));
            Assert.AreEqual(false, Mapper.Map<Int16, bool>(0));
            Assert.AreEqual(false, Mapper.Map<Int16?, bool>(null));

            Assert.AreEqual(true, Mapper.Map<UInt16, bool>(1));
            Assert.AreEqual(false, Mapper.Map<UInt16, bool>(0));
            Assert.AreEqual(false, Mapper.Map<UInt16?, bool>(null));

            Assert.AreEqual(true, Mapper.Map<Int32, bool>(1));
            Assert.AreEqual(false, Mapper.Map<Int32, bool>(0));
            Assert.AreEqual(false, Mapper.Map<Int32?, bool>(null));

            Assert.AreEqual(true, Mapper.Map<UInt32, bool>(1));
            Assert.AreEqual(false, Mapper.Map<UInt32, bool>(0));
            Assert.AreEqual(false, Mapper.Map<UInt32?, bool>(null));

            Assert.AreEqual(true, Mapper.Map<Int64, bool>(1));
            Assert.AreEqual(false, Mapper.Map<Int64, bool>(0));
            Assert.AreEqual(false, Mapper.Map<Int64?, bool>(null));

            Assert.AreEqual(true, Mapper.Map<UInt64, bool>(1));
            Assert.AreEqual(false, Mapper.Map<UInt64, bool>(0));
            Assert.AreEqual(false, Mapper.Map<UInt64?, bool>(null));

            Assert.AreEqual(true, Mapper.Map<string, bool?>("true"));
            Assert.AreEqual(true, Mapper.Map<string, bool?>("TRUE"));
            Assert.AreEqual(false, Mapper.Map<string, bool?>("False"));
            Assert.AreEqual(null, Mapper.Map<string, bool?>(null));


            Assert.AreEqual(true, Mapper.Map<Single, bool>(1));
            Assert.AreEqual(false, Mapper.Map<Single, bool>(0));
            Assert.AreEqual(false, Mapper.Map<Single?, bool>(null));

            Assert.AreEqual(true, Mapper.Map<Double, bool>(1));
            Assert.AreEqual(false, Mapper.Map<Double, bool>(0));
            Assert.AreEqual(false, Mapper.Map<Double?, bool>(null));

            Assert.AreEqual(true, Mapper.Map<Decimal, bool>(1));
            Assert.AreEqual(false, Mapper.Map<Decimal, bool>(0));
            Assert.AreEqual(false, Mapper.Map<Decimal?, bool>(null));

            Assert.Throws<InvalidCastException>(() => Mapper.Map<DateTime, bool>(DateTime.Now));
            Assert.Throws<InvalidCastException>(() => Mapper.Map<DateTime, bool>(DateTime.Now));
            Assert.AreEqual(false, Mapper.Map<DateTime?, bool>(null));


            
            #endregion


            #region Char
            //Assert.AreEqual('A', Mapper.Map<object, Char?>('A'));
            //Assert.AreEqual(null, Mapper.Map<object, Char?>(null));
            //Assert.AreEqual(null, Mapper.Map<object, Char?>(new object()));
            //Assert.AreEqual(null, Mapper.Map<object, Char?>(new List<int>()));

            //Assert.AreEqual(1, Mapper2.Map<bool, Char?>(true));
            //Assert.AreEqual(0, Mapper2.Map<bool, Char?>(false));
            //Assert.AreEqual(false, Mapper2.Map<bool?, Char?>(null));

            #endregion
            #endregion

            Assert.IsFalse(Mapper.Map<sbyte?, Boolean?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<Char?, Boolean?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<Byte?, Boolean?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<Int16?, Boolean?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<UInt16?, Boolean?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<Int32?, Boolean?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<UInt32?, Boolean?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<Int64?, Boolean?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<UInt64?, Boolean?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<Single?, Boolean?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<Double?, Boolean?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<Decimal?, Boolean?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<DateTime?, Boolean?>(null).HasValue);

            Assert.IsFalse(Mapper.Map<Boolean?, Char?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<sbyte?, Char?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<Byte?, Char?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<Int16?, Char?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<UInt16?, Char?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<Int32?, Char?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<UInt32?, Char?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<Int64?, Char?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<UInt64?, Char?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<Single?, Char?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<Double?, Char?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<Decimal?, Char?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<DateTime?, Char?>(null).HasValue);


            Assert.IsFalse(Mapper.Map<Boolean?, sbyte?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<Char?, sbyte?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<Byte?, sbyte?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<Int16?, sbyte?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<UInt16?, sbyte?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<Int32?, sbyte?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<UInt32?, sbyte?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<Int64?, sbyte?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<UInt64?, sbyte?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<Single?, sbyte?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<Double?, sbyte?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<Decimal?, sbyte?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<DateTime?, sbyte?>(null).HasValue);


            Assert.IsFalse(Mapper.Map<Boolean?, byte?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<Char?, byte?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<sbyte?, byte?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<Int16?, byte?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<UInt16?, byte?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<Int32?, byte?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<UInt32?, byte?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<Int64?, byte?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<UInt64?, byte?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<Single?, byte?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<Double?, byte?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<Decimal?, byte?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<DateTime?, byte?>(null).HasValue);

            Assert.IsFalse(Mapper.Map<Boolean?, Int16?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<Char?, Int16?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<sbyte?, Int16?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<byte?, Int16?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<UInt16?, Int16?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<Int32?, Int16?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<UInt32?, Int16?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<Int64?, Int16?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<UInt64?, Int16?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<Single?, Int16?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<Double?, Int16?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<Decimal?, Int16?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<DateTime?, Int16?>(null).HasValue);

            Assert.IsFalse(Mapper.Map<Boolean?, UInt16?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<Char?, UInt16?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<sbyte?, UInt16?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<byte?, UInt16?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<Int16?, UInt16?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<Int32?, UInt16?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<UInt32?, UInt16?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<Int64?, UInt16?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<UInt64?, UInt16?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<Single?, UInt16?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<Double?, UInt16?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<Decimal?, UInt16?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<DateTime?, UInt16?>(null).HasValue);

            Assert.IsFalse(Mapper.Map<Boolean?, Int32?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<Char?, Int32?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<sbyte?, Int32?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<byte?, Int32?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<Int16?, Int32?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<UInt16?, Int32?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<UInt32?, Int32?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<Int64?, Int32?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<UInt64?, Int32?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<Single?, Int32?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<Double?, Int32?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<Decimal?, Int32?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<DateTime?, Int32?>(null).HasValue);

            Assert.IsFalse(Mapper.Map<Boolean?, UInt32?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<Char?, UInt32?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<sbyte?, UInt32?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<byte?, UInt32?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<Int16?, UInt32?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<UInt16?, UInt32?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<Int32?, UInt32?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<Int64?, UInt32?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<UInt64?, UInt32?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<Single?, UInt32?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<Double?, UInt32?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<Decimal?, UInt32?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<DateTime?, UInt32?>(null).HasValue);

            Assert.IsFalse(Mapper.Map<Boolean?, Int64?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<Char?, Int64?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<sbyte?, Int64?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<byte?, Int64?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<Int16?, Int64?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<UInt16?, Int64?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<Int32?, Int64?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<UInt32?, Int64?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<UInt64?, Int64?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<Single?, Int64?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<Double?, Int64?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<Decimal?, Int64?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<DateTime?, Int64?>(null).HasValue);

            Assert.IsFalse(Mapper.Map<Boolean?, UInt64?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<Char?, UInt64?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<sbyte?, UInt64?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<byte?, UInt64?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<Int16?, UInt64?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<UInt16?, UInt64?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<Int32?, UInt64?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<UInt32?, UInt64?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<Int64?, UInt64?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<Single?, UInt64?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<Double?, UInt64?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<Decimal?, UInt64?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<DateTime?, UInt64?>(null).HasValue);

            Assert.IsFalse(Mapper.Map<Boolean?, Single?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<Char?, Single?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<sbyte?, Single?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<byte?, Single?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<Int16?, Single?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<UInt16?, Single?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<Int32?, Single?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<UInt32?, Single?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<Int64?, Single?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<UInt64?, Single?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<Double?, Single?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<Decimal?, Single?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<DateTime?, Single?>(null).HasValue);

            Assert.IsFalse(Mapper.Map<Boolean?, Double?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<Char?, Double?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<sbyte?, Double?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<byte?, Double?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<Int16?, Double?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<UInt16?, Double?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<Int32?, Double?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<UInt32?, Double?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<Int64?, Double?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<UInt64?, Double?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<Single?, Double?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<Decimal?, Double?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<DateTime?, Double?>(null).HasValue);

            Assert.IsFalse(Mapper.Map<Boolean?, Decimal?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<Char?, Decimal?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<sbyte?, Decimal?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<byte?, Decimal?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<Int16?, Decimal?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<UInt16?, Decimal?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<Int32?, Decimal?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<UInt32?, Decimal?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<Int64?, Decimal?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<UInt64?, Decimal?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<Single?, Decimal?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<Double?, Decimal?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<DateTime?, Decimal?>(null).HasValue);

            Assert.IsFalse(Mapper.Map<Boolean?, DateTime?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<Char?, DateTime?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<sbyte?, DateTime?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<byte?, DateTime?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<Int16?, DateTime?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<UInt16?, DateTime?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<Int32?, DateTime?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<UInt32?, DateTime?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<Int64?, DateTime?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<UInt64?, DateTime?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<Single?, DateTime?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<Double?, DateTime?>(null).HasValue);
            Assert.IsFalse(Mapper.Map<Decimal?, DateTime?>(null).HasValue);
        }


        [Test]
        public void UtilTest()
        {
            //Assert.IsTrue(Mapper.Map<string, bool>("True"));
            //Assert.IsTrue(Mapper.CreateMapper<string, bool>().ConvertUsing<string, bool>(s =>
            //    {
            //        if (s == "1")
            //            return true;
            //        if (s == "0")
            //            return false;
            //        return bool.Parse(s);
            //    }).Map("1"));

            object d = null;
            object t = Mapper.Map(d, null, typeof(Int32));
            Assert.IsInstanceOf<int>(t);

            var converter = TypeDescriptor.GetConverter(Types.String);
            Console.WriteLine(converter.CanConvertTo(Types.DateTime));
        }

       

        class OrderDto
        {
            public string Date;
        }

        class Order
        {
            public DateTime Date;
        }
        [Test]
        public void DateTest()
        {
            //var expected = DateTime.UtcNow;
            //var order = Mapper.Map<OrderDto, Order>(new OrderDto { Date = expected.ToString() });

            //Console.WriteLine(order.Date);

            //order = Mapper.Map<OrderDto, Order>(new OrderDto { Date = "2011-4-26" });
            //Console.WriteLine(order.Date);

            //order = Mapper.Map<OrderDto, Order>(new OrderDto { Date = "2011/4/26" });
            //Console.WriteLine(order.Date);

            var order = Mapper.Map<IDictionary<string,object>, Order>(new Dictionary<string,object>{ { "date" , "2011/4/26" }});
            Console.WriteLine(order.Date);
        }
    }
}
