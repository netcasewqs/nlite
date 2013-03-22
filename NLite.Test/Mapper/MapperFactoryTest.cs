using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NLite.Mapping;

namespace NLite.Mapping.Test
{
    [TestFixture]
    public class MapperFactoryTest
    {
        enum MappingFromStatus
        {
            Value1 = 0,
            Value2 = 1
        }

        class MappingFrom
        {
            public int FromID { get; set; }
            public string Name;

            public int Other;

            public MappingFromStatus Status;

            public Guid Guid;
        }

        struct MappingTo
        {
            public int From_id;
            public string Name { get; set; }

            public string Other2;

            public int Status;
            public Guid Guid;
        }

        [Test]
        public void Test()
        {
            var actual = Mapper.Map<string, int>("12");
            Assert.AreEqual(12, actual);


            Assert.AreEqual(1, Mapper.Map<int, decimal?>(1));

            var actualArray = Mapper.Map<IList<int>, decimal?[]>(new List<int> { 1, 2, 3 });
            Assert.AreEqual(2, actualArray[1]);


            var longColl = Mapper.Map<int[], List<long>>(new int[] { 1, 2, 3 });
            Assert.AreEqual(2, longColl[1]);

            var doubleArray = Mapper.Map<List<string>, double[]>(new List<string> { "1.1", "2.2", "3.3" });
            Assert.AreEqual(2.2, doubleArray[1]);


            Mapper
                .CreateMapper<MappingFrom, MappingTo>()
                .IgnoreCase(true)
                .IgnoreUnderscore(true)
               .IgnoreSourceMember(x => x.Guid);

            var guid = Guid.NewGuid();
            var customFrom = new MappingFrom { FromID = 1, Name = "name", Status = MappingFromStatus.Value2, Guid = guid };
            var customTo = Mapper.Map<MappingFrom, MappingTo>(customFrom);
            Assert.AreEqual(1, customTo.From_id);
            Assert.AreEqual("name", customTo.Name);
            Assert.IsNullOrEmpty(customTo.Other2);
            Assert.AreEqual(1, customTo.Status);
            Assert.AreNotEqual(guid, customTo.Guid);
        }
    }
}
