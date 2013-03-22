using NLite.Collections;
using NUnit.Framework;

namespace NLite.Test
{
    [TestFixture]
    public class PropertyManagerTest
    {
        public class Person
        {
            public string Name { get; set; }
            public int Age { get; set; }
            public bool Sex { get; set; }
        }

        [Test]
        public void Test()
        {
            var props = PropertyManager.Instance.Properties;
            Assert.IsNotNull(props);

            Assert.IsTrue(props.Get<int>("A") == 1);


            props["B"] = "how are you";

            PropertyManager.Instance.Save();
            var value = PropertyManager.Instance.Properties.Get<string>("B");
            Assert.IsTrue(string.Equals(value, "how are you"));

            var p = props.Get<Person>("Person");
            Assert.IsNotNull(p);

        }
    }
}
