using System;
using System.Collections.Generic;
using NLite.Reflection;
using NUnit.Framework;

namespace NLite.Test.Reflection
{
    [TestFixture]
    public class GrnericReflectionTest
    {
       

        [Test]
        public void GetNamedGenericParameterTest()
        {
            Assert.IsNotNull(typeof(IList<>).GetNamedGenericParameter("T"));
        }

        [Test]
        public void IsOpenGenericTypeTest()
        {
            var openGenericType = typeof(List<>);
            Assert.IsTrue(openGenericType.IsOpenGenericType());

            var closeGenericType = typeof(List<object>);
            Assert.IsFalse(closeGenericType.IsOpenGenericType());
        }

        [Test]
        public void HasOpenGenericParametersTest()
        {
            var noGenericConstroctor = typeof(List<>).GetConstructor(new Type[0]);
            Assert.IsFalse(noGenericConstroctor.HasOpenGenericParameters());

            var noGenericAddMethod = typeof(List<>).GetMethod("Add");
            Assert.IsFalse(noGenericAddMethod.HasOpenGenericParameters());

            noGenericAddMethod = typeof(List<KeyValuePair<string, int>>).GetMethod("Add");
            Assert.IsFalse(noGenericAddMethod.HasOpenGenericParameters());

            var genericMethod = typeof(MyDic<,>).GetMethod("Add");
            Assert.IsTrue(genericMethod.HasOpenGenericParameters());
        }

        [Test]
        public void MakeCloseGenericTypeTest()
        {
            var openGenericType = typeof(MyDic<,>);
            Assert.IsTrue(openGenericType.IsOpenGenericType());

            var closeGenericType = openGenericType.MakeCloseGenericType(typeof(int), typeof(string));
            Assert.IsFalse(closeGenericType.IsOpenGenericType());
        }

        private class MyDic<TKey, TValue> : List<KeyValuePair<TKey, TValue>>
        {
        }
    }
}
