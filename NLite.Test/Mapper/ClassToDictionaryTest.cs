using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NLite.Mapping;

namespace NLite.Mapping.Test
{
    [TestFixture]
    public class ClassToDictionaryTest
    {
        
        [TearDown]
        public void TearDown()
        {
            Mapper.Reset();
        }

        public class Person1
        {
            public string Name { get; set; }
            public int Age { get; set; }
            public bool Sex { get; set; }
        }

        [Test]
        public void SimpleTest()
        {
            var source = new Person1 { Name = "Kevin", Age = 30, Sex = true };
            var dst = Mapper.Map<Person1, IDictionary<string, object>>(source);

            Assert.AreEqual(dst["Name"], source.Name);
            Assert.AreEqual(dst["Age"], source.Age);
            Assert.AreEqual(dst["Sex"], source.Sex);
        }

        [Test]
        public void IgnoreMemberTest()
        {
            var source = new Person1 { Name = "Kevin", Age = 30, Sex = true };
            var dst = Mapper
                .CreateMapper<Person1, IDictionary<string, object>>()
                .IgnoreSourceMember(x => x.Sex)
                .Map(source);

            Assert.AreEqual(dst["Name"], source.Name);
            Assert.AreEqual(dst["Age"], source.Age);
            Assert.IsFalse(dst.ContainsKey("Sex"));
        }
    }

    [TestFixture]
    public class DictionaryToClassTest
    {
        public class Person1
        {
            public string Name { get; set; }
            public int Age { get; set; }
            public bool Sex { get; set; }
        }

        public class TeamSearchCondition
        {
            public string BusinessCategory;
            public string NIROI;
            public string Lob;
            [Splite(',')]
            public int[] Product;
            public string FormName;
        }

        [Test]
        public void Test()
        {
            var source = new Dictionary<string, string>();
            source["BusinessCategory"] = "kevin";
            source["NIROI"] = "30";
            source["Lob"] = "true";
            source["Product"] = "1,2";
            source["FormName"] = "true";

            var dst = Mapper.Map<IDictionary<string, string>, TeamSearchCondition>(source);

            Assert.IsNotNull(dst);

            Assert.IsNotNull(dst.Product != null);
            Assert.AreEqual(2, dst.Product.Length);
            Assert.AreEqual(1, dst.Product[0]);
            Assert.AreEqual(2, dst.Product[1]);
        }

        [TearDown]
        public void TearDown()
        {
            Mapper.Reset();
        }


        [Test]
        public void SimpleTest()
        {
            var source = new Dictionary<string, object>();
            source["Name"] = "kevin";
            source["Age"] = 30;
            source["Sex"] = true;

            var dst = Mapper.Map<IDictionary<string, object>, Person1>(source);

            Assert.AreEqual(source["Name"], dst.Name);
            Assert.AreEqual(source["Age"], dst.Age);
            Assert.AreEqual(source["Sex"], dst.Sex);
        }


       

        [Test]
        public void EmptyTest()
        {
            var source = new Dictionary<string, string>();
           

            var dst = Mapper.Map<IDictionary<string, string>, Person1>(source);

            Assert.IsNotNull(dst);
            
        }

        class Account
        {
            public long? EIN;
            public string Password;
        }
        [Test]
        public void EmptyTest2()
        {
            var source = new Dictionary<string, object>();
            source["EIN"] = null;
            source["Password"] = null;

            var dst = Mapper.Map<IDictionary<string, object>, Account>(source);

            Assert.IsNotNull(dst);
        }

        [Test]
        public void EmptyTest3()
        {
            var source = new Dictionary<string, string>();
            source["EIN"] = "";
            source["Password"] = "";

            var dst = Mapper.Map<IDictionary<string, string>, Account>(source);

            Assert.IsNotNull(dst);
        }

        [Test]
        public void EmptyTest4()
        {
            var source = new ParameterDictionary();
            source["EIN"] = "";
            source["Password"] = "";

            var dst = Mapper.Map<IDictionary<string, string>, Account>(source);

            Assert.IsNotNull(dst);
        }

        class ParameterDictionary : Dictionary<string, string>
        {
            public ParameterDictionary()
                : base(StringComparer.OrdinalIgnoreCase)
            {
            }

            public string GetValue(string key, string defaultValue)
            {
                TryGetValue(key, out defaultValue);
                return defaultValue;
            }
        }

        [Test]
        public void IgnoreMemberTest()
        {
            var source = new Dictionary<string, object>();
            source["Name"] = "kevin";
            source["Age"] = 30;
            source["Sex"] = true;

            var dst = Mapper
                .CreateMapper<IDictionary<string, object>, Person1>()
                .IgnoreDestinationMember(x=>x.Name)
                .Map(source);

            Assert.IsNullOrEmpty(dst.Name);
            Assert.AreEqual(source["Age"], dst.Age);
            Assert.AreEqual(source["Sex"], dst.Sex);
        }


        [Test]
        public void IgnoureCaseSimpleTest()
        {
            var source = new Dictionary<string, object>();
            source["name"] = "kevin";
            source["age"] = 30;
            source["sex"] = true;

            var dst = Mapper
                .CreateMapper<IDictionary<string, object>, Person1>()
                .IgnoreCase(true)
                .Map(source);

            Assert.AreEqual(source["name"], dst.Name);
            Assert.AreEqual(source["age"], dst.Age);
            Assert.AreEqual(source["sex"], dst.Sex);
        }
    }
}
