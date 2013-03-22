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
    //[Contract]
    public interface ISerializer
    {
        string Serialize(object o);
        object Deserialize(string str, Type type);
    }

    public static class DeserializerExtensions
    {
        public static T Deserialize<T>(this ISerializer ser, string str)
        {
            if (ser == null)
                throw new ArgumentNullException("ser");
            return (T)ser.Deserialize(str, typeof(T));
        }
    }

   
    
}
