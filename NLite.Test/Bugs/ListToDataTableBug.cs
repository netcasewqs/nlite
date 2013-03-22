using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Data;
using NLite;

namespace NLite.Test.Mapper
{
    //Bug:http://nlite.codeplex.com/workitem/15852
    [TestFixture]
    public class ListToDataTableBug
    {
        public enum En1 : byte
        {
            a = 1,
            b = 2,
            c = 3
        }
        class self
        {
            public string aa { get; set; }
            public string bb { get; set; }
            public bool? IsNull;
            public En1 ee;
            public En1? ff;
        }

        List<self> lst = new List<self>() { 
                new self(){
                    aa = "aa1",
                    bb = "bb1",
                    IsNull = null,
                    ee =  En1.b,
                    ff = null,
                },
                new self(){
                    aa = "aa2",
                    bb = "bb2",
                    IsNull = false,
                    ee =  En1.c,
                    ff = En1.b,
                }                
            };

        [Test]
        public void Test()
        {
            var dt=NLite.Mapper.Map<List<self>, DataTable>(lst);
            Assert.IsNotNull(dt);
            Assert.AreEqual(5, dt.Columns.Count);
            Assert.IsTrue(dt.Rows[0]["IsNull"] == DBNull.Value);
            Assert.AreEqual(2, dt.Rows.Count);
            dt.WriteXml(Console.Out);

            dt = new DataTable();
            NLite.Mapper.Map<List<self>, DataTable>(lst,ref dt);
            Assert.IsNotNull(dt);
            Assert.AreEqual(5, dt.Columns.Count);
            Assert.AreEqual(2, dt.Rows.Count);

            dt.Rows.Clear();
            NLite.Mapper.Map(lst, ref dt);
            Assert.IsNotNull(dt);
            Assert.AreEqual(5, dt.Columns.Count);
            Assert.AreEqual(2, dt.Rows.Count);
        }

    }
}
