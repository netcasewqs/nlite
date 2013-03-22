//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using NUnit.Framework;
//using NLite.Serialization;
//using System.Xml.Linq;
//using System.Globalization;
//using NLite.Reflection;

//namespace NLite.Test.Serialization
//{
//    [TestFixture]
//    public class SerializeTest
//    {
//        private class PersonList : List<Person>
//        {

//        }
//        private class Person
//        {
//            public string Name { get; set; }
//            public int Age { get; set; }
//            public decimal Price { get; set; }
//            public DateTime StartDate { get; set; }
//            public List<Item> Items { get; set; }
//            public bool? IsCool { get; set; }
//        }

//        private class Item
//        {
//            public string Name { get; set; }
//            public int Value { get; set; }
//        }

//        interface IEntity
//        {
//            int Id { get; set; }
//        }
//        class Entity : IEntity
//        {
//            public int Id { get; set; }
//        }
//        class CategoryInfo:Entity
//        {
//            public string Name { get; set; }
//        }

//        [Test]
//        public void Can_serializer_memberCount_is_2()
//        {
//            var type = typeof(CategoryInfo);
//            var members = type.GetGetMembers();
//            Assert.AreEqual(2, members.Length);
//        }


//        [Test]
//        public void Can_serialize_simple_POCO()
//        {
//            var poco = new Person
//            {
//                Name = "Foo",
//                Age = 50,
//                Price = 19.95m,
//                StartDate = new DateTime(2009, 12, 18, 10, 2, 23),
//                Items = new List<Item> {
//                    new Item { Name = "One", Value = 1 },
//                    new Item { Name = "Two", Value = 2 },
//                    new Item { Name = "Three", Value = 3 }
//                }
//            };

//            var xml = new XmlSerializer();
//            var doc = xml.Serialize(poco);
//            var expected = GetSimplePocoXDoc();
//            Console.WriteLine(doc);
//            Assert.AreEqual(expected.ToString(), doc.ToString());
//        }

//        [Test]
//        public void Can_serialize_simple_POCO_With_DateFormat_Specified()
//        {
//            var poco = new Person
//            {
//                Name = "Foo",
//                Age = 50,
//                Price = 19.95m,
//                StartDate = new DateTime(2009, 12, 18, 10, 2, 23)
//            };

//            var xml = new XmlSerializer();
//            xml.DateFormat = DateFormat.Iso8601;
//            var doc = xml.Serialize(poco);
//            var expected = GetSimplePocoXDocWithIsoDate();
//            Console.WriteLine(doc);
//            Assert.AreEqual(expected.ToString(), doc.ToString());
//        }

//        [Test]
//        public void Can_serialize_simple_POCO_With_XmlFormat_Specified()
//        {
//            var poco = new Person
//            {
//                Name = "Foo",
//                Age = 50,
//                Price = 19.95m,
//                StartDate = new DateTime(2009, 12, 18, 10, 2, 23),
//                IsCool = false
//            };

//            var xml = new XmlSerializer();
//            xml.DateFormat = DateFormat.Iso8601;
//            var doc = xml.Serialize(poco);
//            var expected = GetSimplePocoXDocWithXmlProperty();
//            Console.WriteLine(doc);
//            Assert.AreEqual(expected.ToString(), doc);
//        }

//        [Test]
//        public void Can_serialize_simple_POCO_With_Different_Root_Element()
//        {
//            var poco = new Person
//            {
//                Name = "Foo",
//                Age = 50,
//                Price = 19.95m,
//                StartDate = new DateTime(2009, 12, 18, 10, 2, 23)
//            };

//            var xml = new XmlSerializer();
//            xml.RootElement = "Result";
//            var doc = xml.Serialize(poco);
//            var expected = GetSimplePocoXDocWithRoot();
//            Console.WriteLine(doc);
//            Assert.AreEqual(expected.ToString(), doc.ToString());
//        }

//        //[Test]
//        //public void Can_serialize_simple_POCO_With_Attribute_Options_Defined()
//        //{
//        //    var poco = new WackyPerson
//        //    {
//        //        Name = "Foo",
//        //        Age = 50,
//        //        Price = 19.95m,
//        //        StartDate = new DateTime(2009, 12, 18, 10, 2, 23)
//        //    };

//        //    var xml = new XmlSerializer();
//        //    var doc = xml.Serialize(poco);
//        //    var expected = GetSimplePocoXDocWackyNames();

//        //    Assert.AreEqual(expected.ToString(), doc.ToString());
//        //}

//        [Test]
//        public void Can_serialize_a_list_which_is_the_root_element()
//        {
//            var pocoList = new PersonList
//            {
//                new Person
//                {
//                    Name = "Foo",
//                    Age = 50,
//                    Price = 19.95m,
//                    StartDate = new DateTime(2009, 12, 18, 10, 2, 23),
//                    Items = new List<Item>
//                    {
//                        new Item {Name = "One", Value = 1},
//                        new Item {Name = "Two", Value = 2},
//                        new Item {Name = "Three", Value = 3}
//                    }
//                },
//                new Person
//                {
//                    Name = "Bar",
//                    Age = 23,
//                    Price = 23.23m,
//                    StartDate = new DateTime(2009, 12, 23, 10, 23, 23),
//                    Items = new List<Item>
//                    {
//                        new Item {Name = "One", Value = 1},
//                        new Item {Name = "Two", Value = 2},
//                        new Item {Name = "Three", Value = 3}
//                    }
//                }
//            };

//            var xml = new XmlSerializer();
//            var doc = xml.Serialize(pocoList);
//            var expected = GetPeopleXDoc(CultureInfo.InvariantCulture);
//            Console.WriteLine(doc);
//           // Assert.AreEqual(expected.ToString(), doc);
//        }


//        private XDocument GetSimplePocoXDoc()
//        {
//            var doc = new XDocument();
//            var root = new XElement("Person");
//            root.Add(new XElement("Name", "Foo"),
//                new XElement("Age", 50),
//                new XElement("Price", 19.95m),
//                new XElement("StartDate", new DateTime(2009, 12, 18, 10, 2, 23).ToString()));

//            var items = new XElement("Items");
//            items.Add(new XElement("Item", new XElement("Name", "One"), new XElement("Value", 1)));
//            items.Add(new XElement("Item", new XElement("Name", "Two"), new XElement("Value", 2)));
//            items.Add(new XElement("Item", new XElement("Name", "Three"), new XElement("Value", 3)));
//            root.Add(items);

//            doc.Add(root);

//            return doc;
//        }

//        private XDocument GetSimplePocoXDocWithIsoDate()
//        {
//            var doc = new XDocument();
//            var root = new XElement("Person");
//            root.Add(new XElement("Name", "Foo"),
//                    new XElement("Age", 50),
//                    new XElement("Price", 19.95m),
//                    new XElement("StartDate", new DateTime(2009, 12, 18, 10, 2, 23).ToString("s")));

//            doc.Add(root);

//            return doc;
//        }

//        private XDocument GetSimplePocoXDocWithXmlProperty()
//        {
//            var doc = new XDocument();
//            var root = new XElement("Person");
//            root.Add(new XElement("Name", "Foo"),
//                new XElement("Age", 50),
//                new XElement("Price", 19.95m),
//                new XElement("StartDate", new DateTime(2009, 12, 18, 10, 2, 23).ToString("s")),
//                new XElement("IsCool", false));

//            doc.Add(root);

//            return doc;
//        }

//        private XDocument GetSimplePocoXDocWithRoot()
//        {
//            var doc = new XDocument();
//            var root = new XElement("Result");

//            var start = new XElement("Person");
//            start.Add(new XElement("Name", "Foo"),
//                    new XElement("Age", 50),
//                    new XElement("Price", 19.95m),
//                    new XElement("StartDate", new DateTime(2009, 12, 18, 10, 2, 23).ToString()));

//            root.Add(start);
//            doc.Add(root);

//            return doc;
//        }

//        private XDocument GetSimplePocoXDocWackyNames()
//        {
//            var doc = new XDocument();
//            var root = new XElement("Person");
//            root.Add(new XAttribute("WackyName", "Foo"),
//                    new XElement("Age", 50),
//                    new XAttribute("Price", 19.95m),
//                    new XAttribute("start_date", new DateTime(2009, 12, 18, 10, 2, 23).ToString()));

//            doc.Add(root);

//            return doc;
//        }

//        private XDocument GetSortedPropsXDoc()
//        {
//            var doc = new XDocument();
//            var root = new XElement("OrderedProperties");

//            root.Add(new XElement("StartDate", new DateTime(2010, 1, 1).ToString()));
//            root.Add(new XElement("Name", "Name"));
//            root.Add(new XElement("Age", 99));

//            doc.Add(root);

//            return doc;
//        }

//        private XDocument GetPeopleXDoc(CultureInfo culture)
//        {
//            var doc = new XDocument();
//            var root = new XElement("People");
//            var element = new XElement("Person");

//            var items = new XElement("Items");
//            items.Add(new XElement("Item", new XElement("Name", "One"), new XElement("Value", 1)));
//            items.Add(new XElement("Item", new XElement("Name", "Two"), new XElement("Value", 2)));
//            items.Add(new XElement("Item", new XElement("Name", "Three"), new XElement("Value", 3)));
//            element.Add(new XElement("Name", "Foo"),
//                new XElement("Age", 50),
//                new XElement("Price", 19.95m.ToString(culture)),
//                new XElement("StartDate", new DateTime(2009, 12, 18, 10, 2, 23).ToString(culture)));

//            element.Add(items);
//            root.Add(element);
//            element = new XElement("Person");

//            element.Add(new XElement("Name", "Bar"),
//                new XElement("Age", 23),
//                new XElement("Price", 23.23m.ToString(culture)),
//                new XElement("StartDate", new DateTime(2009, 12, 23, 10, 23, 23).ToString(culture)));

//            element.Add(items);

//            root.Add(element);
//            doc.Add(root);

//            return doc;
//        }
//    }
//}
