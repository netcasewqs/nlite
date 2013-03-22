using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NLite.Globalization;
using System.Reflection;
using NLite.Reflection;
using NLite.Collections;
using System.Collections;

namespace NLite.Test
{
    [TestFixture]
    public class StringFormatterTest
    {
        [Test]
        public void Test()
        {
            var actual = StringFormatter.Format("aa");
            Assert.AreEqual("aa", actual);

            actual = StringFormatter.Format("aa ${DATE:hhmmss}");
            Console.WriteLine(actual);

            actual = StringFormatter.Format("aa ${DATE}");
            Console.WriteLine(actual);

            actual = StringFormatter.Format("aa ${TIME}");
            Console.WriteLine(actual);

            actual = StringFormatter.Format("aa ${PRODUCTNAME}");
            Console.WriteLine(actual);

            actual = StringFormatter.Format("aa ${GUID}");
            Console.WriteLine(actual);

            actual = StringFormatter.Format("aa ${SDK_PATH}BIN");
            Console.WriteLine(actual);

            actual = StringFormatter.Format("aa ${ENV:TEMP}");
            Console.WriteLine(actual);

            actual = StringFormatter.Format("aa ${Property:A}");
            Console.WriteLine(actual);

            //Register Resource
            LanguageManager.Instance.Language = "en";
            ResourceRepository.StringRegistry.Register("NLite.Test.App_LocalResources", Assembly.GetExecutingAssembly());

            actual = StringFormatter.Format("aa ${res:Fomat.Test}", "abc", "-def");
            Console.WriteLine(actual);
        }

        [Test]
        public void Test2()
        {
            var str = "books[0].Value.PublishedDate";
            var strArr = str.Split('.');
            Assert.AreEqual(3, strArr.Length);

            var arr = strArr[0].Matches("[", "]");
            foreach (var item in arr)
                Console.WriteLine(item);

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
        //[Test]
        //public void Test3()
        //{
        //    var dict = new Dictionary<string, object>();
        //    dict["books[0].BookId"] = "0";
        //    dict["books[1].BookId"] = "1";
        //    dict["books[0].BookName"] = "BookName_00";
        //    dict["books[1].BookName"] = "BookName_11";
        //    dict["books[0].Author.AuthorId"] = "0";
        //    dict["books[1].Author.AuthorId"] = "1";
        //    dict["books[0].Author.AuthorName"] = "AuthorName0";
        //    dict["books[1].Author.AuthorName"] = "AuthorName1";
        //    dict["books[0].Author.Nation"] = "Nation0";
        //    dict["books[1].Author.Nation"] = "Nation1";

        //    var result = ReflectionService.MapCollectionFromDictionary(typeof(List<Book>), "books", dict) as IList<Book>;
        //    Assert.AreEqual(2, result.Count);
        //    Assert.AreEqual(0, result[0].BookId);
        //    Assert.AreEqual(1, result[1].BookId);

        //    Assert.AreEqual("BookName_00", result[0].BookName);
        //    Assert.AreEqual("BookName_11", result[1].BookName);

        //    Assert.AreEqual(0, result[0].Author.AuthorId);
        //    Assert.AreEqual(1, result[1].Author.AuthorId);

        //    Assert.AreEqual("AuthorName0", result[0].Author.AuthorName);
        //    Assert.AreEqual("AuthorName1", result[1].Author.AuthorName);

        //    Assert.AreEqual("Nation0", result[0].Author.Nation);
        //    Assert.AreEqual("Nation1", result[1].Author.Nation);
        //}

        //[Test]
        //public void Test4()
        //{
        //    //TODO:Test value 不带点
        //    var dict = new Dictionary<string, object>();
        //    dict["books[0].key"] = "key0";
        //    dict["books[1].key"] = "key1";

        //    dict["books[0].value.BookId"] = "0";
        //    dict["books[1].value.BookId"] = "1";
        //    dict["books[0].value.BookName"] = "BookName_00";
        //    dict["books[1].value.BookName"] = "BookName_11";
        //    dict["books[0].value.Author.AuthorId"] = "0";
        //    dict["books[1].value.Author.AuthorId"] = "1";
        //    dict["books[0].value.Author.AuthorName"] = "AuthorName0";
        //    dict["books[1].value.Author.AuthorName"] = "AuthorName1";
        //    dict["books[0].value.Author.Nation"] = "Nation0";
        //    dict["books[1].value.Author.Nation"] = "Nation1";

        //    var result = ReflectionService.MapDictionaryFromDictionary(typeof(Dictionary<string,Book>), "books", dict) as IDictionary<string,Book>;
        //    Assert.AreEqual(2, result.Count);
        //    Assert.IsTrue(result.ContainsKey("key0"));
        //    Assert.IsTrue(result.ContainsKey("key1"));

        //    Assert.AreEqual(0, result["key0"].BookId);
        //    Assert.AreEqual(1, result["key1"].BookId);

        //    Assert.AreEqual("BookName_00", result["key0"].BookName);
        //    Assert.AreEqual("BookName_11", result["key1"].BookName);

        //    Assert.AreEqual(0, result["key0"].Author.AuthorId);
        //    Assert.AreEqual(1, result["key1"].Author.AuthorId);

        //    Assert.AreEqual("AuthorName0", result["key0"].Author.AuthorName);
        //    Assert.AreEqual("AuthorName1", result["key1"].Author.AuthorName);

        //    Assert.AreEqual("Nation0", result["key0"].Author.Nation);
        //    Assert.AreEqual("Nation1", result["key1"].Author.Nation);
        //}
    }
}
