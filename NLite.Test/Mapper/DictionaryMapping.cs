using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Collections;
using System.Collections.Specialized;

namespace NLite.Mapping.Test
{
    [TestFixture]
    public class DictionaryMapping
    {
        public class SourceValue
        {
            public int Value { get; set; }
        }

        public class DestValue
        {
            public int Value { get; set; }
        }

        

        [Test]
        public void Example()
        {
            var sourceDict = new Dictionary<string, SourceValue>
				{
					{"First", new SourceValue {Value = 5}},
					{"Second", new SourceValue {Value = 10}},
					{"Third", new SourceValue {Value = 15}}
				};

            var destDict = Mapper.Map<Dictionary<string, SourceValue>, IDictionary<string, DestValue>>(sourceDict);
            Assert.AreEqual(3, destDict.Count);
            Assert.AreEqual(destDict["First"].Value, 5);
            Assert.AreEqual(destDict["Second"].Value, 10);
            Assert.AreEqual(destDict["Third"].Value, 15);
            
        }

        [Test]
        public void Example2()
        {
            var sourceDict = new Dictionary<int, SourceValue>
				{
					{1, new SourceValue {Value = 5}},
					{2, new SourceValue {Value = 10}},
					{3, new SourceValue {Value = 15}}
				};

            var destDict = Mapper.Map<Dictionary<int, SourceValue>, IDictionary<string, DestValue>>(sourceDict);
            Assert.AreEqual(3, destDict.Count);
            Assert.AreEqual(destDict["1"].Value, 5);
            Assert.AreEqual(destDict["2"].Value, 10);
            Assert.AreEqual(destDict["3"].Value, 15);

        }

        enum sourceKey
        {
            First,
            Second,
            Third,
        }

        enum destKey
        {
            First,
            Second,
            Third,
        }

        public void Example3()
        {
            var sourceDict = new Dictionary<sourceKey, SourceValue>
				{
					{sourceKey.First, new SourceValue {Value = 5}},
					{sourceKey.Second, new SourceValue {Value = 10}},
					{sourceKey.Third, new SourceValue {Value = 15}}
				};

            var destDict = Mapper.Map<Dictionary<sourceKey, SourceValue>, IDictionary<destKey, DestValue>>(sourceDict);
            Assert.AreEqual(3, destDict.Count);
            Assert.AreEqual(destDict[destKey.First].Value, 5);
            Assert.AreEqual(destDict[destKey.Second].Value, 10);
            Assert.AreEqual(destDict[destKey.Third].Value, 15);

        }

        public void Example4()
        {
            var sourceDict = new Hashtable()
				{
					{1, new SourceValue {Value = 5}},
					{2, new SourceValue {Value = 10}},
					{3, new SourceValue {Value = 15}}
				};

            var destDict = Mapper.Map<Hashtable, IDictionary<string, DestValue>>(sourceDict);
            Assert.AreEqual(3, destDict.Count);
            Assert.AreEqual(destDict["1"].Value, 5);
            Assert.AreEqual(destDict["2"].Value, 10);
            Assert.AreEqual(destDict["3"].Value, 15);

        }

        public void Example5()
        {
            var sourceDict = new Dictionary<int, SourceValue>()
				{
					{1, new SourceValue {Value = 5}},
					{2, new SourceValue {Value = 10}},
					{3, new SourceValue {Value = 15}}
				};

            var destDict = Mapper.Map<Dictionary<int, SourceValue>, Hashtable>(sourceDict);
            Assert.AreEqual(3, destDict.Count);
            Assert.AreEqual((destDict[1] as SourceValue).Value, 5);
            Assert.AreEqual((destDict[2] as SourceValue).Value, 10);
            Assert.AreEqual((destDict[3] as SourceValue).Value, 15);

        }


        public class Book
        {
            public int BookId { get; set; }
            public string BookName { get; set; }
            public Author Author { get; set; }
            public DateTime PublishedDate { get; set; }
        }

        public class Author
        {
            public int AuthorId { get; set; }
            public string AuthorName { get; set; }
            public string Nation { get; set; }
        }

        [Test]
        public void TestSubProperty()
        {
            var sourceDict = new Dictionary<string, string>
				{
					{"BookName", "设计模式"},
					{"Author.AuthorName", "四人帮"},
					{"Author.AuthorId", "3"}
				};

            var destDict = Mapper.Map<Dictionary<string, string>, Book>(sourceDict);

            Assert.NotNull(destDict);
            Assert.AreEqual("设计模式", destDict.BookName);
            Assert.AreEqual("四人帮", destDict.Author.AuthorName);
            Assert.AreEqual(3, destDict.Author.AuthorId);
        }

        [Test]
        public void ClassToDictionary()
        {
            var book = new 
            {
                BookName = "设计模式",
            };

            var dict = Mapper.Map(book, book.GetType(), typeof(IDictionary<string, object>)) as IDictionary<string, object>;
            Assert.AreEqual(dict["BookName"], book.BookName);

            var dict2 = Mapper.Map(book, book.GetType(), typeof(IDictionary<string, string>)) as IDictionary<string, string>;
            Assert.AreEqual(dict2["BookName"], book.BookName);


            var ht = Mapper.Map(book, book.GetType(), typeof(Hashtable)) as Hashtable;
            Assert.AreEqual(ht["BookName"], book.BookName);

            var nvc = Mapper.Map(book, book.GetType(), typeof(NameValueCollection)) as NameValueCollection;
            Assert.AreEqual(nvc["BookName"], book.BookName);

            var sd = Mapper.Map(book, book.GetType(), typeof(StringDictionary)) as StringDictionary;
            Assert.AreEqual(sd["BookName"], book.BookName);
        }

        [Test]
        public void ClassToDictionary2()
        {
            var book = new
            {
                BookName = "设计模式",
            };

            var dict = new Dictionary<string, object>();
            Mapper.Map(book,ref dict) ;
            Assert.AreEqual(dict["BookName"], book.BookName);

            var dict2 = new Dictionary<string, string>();
            Mapper.Map(book, ref dict2) ;
            Assert.AreEqual(dict2["BookName"], book.BookName);


            var ht = new Hashtable();
            Mapper.Map(book, ref ht);
            Assert.AreEqual(ht["BookName"], book.BookName);

            var nvc = new NameValueCollection();
            Mapper.Map(book, ref nvc);
            Assert.AreEqual(nvc["BookName"], book.BookName);

            var sd = new StringDictionary();
            Mapper.Map(book, ref sd);
            Assert.AreEqual(sd["BookName"], book.BookName);
        }
      
    }
}
