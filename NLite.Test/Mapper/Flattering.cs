using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace NLite.Mapping.Test
{
    [TestFixture]
    public class Flattering
    {
        public class ModelObject
        {
            public DateTime BaseDate { get; set; }
            public ModelSubObject Sub { get; set; }
            public ModelSubObject Sub2 { get; set; }
            public ModelSubObject SubWithExtraName { get; set; }
        }

        public class ModelSubObject
        {
            public string ProperName { get; set; }
            public ModelSubSubObject SubSub { get; set; }
        }

        public class ModelSubSubObject
        {
            public string IAmACoolProperty { get; set; }
        }

        public class ModelDto
        {
            public DateTime BaseDate { get; set; }
            public string SubProperName { get; set; }
            public string Sub2ProperName { get; set; }
            public string SubWithExtraNameProperName { get; set; }
            public string SubSubSubIAmACoolProperty { get; set; }
        }

        [Test]
        public void TestFlattering()
        {
            var source = new ModelObject
            {
                BaseDate = DateTime.Now,
                Sub = new ModelSubObject
                {
                    ProperName = "Some name",
                    SubSub = new ModelSubSubObject
                    {
                        IAmACoolProperty = "Cool daddy-o"
                    }
                },
                Sub2 = new ModelSubObject
                {
                    ProperName = "Sub 2 name"
                },
                SubWithExtraName = new ModelSubObject
                {
                    ProperName = "Some other name"
                },
            };

            var b =Mapper.Map<ModelObject, ModelDto>(source);

            Assert.AreEqual(source.BaseDate, b.BaseDate);
            Assert.AreEqual(source.Sub.ProperName, b.SubProperName);
            Assert.AreEqual(source.Sub2.ProperName, b.Sub2ProperName);
            Assert.AreEqual(source.SubWithExtraName.ProperName, b.SubWithExtraNameProperName);
            Assert.AreEqual(source.Sub.SubSub.IAmACoolProperty, b.SubSubSubIAmACoolProperty);
        }
    }
}
