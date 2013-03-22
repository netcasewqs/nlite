using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NLite.Reflection;
namespace NLite.Mapping.Test
{
    [TestFixture]
    public class IgnoreByAttributes
    {
        [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
        public class MyIgnoreAttribute : Attribute
        {
        }

        public class IgnoreByAttributesSrc
        {
            [MyIgnoreAttribute]
            public string str1 = "IgnoreByAttributesSrc::str1";
            public string str2 = "IgnoreByAttributesSrc::str2";
        }

        public class IgnoreByAttributesDst
        {
            public string str1 = "IgnoreByAttributesDst::str1";
            public string str2 = "IgnoreByAttributesDst::str2";
        }

     

        [Test]
        public void Test()
        {


            var dst = Mapper.CreateMapper<IgnoreByAttributesSrc, IgnoreByAttributesDst>()
                .IgnoreSourceMembers(t => from f in t.GetFields()
                                           where f.HasAttribute<MyIgnoreAttribute>(true)
                                           select f.Name)
                .Map(new IgnoreByAttributesSrc());

            
            Assert.AreEqual("IgnoreByAttributesDst::str1", dst.str1);
            Assert.AreEqual("IgnoreByAttributesSrc::str2", dst.str2);
        }

    }
}
