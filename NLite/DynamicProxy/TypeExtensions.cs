using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace NLite.DynamicProxy
{
    static class TypeExtensions
    {
        public static bool CanBeProxied(this Type type)
        {
            if (type.IsSealed)
                return false;
            
            if (!type.IsClass)
                return false;

            BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;

            // Search for any methods that cannot be proxied
            var matches = from m in type.GetMethods(flags)
                          where !(m.IsAbstract && m.IsVirtual) || m.IsFinal
                          select m;

            // A type can only be proxied if all of its public methods are virtual
            return matches.Count() == 0;
        }
    }
}
