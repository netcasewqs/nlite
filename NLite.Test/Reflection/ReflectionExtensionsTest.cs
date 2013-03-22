using System.Collections.Generic;
using System.Reflection;
using NLite.Reflection;
using NUnit.Framework;


namespace NLite.Test.Reflection
{
    [TestFixture]
    public class ReflectionExtensionsTest
    {
        [Test]
        public void Test()
        {
            object lst = new List<int>();
            lst.Proc("Add", 1);
            Assert.IsTrue(lst.Func<int>("get_Item", 0) == 1);

            lst.SetProperty("Item", 2, 0);
            Assert.IsTrue(lst.GetProperty<int>("Item", 0) == 2);
        }

        [Test]
        public void AttributeTest()
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            var attr = asm.GetAttribute<AssemblyCopyrightAttribute>(false);
            Assert.IsNotNull(attr);
        }
        
        [Test]
        public void EmitCtorTest()
        {
        	
        }
    }
}
