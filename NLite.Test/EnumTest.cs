using System;
using NUnit.Framework;

namespace NLite.Test
{
    [TestFixture]
    public class EnumTest
    {
        [Test]
        public void AsEnumerableTest()
        {
            foreach (var item in Enum<TypeCode>.AsEnumerable())
                Console.WriteLine(item);
        }

        [Test]
        public void ParseTest()
        {
            var code = EnumHelper.Parse<TypeCode>("byte");
            Assert.IsTrue(code == TypeCode.Byte);

            code = EnumHelper.Parse<TypeCode>(4);
            Assert.IsTrue(code == TypeCode.Char);

            var now = DateTime.UtcNow;
            Console.WriteLine(now.Ticks);
            Console.WriteLine(NLite.Dynamic.UnixTime.FromDateTime(now));
        }

    }

    [Flags]
    public enum NIROIType
    {
        NI = 1,
        ROI = NI * 2,
        UK = ROI * 2,
    }

    [Flags]
    public enum LobType
    {
        GS = 1,
        Openreach = GS * 2,
        Retail = Openreach * 2,
        Wholesale = Retail * 2,
    }
}
