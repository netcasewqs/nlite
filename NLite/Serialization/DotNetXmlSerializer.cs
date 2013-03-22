using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using NLite.Reflection;
using NLite.Mapping;
using System.Collections;
using NLite.Mapping.Internal;
using System.Reflection;
using NLite.ComponentModel;

namespace NLite.Serialization
{
    public class DotNetXmlSerializer : ISerializer
    {
        private static ISerializer defaultInstance = new DotNetXmlSerializer();
        public static ISerializer Current { get { return defaultInstance; } }
        public static void SetSerializer(ISerializer serializer)
        {
            if (serializer == null)
                throw new ArgumentNullException("serializer");

            defaultInstance = serializer;
        }


        public string Serialize(object o)
        {
            if (o == null) return null;

            var includeTypes = new HashSet<Type>();
            var track = new HashSet<object>();
            PopulateInculdeTypes(o, track, includeTypes);

            var serialzier = new System.Xml.Serialization.XmlSerializer(o.GetType(), includeTypes.ToArray());
            var writer = new EncodingStringWriter();
            serialzier.Serialize(writer, o);

            return writer.ToString();
        }

        private static void PopulateInculdeTypes(object instance, HashSet<object> track, HashSet<Type> includeTypes)
        {
            if (track.Contains(instance))
                return;

            var instanceType = instance.GetType();
            if (Converter.IsPrimitiveType(instanceType))
                return;

            track.Add(instance);

            if (Types.IEnumerable.IsAssignableFrom(instanceType))
            {
                foreach (var item in instance as IEnumerable)
                    PopulateInculdeTypes(item, track, includeTypes);
                return;
            }

            if (instanceType.IsIDictionaryType())
            {
                var dict = (instance as IDictionary);
                PopulateInculdeTypes(dict.Keys, track, includeTypes);
                PopulateInculdeTypes(dict.Values, track, includeTypes);
                return;
            }

            if (instanceType.IsGenericDictionaryType())
            {
                var keys = instance.GetProperty("Keys");
                var values = instance.GetProperty("Values");
                PopulateInculdeTypes(keys, track, includeTypes);
                PopulateInculdeTypes(values, track, includeTypes);
                return;
            }

            includeTypes.Add(instanceType);

            foreach (var m in instanceType.GetFields().Where(m => !m.HasAttribute<System.Xml.Serialization.XmlIgnoreAttribute>(true)))
            {
                var fv = m.GetValue(instance);
                if (fv != null)
                    PopulateInculdeTypes(fv, track, includeTypes);
            }
            foreach (var m in instance.GetType().GetProperties().Where(m => !m.HasAttribute<System.Xml.Serialization.XmlIgnoreAttribute>(true)))
            {
                var fv = m.GetValue(instance, null);
                if (fv != null)
                    PopulateInculdeTypes(fv, track, includeTypes);
            }
        }

        private class EncodingStringWriter : StringWriter
        {
            public override Encoding Encoding
            {
                get { return Encoding.UTF8; }
            }
        }

        public object Deserialize(string str, Type type)
        {
            if (string.IsNullOrEmpty(str))
                throw new ArgumentNullException(str);
            if (type == null)
                throw new ArgumentNullException("type");

            var serialzier = new System.Xml.Serialization.XmlSerializer(type);
            var reader = new StringReader(str);
            return serialzier.Deserialize(reader);
        }
    }

}
