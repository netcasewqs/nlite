using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System;
using System.Xml.Serialization;
using System.IO;

namespace NLite.Internal
{
    sealed class DebuggerEnumerableView<T>
    {
        private IEnumerable<T> m_enumerable;

        public DebuggerEnumerableView(IEnumerable<T> enumerable)
        {
            m_enumerable = enumerable;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Items
        {
            get { return m_enumerable.ToArray(); }
        }
    }

     #if !SILVERLIGHT
    [Serializable]
    #endif
    internal class SerializedValue
    {
        string content;

        public string Content
        {
            get { return content; }
        }

        public object Deserialize(Type type)
        {
            XmlSerializer serializer = new XmlSerializer(type);
            using (var reader = new StringReader(content))
                return serializer.Deserialize(reader);
        }

        public SerializedValue(string content)
        {
            this.content = content;
        }
    }
}
