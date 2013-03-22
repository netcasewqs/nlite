using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLite.Collections;
using NUnit.Framework;
using System.Reflection;

namespace NLite.Mapping.Test
{
    [TestFixture]
    public class MetadataMapperTest
    {
        [Test]
        public void Test()
        {
            var type = typeof(Mapper).Assembly.GetType("NLite.Mapping.Internal.MetadataMapper");
            Assert.IsNotNull(type);

            var methods = type.GetMethods().Where(m=>m.Name == "Map").ToArray();

            MethodInfo MapToDict = methods.Where(m => m.GetGenericArguments().Length == 1).FirstOrDefault();
            MethodInfo ArgsMap = methods.Where(m => m.GetParameters().Length == 1).FirstOrDefault();
            MethodInfo LastMap = methods.Except(new MethodInfo[] { MapToDict, ArgsMap }).FirstOrDefault();

            Assert.IsNotNull(MapToDict);
            Assert.IsNotNull(ArgsMap);
            Assert.IsNotNull(LastMap);

        }
    }
    static class MetadataMapper
    {
        static readonly MethodInfo MapToDict;
        static readonly MethodInfo ArgsMap;
        static readonly MethodInfo LastMap;

        static MetadataMapper()
        {
            var type = typeof(Mapper).Assembly.GetType("NLite.Mapping.Internal.MetadataMapper");
            var methods = type.GetMethods().Where(m => m.Name == "Map").ToArray();

            MapToDict = methods.Where(m => m.GetGenericArguments().Length == 1).FirstOrDefault();
            ArgsMap = methods.Where(m => m.GetParameters().Length == 1).FirstOrDefault();
            LastMap = methods.Except(new MethodInfo[] { MapToDict, ArgsMap }).FirstOrDefault();

        }

        public static To Map<From, To>() where To : class
        {
            try
            {
                return (To)LastMap.MakeGenericMethod(typeof(From), typeof(To)).Invoke(null, null);
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }



        public static IDictionary<string, object> Map<From>()
        {
            try
            {
                return (IDictionary<string, object>)MapToDict.MakeGenericMethod(typeof(From)).Invoke(null, null);
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }


        public static To Map<To>(PropertySet mappings)
        {
            try
            {
                return (To)ArgsMap.MakeGenericMethod(typeof(To)).Invoke(null, new object[] { mappings });
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }

    }
}
