using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Reflection;
using NLite.Binding;

namespace NLite.Domain.Spec
{
    [TestFixture]
    public class ListModelBinderTest
    {
        static readonly  MethodInfo BindListMethod = typeof(DefaultModelBinder).GetMethod("BindCollectionModel", BindingFlags.Static | BindingFlags.NonPublic);
        static readonly Func<Type, string, IDictionary<string, object>, object> BindList = (type, prefix, dict) => BindListMethod.Invoke(null, new object[] { type, prefix, dict });

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

            var result = BindList(typeof(List<Book>), "books", dict) as IEnumerable<Book>;
            foreach (var item in result)
            {
            }

        }
    }
}
