using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Diagnostics;
using NLite.Reflection;
using System.Data.Common;
using System.Data;
using NLite.Data;

namespace NLite.Test
{
    [TestFixture]
    public class UtilTest
    {
        [Test]//
        public void DateFormateTest()
        {
            Console.WriteLine(DateTime.Now.ToString("MM-dd-HH-mm-yyyy.ss"));

            var dictType = typeof(Dictionary<string,string>);
            var iDictType = typeof(IDictionary<,>);

            var type = dictType.GetGenericTypeDefinition();
            var type2 = iDictType.GetGenericTypeDefinition();
            Console.WriteLine(type);
            Console.WriteLine(type2);


        }

        [Test]
        public void TestClassAndStruct()
        {
            List<ClassObject> classObjs = new List<ClassObject>(20);
            List<StructObject> structObjs = new List<StructObject>(20);
            List<StructObject> structObjs2 = new List<StructObject>(20);

            var count = 3000000;

            var sw = Stopwatch.StartNew();
            for (int i = 0; i < count; i++)
            {
                foreach (var item in structObjs)
                {
                    LoadStructObject(item);
                }
            }
            sw.Stop();
            Console.WriteLine("struct elapsed:{0}ms", sw.ElapsedMilliseconds);


            sw = Stopwatch.StartNew();
            for (int i = 0; i < count; i++)
            {
                foreach (var item in structObjs2)
                {
                    var it = item;
                    LoadStructObjectByRef(ref it);
                }
            }
            sw.Stop();
            Console.WriteLine("ref struct elapsed:{0}ms", sw.ElapsedMilliseconds);


            sw = Stopwatch.StartNew();
            for (int i = 0; i < count; i++)
            {
                foreach (var item in classObjs)
                {
                    LoadClassObject(item);
                }
            }
            sw.Stop();
            Console.WriteLine("class elapsed:{0}ms", sw.ElapsedMilliseconds);

        }

        class ClassObject
        {
            public int Id;
            public string Name;
            public bool Sex;
            public int Age;
            public DateTime Birthday;

            public bool Sex2;
            public int Age2;
            public DateTime Birthday2;

            public bool Sex3;
            public int Age3;
            public DateTime Birthday3;
        }

        struct StructObject
        {
            public int Id;
            public string Name;
            public bool Sex;
            public int Age;
            public DateTime Birthday;


            public bool Sex2;
            public int Age2;
            public DateTime Birthday2;

            public bool Sex3;
            public int Age3;
            public DateTime Birthday3;
        }

        void LoadClassObject(ClassObject o)
        {
            o.Name = "aaa";
            o.Id = 111;
        }

        void LoadStructObject(StructObject o)
        {
            o.Name = "aaa";
            o.Id = 111;
        }

        void LoadStructObjectByRef(ref StructObject o)
        {
            o.Name = "aaa";
            o.Id = 111;
        }

        [Test]
        public void GacTest()
        {
            foreach (var a in Gac.GetAssemblyNames().OrderBy(p => p.FullName))
                Console.WriteLine(a.FullName);
            DbProviderFactories.GetFactoryClasses().WriteXml(Console.Out);

            //var items = DbManager.DbProviderFactories;
            
        }
    }
}
