using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace NLite.Domain.Spec.DataBindings
{
    public class ComplexDictionaryParameterActionSpec:TestBase
    {
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

        class BookService
        {
            public IDictionary<string, Book> Save(IDictionary<string, Book> books)
            {
                return books;
            }
        }

        [Test]
        public void Test()
        {
            var dict = new Dictionary<string, object>();
            dict["books[0].key"] = "key0";
            dict["books[1].key"] = "key1";

            dict["books[0].value.BookId"] = "0";
            dict["books[1].value.BookId"] = "1";
            dict["books[0].value.BookName"] = "BookName_00";
            dict["books[1].value.BookName"] = "BookName_11";
            dict["books[0].value.Author.AuthorId"] = "0";
            dict["books[1].value.Author.AuthorId"] = "1";
            dict["books[0].value.Author.AuthorName"] = "AuthorName0";
            dict["books[1].value.Author.AuthorName"] = "AuthorName1";
            dict["books[0].value.Author.Nation"] = "Nation0";
            dict["books[1].value.Author.Nation"] = "Nation1";

            ServiceRegistry.Register<BookService>();

            var result = ServiceDispatcher
                .Dispatch<IDictionary<string, Book>>("book", "save", dict);

            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.ContainsKey("key0"));
            Assert.IsTrue(result.ContainsKey("key1"));

            Assert.AreEqual(0, result["key0"].BookId);
            Assert.AreEqual(1, result["key1"].BookId);

            Assert.AreEqual("BookName_00", result["key0"].BookName);
            Assert.AreEqual("BookName_11", result["key1"].BookName);

            Assert.AreEqual(0, result["key0"].Author.AuthorId);
            Assert.AreEqual(1, result["key1"].Author.AuthorId);

            Assert.AreEqual("AuthorName0", result["key0"].Author.AuthorName);
            Assert.AreEqual("AuthorName1", result["key1"].Author.AuthorName);

            Assert.AreEqual("Nation0", result["key0"].Author.Nation);
            Assert.AreEqual("Nation1", result["key1"].Author.Nation);
           
        }
    }
}
