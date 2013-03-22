using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NLite.Collections;
using NLite.Test.IoC;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Reflection.Emit;
using NLite.Reflection;
using NLite.Threading;
using System.Collections;

namespace NLite.Mapping.Test
{
    [TestFixture]
    public class MappingAttributeTest
    {
        public enum SimpleEnum
        {
            First,
            Second,
        }

        [Component]
        [Metadata("String", "42")]
        [Metadata("Int", 42)]
        [Metadata("Float", 42.0f)]
        //[Mapping("Enum", null)]
        [Metadata("Type", typeof(string))]
        [Metadata("Object", 42)]
        public class SimplemappingsExporter
        {
        }

        [Component]
        [Metadata("String", null)] // null
        [Metadata("Int", 42)]
        [Metadata("Float", 42.0f)]
        [Metadata("Enum", SimpleEnum.First)]
        [Metadata("Type", typeof(string))]
        [Metadata("Object", 42)]
        public class SimplemappingsExporterWithNullReferenceValue
        {
        }

        [Component]
        [Metadata("String", "42")]
        [Metadata("Int", null)] //null
        [Metadata("Float", 42.0f)]
        [Metadata("Enum", SimpleEnum.First)]
        [Metadata("Type", typeof(string))]
        [Metadata("Object", 42)]
        public class SimplemappingsExporterWithNullNonReferenceValue
        {
        }

        [Component]
        [Metadata("String", "42")]
        [Metadata("Int", "42")] // wrong binderType
        [Metadata("Float", 42.0f)]
        [Metadata("Enum", SimpleEnum.First)]
        [Metadata("Type", typeof(string))]
        [Metadata("Object", 42)]
        public class SimplemappingsExporterWithTypeMismatch
        {
        }

        public interface ISimplemappingsView
        {

            string String { get; }
            int Int { get; }
            float Float { get; }
            SimpleEnum? Enum { get; }
            Type Type { get; }
            object Object { get; }

            //[DefaultValue(0)]
            //uint UInt32 { get; }
            //Int64 Int64 { get; }
            //UInt64 UInt64 { get; }
            //Single Single { get; }
            //Double Double { get; }
            //DateTime DateTime { get; }

            //[DefaultValue(null)]
            //object[] ObjectArray { get; }
        }

        [Test]
        public void SimplemappingsTest()
        {

            var view = MetadataMapper.Map<SimplemappingsExporter, ISimplemappingsView>();

            Assert.AreEqual("42", view.String);
            Assert.AreEqual(42, view.Int);
            Assert.AreEqual(42.0f, view.Float);
            // Assert.AreEqual(null, view.Enum);
            Assert.AreEqual(typeof(string), view.Type);
            Assert.AreEqual(42, view.Object);
        }


        [Test]
        public void SimplemappingsTestWithNullReferenceValue()
        {
            var view = MetadataMapper.Map<SimplemappingsExporterWithNullReferenceValue, ISimplemappingsView>();

            Assert.AreEqual(null, view.String);
            Assert.AreEqual(42, view.Int);
            Assert.AreEqual(42.0f, view.Float);
            Assert.AreEqual(SimpleEnum.First, view.Enum);
            Assert.AreEqual(typeof(string), view.Type);
            Assert.AreEqual(42, view.Object);
        }

        [Metadata("Data", null)]
        [Metadata("Data", false)]
        [Metadata("Data", Int16.MaxValue)]
        [Metadata("Data", Int32.MaxValue)]
        [Metadata("Data", Int64.MaxValue)]
        [Metadata("Data", UInt16.MaxValue)]
        [Metadata("Data", UInt32.MaxValue)]
        [Metadata("Data", UInt64.MaxValue)]
        [Metadata("Data", "String")]
        [Metadata("Data", typeof(ClassWithLotsOfDifferentmappingsTypes))]
        //[Mapping("Data", ConnectionState.Open)]
        [Metadata("Data", new object[] { 1, 2, null })]
        public class ClassWithLotsOfDifferentmappingsTypes
        {
        }

        [Test]
        public void ExportWithValidCollectionOfmappings_ShouldDiscoverAllmappings()
        {
            var data = MetadataMapper.Map<ClassWithLotsOfDifferentmappingsTypes>()["Data"] as object[];
            Assert.AreEqual(11, data.Length);
        }

        [Metadata("Data", null)]
        [Metadata("Data", 1)]
        [Metadata("Data", 2)]
        [Metadata("Data", 3)]
        public class ClassWithIntCollectionWithNullValue
        {
        }

        [Test]
        public void ExportWithIntCollectionPlusNullValueOfmappings_ShouldDiscoverAllmappings()
        {
            var data = MetadataMapper.Map<ClassWithIntCollectionWithNullValue>()["Data"] as object[];
            Assert.IsNotInstanceOf<int[]>(data);
            Assert.AreEqual(4, data.Length);
        }


        [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
        [MetadataAttributeAttribute]
        public class DataAttribute : Attribute
        {
            public object Object { get; set; }
        }

        [Component]
        [Data(Object = "42")]
        [Data(Object = "10")]
        public class ExportWithMultiplemappings_ExportStringsAsObjects
        {
        }

        [Component]
        [Data(Object = "42")]
        [Data(Object = "10")]
        [Data(Object = null)]
        public class ExportWithMultiplemappings_ExportStringsAsObjects_WithNull
        {
        }

        [Data(Object = 42)]
        [Data(Object = 10)]
        public class ExportWithMultiplemappings_ExportIntsAsObjects
        {
        }

        [Component]
        [Data(Object = null)]
        [Data(Object = 42)]
        [Data(Object = 10)]
        public class ExportWithMultiplemappings_ExportIntsAsObjects_WithNull
        {
        }

        public interface IObjectView_AsStrings
        {
            [DefaultValue(new string[] { "1", "2" })]
            string[] Object { get; }
        }

        public interface IObjectView_AsInts
        {
            int[] Object { get; }
        }

        public interface IObjectView
        {
            object[] Object { get; }
        }


        [Test]
        public void ExportWithMultiplemappings_ExportStringsAsObjects_ShouldDiscovermappingsAsStrings()
        {

            var view = MetadataMapper.Map<ExportWithMultiplemappings_ExportStringsAsObjects, IObjectView>();
            Assert.IsNotNull(view);

            Assert.IsNotNull(view.Object);
            Assert.AreEqual(2, view.Object.Length);
        }

        [Test]
        public void ExportWithMultiplemappings_ExportStringsAsObjects_With_Null_ShouldDiscovermappingsAsStrings()
        {

            var view = MetadataMapper.Map<ExportWithMultiplemappings_ExportStringsAsObjects_WithNull, IObjectView_AsStrings>();
            Assert.IsNotNull(view);

            Assert.IsNotNull(view.Object);
            Assert.AreEqual(3, view.Object.Length);
        }

        [Test]
        public void ExportWithMultiplemappings_ExportIntsAsObjects_ShouldDiscovermappingsAsInts()
        {
            var view = MetadataMapper.Map<ExportWithMultiplemappings_ExportIntsAsObjects, IObjectView_AsInts>();
            Assert.IsNotNull(view);


            Assert.IsNotNull(view.Object);
            Assert.AreEqual(2, view.Object.Length);
        }


        [MetadataAttributeAttribute]
        [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
        public class OrderAttribute : Attribute
        {
            public string Before { get; set; }
            public string After { get; set; }
        }

        public interface IOrdermappingsView
        {
            string[] Before { get; }
            string[] After { get; }
        }

        [Component]
        [Order(Before = "Step3")]
        [Order(Before = "Step2")]
        public class OrderedItemBeforesOnly
        {
        }

        [Test]
        public void ExportWithMultiplemappings_ExportStringsAndNulls_ThroughmappingsAttributes()
        {

            var view = MetadataMapper.Map<OrderedItemBeforesOnly, IOrdermappingsView>();
            Assert.IsNotNull(view);

            Assert.IsNotNull(view.Before);
            Assert.IsNotNull(view.After);

            Assert.AreEqual(2, view.Before.Length);
            Assert.AreEqual(2, view.After.Length);

            Assert.IsNotNull(view.Before[0]);
            Assert.IsNotNull(view.Before[1]);

            Assert.IsNull(view.After[0]);
            Assert.IsNull(view.After[1]);
        }


        [MetadataAttributeAttribute]
        [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
        public class DataTypeAttribute : Attribute
        {
            public Type Type { get; set; }
        }

        public interface ITypesmappingsView
        {
            Type[] Type { get; }
        }

        [Component]
        [DataType(Type = typeof(int))]
        [DataType(Type = typeof(string))]
        public class ItemWithTypeExports
        {
        }

        [Component]
        [DataType(Type = typeof(int))]
        [DataType(Type = typeof(string))]
        [DataType(Type = null)]
        public class ItemWithTypeExports_WithNulls
        {
        }

        [Component]
        [DataType(Type = null)]
        [DataType(Type = null)]
        [DataType(Type = null)]
        public class ItemWithTypeExports_WithAllNulls
        {
        }

        [Test]
        public void ExportWithMultiplemappings_ExportTypes()
        {
            var view = MetadataMapper.Map<ItemWithTypeExports, ITypesmappingsView>();
            Assert.IsNotNull(view);

            Assert.IsNotNull(view.Type);
            Assert.AreEqual(2, view.Type.Length);
        }

        [Test]
        public void ExportWithMultiplemappings_ExportTypes_WithNulls()
        {
            var view = MetadataMapper.Map<ItemWithTypeExports_WithNulls, ITypesmappingsView>();
            Assert.IsNotNull(view);

            Assert.IsNotNull(view.Type);
            Assert.AreEqual(3, view.Type.Length);
        }

        [Test]
        public void ExportWithMultiplemappings_ExportTypes_WithAllNulls()
        {
            var view = MetadataMapper.Map<ItemWithTypeExports_WithAllNulls, ITypesmappingsView>();
            Assert.IsNotNull(view);

            Assert.IsNotNull(view.Type);
            Assert.AreEqual(3, view.Type.Length);

            Assert.IsNull(view.Type[0]);
            Assert.IsNull(view.Type[1]);
            Assert.IsNull(view.Type[2]);
        }


    }


}
