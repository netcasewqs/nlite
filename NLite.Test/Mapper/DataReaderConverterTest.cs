using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Data;
using NLite.Data;
using System.Data.Common;

namespace NLite.Mapping.Test
{
    [TestFixture]
    public class DataReaderConverterTest
    {
        public enum TestClassStatus
        {
            First = 0,
            Second = 1
        }

        class TestClass
        {
            public int Id { get; set; }
            public string Name;

            public TestClassStatus? Status;
            public Guid Guid;
        }

       

        [Test]
        public void Test()
        {
            var dt = new DataTable();
            dt.Columns.Add(new DataColumn("Id", typeof(long)));
            dt.Columns.Add(new DataColumn("Name", typeof(string)));
            dt.Columns[1].AllowDBNull = true;
            dt.Columns.Add(new DataColumn("Status", typeof(long)));
            dt.Columns.Add(new DataColumn("Guid", typeof(Guid)));

            int count = 5;
            for (int i = 0; i < count; ++i)
            {
                if(i%2 == 0)
                    dt.Rows.Add(i, Guid.NewGuid().ToString(), DBNull.Value, Guid.NewGuid());
                else
                    dt.Rows.Add(i, DBNull.Value, 1, Guid.NewGuid());
            }

            dt.AcceptChanges();

            IDataReader reader = null;
#if !SDK35
            reader = dt.CreateDataReader();
            var rows = reader.ToList<dynamic>().ToArray();
            Assert.AreEqual(count, rows.Length);
#else
            reader = dt.CreateDataReader();
            var rows = reader.ToList<IDictionary<string,object>>().ToArray();
            Assert.AreEqual(count, rows.Length);
#endif
            reader = dt.CreateDataReader();
            var items = reader.ToList<TestClass>().ToArray();

            Assert.AreEqual(count, items.Length);

            reader = dt.CreateDataReader();
            items = reader.ToList<TestClass>().ToArray();
            Assert.AreEqual(count, items.Length);

            reader = dt.CreateDataReader();
            items = Mapper.Map<IDataReader, List<TestClass>>(reader).ToArray();
            Assert.AreEqual(count, items.Length);

            reader = dt.CreateDataReader();
            items = Mapper.Map<IDataReader, TestClass[]>(reader);
            Assert.AreEqual(count, items.Length);

            foreach(DataRow row in dt.Rows)
            {
	            	var o1 = row[0];
	            	var o2 = row[1];
	            	var o3 = row[2];
	            	var o4 = row[3];
            	  
	            	var i1 = Mapper.Map<object,int>(o1);
	            	var i2 = Mapper.Map<object,string>(o2);
	            	var i3 = Mapper.Map<object,int>(o3);
	            	var i4 = Mapper.Map<object,Guid>(o4);
            }
        }

        [Test]
        public void DataTablePagination()
        {
            var dt = new DataTable();
            dt.Columns.Add(new DataColumn("Id", typeof(long)));
            dt.Columns.Add(new DataColumn("Name", typeof(string)));
            dt.Columns[1].AllowDBNull = true;
            dt.Columns.Add(new DataColumn("Status", typeof(long)));
            dt.Columns.Add(new DataColumn("Guid", typeof(Guid)));

            int count = 10;
            for (int i = 0; i < count; ++i)
            {
                if (i % 2 == 0)
                    dt.Rows.Add(i, Guid.NewGuid().ToString(), DBNull.Value, Guid.NewGuid());
                else
                    dt.Rows.Add(i, DBNull.Value, 1, Guid.NewGuid());
            }

            var pagination = dt.ToPaination(0, 10, 50);
            foreach (DataRowView row in pagination)
            {
                Console.WriteLine(row[0]);
                Assert.IsNotNull(row);
            }

        }

        [Test]
        public void DataReaderPagination()
        {
            var dt = new DataTable();
            dt.TableName = "Table1";
            dt.Columns.Add(new DataColumn("Id", typeof(long)));
            dt.Columns.Add(new DataColumn("Name", typeof(string)));
            dt.Columns[1].AllowDBNull = true;
            dt.Columns.Add(new DataColumn("Status", typeof(long)));
            dt.Columns.Add(new DataColumn("Guid", typeof(Guid)));

            int count = 10;
            for (int i = 0; i < count; ++i)
            {
                if (i % 2 == 0)
                    dt.Rows.Add(i, Guid.NewGuid().ToString(), DBNull.Value, Guid.NewGuid());
                else
                    dt.Rows.Add(i, DBNull.Value, 1, Guid.NewGuid());
            }

            dt.AcceptChanges();
            var reader = dt.CreateDataReader();
            foreach (DbDataRecord row in reader)
            {
                Assert.IsNotNull(row);
                Console.WriteLine(row[0]);

            }

            //var pagination = dt.CreateDataReader().ToPaination(0, 10, 50);
            //foreach (IDataRecord row in pagination)
            //{
            //    Assert.IsNotNull(row);
            //    Console.WriteLine(row[0]);
            //}
        }


    }
}
