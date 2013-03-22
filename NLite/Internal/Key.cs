using System;
using System.Diagnostics;
using NLite.Internal;

namespace NLite.Messaging.Internal
{
    #if !SILVERLIGHT
    [Serializable]
    #endif
    [DebuggerDisplay("Name='{name}', Type = '{Type}'")]
    struct Key : IEquatable<Key>
    {
        private RuntimeTypeHandle? handle;
        private string name;
        private readonly int hashCode;
        private Key(RuntimeTypeHandle type, string name)
        {
            this.handle = type;
            this.name = !string.IsNullOrEmpty(name) ? name : null;

            int typeHash = handle == null ? 0 : handle.GetHashCode();
            int nameHash = name == null ? 0 : name.GetHashCode();
            hashCode = typeHash ^ nameHash;
        }

        private Key(RuntimeTypeHandle type)
        {
            this.handle = type;
            name = null;
            hashCode = handle.GetHashCode();
        }

        private Key(string name)
        {
            Guard.NotNullOrEmpty(name,"name");
            this.name = name;
            handle = null;
            hashCode = name.GetHashCode();
        }

        /// <summary>
        /// 通过类型得到一个Key
        /// </summary>
        /// <param name="binderType"></param>
        /// <returns></returns>
        public static Key Make(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            return new Key(type.TypeHandle, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="binderType"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Key Make(Type type, string name)
        {
            if (type == null)
                return new Key(name);
            return new Key(type.TypeHandle, name);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TTarget"></typeparam>
        /// <returns></returns>
        public static Key Make<T>()
        {
            return new Key(typeof(T).TypeHandle, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Key Make<T>(string name)
        {
            return new Key(typeof(T).TypeHandle, name);
        }

        public static Key Make(string name)
        {
            return new Key(name);
        }

        /// <summary>
        /// 
        /// </summary>
        public RuntimeTypeHandle? Handle
        {
            get { return handle; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Name
        {
            get { return name; }
        }

        /// <summary>
        /// 
        /// </summary>
        public Type Type
        {
            get
            {
                return handle.HasValue ? Type.GetTypeFromHandle(handle.Value) : null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Id
        {
            get
            {
                return string.IsNullOrEmpty(Name) ? Type.Name : Name;
            }
        }

        public override bool Equals(object obj)
        {
            return Equals((Key)obj);
        }


        public override int GetHashCode()
        {
            return hashCode;
        }


        public static bool operator ==(Key left, Key right)
        {
            if (left != null)
                return left.Equals(right);
            return false;
        }


        public static bool operator !=(Key left, Key right)
        {
            return !(left == right);
        }


        public override string ToString()
        {
            return string.IsNullOrEmpty(Name) ? Type.Name : Name;
        }

        public bool Equals(Key other)
        {
            if (other == null)
                return false;
            return object.Equals(other.handle,handle)
                && string.Compare(name, other.name, StringComparison.InvariantCulture) == 0;
        }
    }
}