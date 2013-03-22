using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace NLite.Domain.Spec.DataBindings
{
    public class ComplexArrayParameterActionSpec:TestBase
    {
        class Book
        {
            public int BookId { get; set; }
            public string BookName { get; set; }
            public Author Author { get; set; }
            public DateTime PublishedDate { get; set; }
        }

        class Author
        {
            public int AuthorId { get; set; }
            public string AuthorName { get; set; }
            public string Nation { get; set; }
        }

        class BookService
        {
            public IEnumerable<Book> Save(List<Book> books)
            {
                return books;
            }
        }

        [Test]
        public void Test()
        {
            var dict = new Dictionary<string, object>();
            dict["books[0].BookId"] = "0";
            dict["books[1].BookId"] = "1";
            dict["books[0].BookName"] = "BookName_00";
            dict["books[1].BookName"] = "BookName_11";
            dict["books[0].Author.AuthorId"] = "0";
            dict["books[1].Author.AuthorId"] = "1";
            dict["books[0].Author.AuthorName"] = "AuthorName0";
            dict["books[1].Author.AuthorName"] = "AuthorName1";
            dict["books[0].Author.Nation"] = "Nation0";
            dict["books[1].Author.Nation"] = "Nation1";

            ServiceRegistry.Register<BookService>();

            var result = ServiceDispatcher
                .Dispatch<IEnumerable<Book>>("book", "save", dict)
                .ToArray();

            Assert.AreEqual(2, result.Length);
            Assert.AreEqual(0, result[0].BookId);
            Assert.AreEqual(1, result[1].BookId);

            Assert.AreEqual("BookName_00", result[0].BookName);
            Assert.AreEqual("BookName_11", result[1].BookName);

            Assert.AreEqual(0, result[0].Author.AuthorId);
            Assert.AreEqual(1, result[1].Author.AuthorId);

            Assert.AreEqual("AuthorName0", result[0].Author.AuthorName);
            Assert.AreEqual("AuthorName1", result[1].Author.AuthorName);

            Assert.AreEqual("Nation0", result[0].Author.Nation);
            Assert.AreEqual("Nation1", result[1].Author.Nation);
           
        }
    }
}
