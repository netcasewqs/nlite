/*
 * Created by SharpDevelop.
 * User: issuser
 * Date: 2011-1-20
 * Time: 16:50
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Reflection;

namespace NLite.Reflection.Internal
{
	 class Property
    {
        public readonly PropertyInfo Name;
        public readonly object Owner;

        public Property(PropertyInfo name, object owner)
        {
            Name = name;
            Owner = owner;
        }
    }

    class TypeDeepCalculator
    {
        public int Deep
        {
            get
            {
                return deep == 1 ? 0 : deep - 2;
            }
        }

        private int deep = 0;
        public bool Calculate(Type parentType, Type type)
        {
            if (type == null)
                return false;

            deep = deep + 1;
            if (parentType.IsSubclassOf(type))
                return true;

            if (parentType.IsInterface)
                return type.GetInterface(parentType.Name) != null;

            return Calculate(parentType, type.BaseType);
        }
    }
}
