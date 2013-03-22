using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace NLite.Test.Data
{
    [TestFixture]
    public class PaginationTest
    {
        class User
        {
            public int Id;
            public string Name;

            public override string ToString()
            {
                return Id.ToString();
            }
        }

        [Test]
        public void Test2()
        {
            var excepted = 41;
            var items = Enumerable
                .Range(1, excepted)
                .Select(p => new User { Id = p, Name = "Name" + p })
                .AsQueryable();

            var pageSize = 10;
            var pageIndex = 0;
            var pagination = new NLite.Data.Pagination<User>(items, pageIndex, pageSize);
            Assert.AreEqual(pageIndex, pagination.PageIndex);
            Assert.AreEqual(pageSize, pagination.PageSize);
            Assert.AreEqual(pageIndex + 1, pagination.PageNumber);
            Assert.AreEqual(excepted, pagination.TotalRowCount);
            Assert.IsTrue(pagination.HasPagination);
            Assert.IsTrue(pagination.PageCount == 5);
            Assert.IsFalse(pagination.HasFirst);
            Assert.IsTrue(pagination.HasLast);
            Assert.IsFalse(pagination.HasPrevious);
            Assert.IsTrue(pagination.HasNext);
            Assert.AreEqual(pageSize, pagination.Length);

            pageIndex = 1;
            pagination = new NLite.Data.Pagination<User>(items, pageIndex, pageSize);
            Assert.AreEqual(pageIndex, pagination.PageIndex);
            Assert.AreEqual(pageSize, pagination.PageSize);
            Assert.AreEqual(pageIndex + 1, pagination.PageNumber);
            Assert.AreEqual(excepted, pagination.TotalRowCount);
            Assert.IsTrue(pagination.HasPagination);
            Assert.IsTrue(pagination.PageCount == 5);
            Assert.IsTrue(pagination.HasFirst);
            Assert.IsTrue(pagination.HasLast);
            Assert.IsFalse(pagination.HasPrevious);
            Assert.IsTrue(pagination.HasNext);
            Assert.AreEqual(pageSize, pagination.Length);

            pageIndex = 2;
            pagination = new NLite.Data.Pagination<User>(items, pageIndex, pageSize);
            Assert.AreEqual(pageIndex, pagination.PageIndex);
            Assert.AreEqual(pageSize, pagination.PageSize);
            Assert.AreEqual(pageIndex + 1, pagination.PageNumber);
            Assert.AreEqual(excepted, pagination.TotalRowCount);
            Assert.IsTrue(pagination.HasPagination);
            Assert.IsTrue(pagination.PageCount == 5);
            Assert.IsTrue(pagination.HasFirst);
            Assert.IsTrue(pagination.HasLast);
            Assert.IsTrue(pagination.HasPrevious);
            Assert.IsTrue(pagination.HasNext);
            Assert.AreEqual(pageSize, pagination.Length);

            pageIndex = 3;
            pagination = new NLite.Data.Pagination<User>(items, pageIndex, pageSize);
            Assert.AreEqual(pageIndex, pagination.PageIndex);
            Assert.AreEqual(pageSize, pagination.PageSize);
            Assert.AreEqual(pageIndex + 1, pagination.PageNumber);
            Assert.AreEqual(excepted, pagination.TotalRowCount);
            Assert.IsTrue(pagination.HasPagination);
            Assert.IsTrue(pagination.PageCount == 5);
            Assert.IsTrue(pagination.HasFirst);
            Assert.IsTrue(pagination.HasLast);
            Assert.IsTrue(pagination.HasPrevious);
            Assert.IsFalse(pagination.HasNext);
            Assert.AreEqual(pageSize, pagination.Length);

            pageIndex = 4;
            pagination = new NLite.Data.Pagination<User>(items, pageIndex, pageSize);
            Assert.AreEqual(pageIndex, pagination.PageIndex);
            Assert.AreEqual(pageSize, pagination.PageSize);
            Assert.AreEqual(pageIndex + 1, pagination.PageNumber);
            Assert.AreEqual(excepted, pagination.TotalRowCount);
            Assert.IsTrue(pagination.HasPagination);
            Assert.IsTrue(pagination.PageCount == 5);
            Assert.IsTrue(pagination.HasFirst);
            Assert.IsFalse(pagination.HasLast);
            Assert.IsTrue(pagination.HasPrevious);
            Assert.IsFalse(pagination.HasNext);
            Assert.AreEqual(1, pagination.Length);

        }



        [Test]
        public void Test()
        {
            var items = Enumerable
                .Range(1, 10)
                .Select(p=>new User{ Id = p, Name = "Name"+ p})
                .AsQueryable();

            var pageSize = 3;
            var pageIndex = 0;
            var pagination = new NLite.Data.Pagination<User>(items, pageIndex, pageSize);
            Assert.AreEqual(pageIndex, pagination.PageIndex);
            Assert.AreEqual(pageSize, pagination.PageSize);
            Assert.AreEqual(pageIndex+1, pagination.PageNumber);
            Assert.AreEqual(10, pagination.TotalRowCount);
            Assert.IsTrue(pagination.HasPagination);
            Assert.IsTrue(pagination.PageCount == 4);
            Assert.IsFalse(pagination.HasFirst);
            Assert.IsTrue(pagination.HasLast);
            Assert.IsFalse(pagination.HasPrevious);
            Assert.IsTrue(pagination.HasNext);
            Assert.AreEqual(pageSize, pagination.Length);


            pageIndex = 1;
            pagination = new NLite.Data.Pagination<User>(items.AsQueryable(), pageIndex, pageSize);
            Assert.AreEqual(pageIndex, pagination.PageIndex);
            Assert.AreEqual(pageSize, pagination.PageSize);
            Assert.AreEqual(pageIndex + 1, pagination.PageNumber);
            Assert.AreEqual(10, pagination.TotalRowCount);
            Assert.IsTrue(pagination.HasPagination);
            Assert.IsTrue(pagination.PageCount == 4);
            Assert.IsTrue(pagination.HasFirst);
            Assert.IsTrue(pagination.HasLast);
            Assert.IsFalse(pagination.HasPrevious);
            Assert.IsTrue(pagination.HasNext);
            Assert.AreEqual(pageSize, pagination.Length);

            pageIndex = 2;
            pagination = new NLite.Data.Pagination<User>(items, pageIndex, pageSize);
            Assert.AreEqual(pageIndex, pagination.PageIndex);
            Assert.AreEqual(pageSize, pagination.PageSize);
            Assert.AreEqual(pageIndex + 1, pagination.PageNumber);
            Assert.AreEqual(10, pagination.TotalRowCount);
            Assert.IsTrue(pagination.HasPagination);
            Assert.IsTrue(pagination.PageCount == 4);
            Assert.IsTrue(pagination.HasFirst);
            Assert.IsTrue(pagination.HasLast);
            Assert.IsTrue(pagination.HasPrevious);
            Assert.IsFalse(pagination.HasNext);
            Assert.AreEqual(pageSize, pagination.Length);

            pageIndex = 3;
            pagination = new NLite.Data.Pagination<User>(items, pageIndex, pageSize);
            Assert.AreEqual(pageIndex, pagination.PageIndex);
            Assert.AreEqual(pageSize, pagination.PageSize);
            Assert.AreEqual(pageIndex + 1, pagination.PageNumber);
            Assert.AreEqual(10, pagination.TotalRowCount);
            Assert.IsTrue(pagination.HasPagination);
            Assert.IsTrue(pagination.PageCount == 4);
            Assert.IsTrue(pagination.HasFirst);
            Assert.IsFalse(pagination.HasLast);
            Assert.IsTrue(pagination.HasPrevious);
            Assert.IsFalse(pagination.HasNext);
            Assert.AreEqual(1, pagination.Length);


            pageSize = 10;
            pageIndex = 0;
            pagination = new NLite.Data.Pagination<User>(items, pageIndex, pageSize);
            Assert.AreEqual(pageIndex, pagination.PageIndex);
            Assert.AreEqual(pageSize, pagination.PageSize);
            Assert.AreEqual(pageIndex + 1, pagination.PageNumber);
            Assert.AreEqual(10, pagination.TotalRowCount);
            Assert.IsFalse(pagination.HasPagination);
            Assert.IsTrue(pagination.PageCount == 1);
            Assert.IsFalse(pagination.HasFirst);
            Assert.IsFalse(pagination.HasLast);
            Assert.IsFalse(pagination.HasPrevious);
            Assert.IsFalse(pagination.HasNext);
            Assert.AreEqual(pageSize, pagination.Length);

            pageSize = 10;
            pageIndex = 1;
            pagination = new NLite.Data.Pagination<User>(items, pageIndex, pageSize);
            Assert.AreEqual(pageIndex, pagination.PageIndex);
            Assert.AreEqual(pageSize, pagination.PageSize);
            Assert.AreEqual(pageIndex + 1, pagination.PageNumber);
            Assert.AreEqual(10, pagination.TotalRowCount);
            Assert.IsFalse(pagination.HasPagination);
            Assert.IsTrue(pagination.PageCount == 1);
            Assert.IsFalse(pagination.HasFirst);
            Assert.IsFalse(pagination.HasLast);
            Assert.IsFalse(pagination.HasPrevious);
            Assert.IsFalse(pagination.HasNext);
            Assert.AreEqual(0, pagination.Length);


            pageSize = 8;
            pageIndex = 0;
            pagination = new NLite.Data.Pagination<User>(items, pageIndex, pageSize);
            Assert.AreEqual(pageIndex, pagination.PageIndex);
            Assert.AreEqual(pageSize, pagination.PageSize);
            Assert.AreEqual(pageIndex + 1, pagination.PageNumber);
            Assert.AreEqual(10, pagination.TotalRowCount);
            Assert.IsTrue(pagination.HasPagination);
            Assert.IsTrue(pagination.PageCount == 2);
            Assert.IsFalse(pagination.HasFirst);
            Assert.IsTrue(pagination.HasLast);
            Assert.IsFalse(pagination.HasPrevious);
            Assert.IsFalse(pagination.HasNext);
            Assert.AreEqual(8, pagination.Length);

            pageSize = 8;
            pageIndex = 1;
            pagination = new NLite.Data.Pagination<User>(items, pageIndex, pageSize);
            Assert.AreEqual(pageIndex, pagination.PageIndex);
            Assert.AreEqual(pageSize, pagination.PageSize);
            Assert.AreEqual(pageIndex + 1, pagination.PageNumber);
            Assert.AreEqual(10, pagination.TotalRowCount);
            Assert.IsTrue(pagination.HasPagination);
            Assert.IsTrue(pagination.PageCount == 2);
            Assert.IsTrue(pagination.HasFirst);
            Assert.IsFalse(pagination.HasLast);
            Assert.IsFalse(pagination.HasPrevious);
            Assert.IsFalse(pagination.HasNext);
            Assert.AreEqual(2, pagination.Length);

            pageSize = 8;
            pageIndex = 2;
            pagination = new NLite.Data.Pagination<User>(items, pageIndex, pageSize);
            Assert.AreEqual(pageIndex, pagination.PageIndex);
            Assert.AreEqual(pageSize, pagination.PageSize);
            Assert.AreEqual(pageIndex + 1, pagination.PageNumber);
            Assert.AreEqual(10, pagination.TotalRowCount);
            Assert.IsTrue(pagination.HasPagination);
            Assert.IsTrue(pagination.PageCount == 2);
            Assert.IsTrue(pagination.HasFirst);
            Assert.IsTrue(pagination.HasLast);
            Assert.IsFalse(pagination.HasPrevious);
            Assert.IsFalse(pagination.HasNext);
            Assert.AreEqual(0, pagination.Length);

            items = Enumerable
                .Range(1, 20)
                .Select(p => new User { Id = p, Name = "Name" + p })
                .AsQueryable();

            pageSize = 5;
            pageIndex = 0;
            pagination = new NLite.Data.Pagination<User>(items, pageIndex, pageSize);
            Assert.AreEqual(pageIndex, pagination.PageIndex);
            Assert.AreEqual(pageSize, pagination.PageSize);
            Assert.AreEqual(pageIndex + 1, pagination.PageNumber);
            Assert.AreEqual(20, pagination.TotalRowCount);
            Assert.IsTrue(pagination.HasPagination);
            Assert.IsTrue(pagination.PageCount == 4);
            Assert.IsFalse(pagination.HasFirst);
            Assert.IsTrue(pagination.HasLast);
            Assert.IsFalse(pagination.HasPrevious);
            Assert.IsTrue(pagination.HasNext);
            Assert.AreEqual(5, pagination.Length);

            pageIndex = 1;
            pagination = new NLite.Data.Pagination<User>(items, pageIndex, pageSize);
            Assert.AreEqual(pageIndex, pagination.PageIndex);
            Assert.AreEqual(pageSize, pagination.PageSize);
            Assert.AreEqual(pageIndex + 1, pagination.PageNumber);
            Assert.AreEqual(20, pagination.TotalRowCount);
            Assert.IsTrue(pagination.HasPagination);
            Assert.IsTrue(pagination.PageCount == 4);
            Assert.IsTrue(pagination.HasFirst);
            Assert.IsTrue(pagination.HasLast);
            Assert.IsFalse(pagination.HasPrevious);
            Assert.IsTrue(pagination.HasNext);
            Assert.AreEqual(5, pagination.Length);

            pageIndex = 2;
            pagination = new NLite.Data.Pagination<User>(items, pageIndex, pageSize);
            Assert.AreEqual(pageIndex, pagination.PageIndex);
            Assert.AreEqual(pageSize, pagination.PageSize);
            Assert.AreEqual(pageIndex + 1, pagination.PageNumber);
            Assert.AreEqual(20, pagination.TotalRowCount);
            Assert.IsTrue(pagination.HasPagination);
            Assert.IsTrue(pagination.PageCount == 4);
            Assert.True(pagination.HasFirst);
            Assert.True(pagination.HasLast);
            Assert.IsTrue(pagination.HasPrevious);
            Assert.IsFalse(pagination.HasNext);
            Assert.AreEqual(5, pagination.Length);

            pageIndex = 3;
            pagination = new NLite.Data.Pagination<User>(items, pageIndex, pageSize);
            Assert.AreEqual(pageIndex, pagination.PageIndex);
            Assert.AreEqual(pageSize, pagination.PageSize);
            Assert.AreEqual(pageIndex + 1, pagination.PageNumber);
            Assert.AreEqual(20, pagination.TotalRowCount);
            Assert.IsTrue(pagination.HasPagination);
            Assert.IsTrue(pagination.PageCount == 4);
            Assert.IsTrue(pagination.HasFirst);
            Assert.IsFalse(pagination.HasLast);
            Assert.IsTrue(pagination.HasPrevious);
            Assert.IsFalse(pagination.HasNext);
            Assert.AreEqual(5, pagination.Length);
        }
    }
}
