
namespace NLite
{
    /// <summary>
    /// 对象工厂类型
    /// </summary>
    public class ActivatorType
    {
        /// <summary>
        /// 缺省工厂类型
        /// </summary>
        public const string Default = "Default";
        /// <summary>
        /// 实例工厂类型
        /// </summary>
        public const string Instance = "Instance";
        /// <summary>
        /// 工厂的工厂类型
        /// </summary>
        public const string Factory = "Factory";
        /// <summary>
        /// 代理工厂类型
        /// </summary>
        public const string Proxy = "Proxy";
    }

    /// <summary>
    /// 组件生命周期类型
    /// </summary>
    public class  LifestyleType
    {
        /// <summary>
        /// 得到或设置组件的确实生命周期类型
        /// </summary>
        public static LifestyleFlags Default {get;set;}

        static LifestyleType()
        {
            Default = LifestyleFlags.Singleton;
        }

        internal static LifestyleFlags GetGenericLifestyle(LifestyleFlags lifestyle)
        {
            if (lifestyle == LifestyleFlags.Singleton)
                return LifestyleFlags.GenericSingleton;
            if (lifestyle == LifestyleFlags.Transient)
                return LifestyleFlags.GenericTransient;
            if (lifestyle == LifestyleFlags.Thread)
                return LifestyleFlags.GenericThread;
            return lifestyle;
        }

        internal static LifestyleFlags GetLifestyle(LifestyleFlags lifestyle)
        {
            if (lifestyle == LifestyleFlags.GenericSingleton)
                return LifestyleFlags.Singleton;
            if (lifestyle == LifestyleFlags.GenericTransient)
                return LifestyleFlags.Transient;
            if (lifestyle == LifestyleFlags.GenericThread)
                return LifestyleFlags.Thread;
            return lifestyle;
        }
    }

    /// <summary>
    /// 组件生命周期类型枚举
    /// </summary>
    public enum LifestyleFlags
    {
        /// <summary>
        /// 单利
        /// </summary>
        Singleton,
        /// <summary>
        /// 线程内单利
        /// </summary>
        Thread,
        /// <summary>
        /// 临时
        /// </summary>
        Transient,

        /// <summary>
        /// 泛型单利
        /// </summary>
        GenericSingleton,
        /// <summary>
        /// 泛型线程内单利
        /// </summary>
        GenericThread,
        /// <summary>
        /// 泛型临时
        /// </summary>
        GenericTransient,
    }
}
