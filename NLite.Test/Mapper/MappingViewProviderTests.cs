using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.ComponentModel;
using NLite.Collections;
using System.Reflection;

namespace NLite.Mapping.Test
{
    [TestFixture]
    public class MappingViewProviderTests
    {

        [Test]
        public void GetMappingView_InterfaceWithPropertySetter_ShouldThrowNotSupported()
        {
            var Mapping = new PropertySet();
            Mapping["Value"] = "value";

            Assert.Throws<NotSupportedException>(() =>
            {
                MetadataMapper.Map<IMappingViewWithPropertySetter>(Mapping);
            });
        }

        [Test]
        public void GetMappingView_InterfaceWithMethod_ShouldThrowNotSupportedException()
        {
            var Mapping = new PropertySet();
            Mapping["Value"] = "value";

            Assert.Throws<NotSupportedException>(() =>
            {
                MetadataMapper.Map<IMappingViewWithMethod>(Mapping);
            });
        }

        [Test]
        public void GetMappingView_InterfaceWithEvent_ShouldThrowNotSupportedException()
        {
            var Mapping = new PropertySet();
            Mapping["Value"] = "value";

            Assert.Throws<NotSupportedException>(() =>
            {
                MetadataMapper.Map<IMappingViewWithEvent>(Mapping);
            });
        }

        [Test]
        public void GetMappingView_InterfaceWithIndexer_ShouldThrowNotSupportedException()
        {
            var Mapping = new PropertySet();
            Mapping["Value"] = "value";

            Assert.Throws<NotSupportedException>(() =>
            {
                MetadataMapper.Map<IMappingViewWithIndexer>(Mapping);
            });
        }

        [Test]
        public void GetMappingView_AbstractClass_ShouldThrowMissingMethodException()
        {
            var Mapping = new PropertySet();
            Mapping["Value"] = "value";

            Assert.Throws<NotSupportedException>(() =>
            {
                MetadataMapper.Map<AbstractClassMappingView>(Mapping);
            });
        }

        [Test]
        public void GetMappingView_AbstractClassWithConstructor_ShouldThrowMemberAccessException()
        {
            var Mapping = new PropertySet();
            Mapping["Value"] = "value";

            Assert.Throws<MemberAccessException>(() =>
            {
                MetadataMapper.Map<AbstractClassWithConstructorMappingView>(Mapping);
            });
        }

        [Test]
        public void GetMappingView_IDictionaryAsTMappingViewTypeArgument_ShouldReturnMapping()
        {
            var Mapping = new PropertySet();

            var result = MetadataMapper.Map<IPropertySet>(Mapping);

            Assert.AreSame(Mapping, result);
        }

        [Test]
        public void GetMappingView_IEnumerableAsTMappingViewTypeArgument_ShouldReturnMapping()
        {
            var Mapping = new PropertySet();

            var result = MetadataMapper.Map<IEnumerable<KeyValuePair<string, object>>>(Mapping);

            Assert.AreSame(Mapping, result);
        }


        [Test]
        public void GetMappingView_DictionaryAsTMappingViewTypeArgument_ShouldNotThrow()
        {
            var Mapping = new PropertySet();
            MetadataMapper.Map<PropertySet>(Mapping);
        }

        [Test]
        public void GetMappingView_PrivateInterfaceAsTMappingViewTypeArgument_ShouldhrowNotSupportedException()
        {
            var Mapping = new PropertySet();
            Mapping["CanActivate"] = true;

            Assert.Throws<NotSupportedException>(() =>
            {
                MetadataMapper.Map<IActivator>(Mapping);
            });
        }




        //[Test]
        //public void GetMappingView_InterfaceWithTwoPropertiesWithSameNameDifferentTypeAsTMappingViewArgument_ShouldThrowContractMismatch()
        //{
        //    var Mapping = new PropertySet();
        //    Mapping["Value"] = 10;

        //    Assert.Throws<NotSupportedException>(() =>
        //    {
        //        Mapper.Map<IMappingView2>(Mapping);
        //    });
        //}



        [Test]
        public void GetMappingView_InterfaceInheritance()
        {
            var Mapping = new PropertySet();
            Mapping["Value"] = "value";
            Mapping["Value2"] = "value2";

            var view = MetadataMapper.Map<IMappingView3>(Mapping);
            Assert.AreEqual("value", view.Value);
            Assert.AreEqual("value2", view.Value2);
        }


        [Test]
        public void GetMappingView_CachesViewType()
        {
            var Mapping1 = new PropertySet();
            Mapping1["Value"] = "value1";
            var view1 = MetadataMapper.Map<IMappingView>(Mapping1);
            Assert.AreEqual("value1", view1.Value);

            var Mapping2 = new PropertySet();
            Mapping2["Value"] = "value2";
            var view2 = MetadataMapper.Map<IMappingView>(Mapping2);
            Assert.AreEqual("value2", view2.Value);

            Assert.AreEqual(view1.GetType(), view2.GetType());
        }

        private interface IActivator
        {
            bool CanActivate
            {
                get;
            }
        }


        public interface IMappingView
        {
            string Value
            {
                get;
            }
        }

        public interface IMappingView2 : IMappingView
        {
            new int Value
            {
                get;
            }
        }

        public interface IMappingView3 : IMappingView
        {
            string Value2
            {
                get;
            }
        }

        public interface IMappingViewWithPropertySetter
        {
            string Value
            {
                get;
                set;
            }
        }

        public interface IMappingViewWithMethod
        {
            string Value
            {
                get;
            }
            void Method();
        }

        public interface IMappingViewWithEvent
        {
            string Value
            {
                get;
            }
            event EventHandler TestEvent;
        }

        public interface IMappingViewWithIndexer
        {
            string Value
            {
                get;
            }
            string this[object o] { get; }
        }

        public abstract class AbstractClassMappingView
        {
            public abstract object Value { get; }
        }

        public abstract class AbstractClassWithConstructorMappingView
        {
            public AbstractClassWithConstructorMappingView(PropertySet Mapping) { }
            public abstract object Value { get; }
        }

        public interface IMappingViewWithDefaultedInt
        {
            [DefaultValue(120)]
            int MyInt { get; }
        }

        [Test]
        public void GetMappingView_IMappingViewWithDefaultedInt()
        {
            var view = MetadataMapper.Map<IMappingViewWithDefaultedInt>(new PropertySet());
            Assert.AreEqual(120, view.MyInt);
        }




        public interface IMappingViewWithDefaultedBool
        {
            [DefaultValue(true)]
            bool MyBool { get; }
        }

        [Test]
        public void GetMappingView_IMappingViewWithDefaultedBool()
        {
            var view = MetadataMapper.Map<IMappingViewWithDefaultedBool>(new PropertySet());
            Assert.AreEqual(true, view.MyBool);
        }

        public interface IMappingViewWithDefaultedInt64
        {
            [DefaultValue(Int64.MaxValue)]
            Int64 MyInt64 { get; }
        }

        [Test]
        public void GetMappingView_IMappingViewWithDefaultedInt64()
        {
            var view = MetadataMapper.Map<IMappingViewWithDefaultedInt64>(new PropertySet());
            Assert.AreEqual(Int64.MaxValue, view.MyInt64);
        }

        public interface IMappingViewWithDefaultedString
        {
            [DefaultValue("MyString")]
            string MyString { get; }
        }

        [Test]
        public void GetMappingView_IMappingViewWithDefaultedString()
        {
            var view = MetadataMapper.Map<IMappingViewWithDefaultedString>(new PropertySet());
            Assert.AreEqual("MyString", view.MyString);
        }

        public interface IMappingViewWithTypeMismatchDefaultValue
        {
            [DefaultValue("Strings can't cast to numbers")]
            int MyInt { get; }
        }

        [Test]
        public void GetMappingView_IMappingViewWithTypeMismatchDefaultValue()
        {
            var exception = Assert.Throws<NotSupportedException>(() =>
            {
                MetadataMapper.Map<IMappingViewWithTypeMismatchDefaultValue>(new PropertySet());
            });

            Assert.IsInstanceOfType(typeof(TargetInvocationException), exception.InnerException);
        }


        public interface IMappingViewUnboxAsInt
        {
            int Value { get; }
        }


        [Test]
        public void GetMappingView_IMappingViewWithTypeMismatchOnUnbox()
        {
            var Mapping = new PropertySet();
            Mapping["Value"] = (short)9999;

            var exception = Assert.Throws<NotSupportedException>(() =>
            {
                MetadataMapper.Map<IMappingViewWithTypeMismatchDefaultValue>(new PropertySet());
            });

            Assert.IsInstanceOfType(typeof(TargetInvocationException), exception.InnerException);
        }

        public interface IHasInt32
        {
            Int32 Value { get; }
        }

        [Test]
        public void TestMappingIntConversion()
        {
            var Mapping = new PropertySet();
            Mapping["Value"] = (Int64)45;

            MetadataMapper.Map<IHasInt32>(Mapping);
        }

    }
}
