using System;
using System.Linq;
using NLite.Reflection;
namespace NLite
{
    /// <summary>
    /// 初始化接口
    /// </summary>
    public interface IInitializable
    {
        /// <summary>
        /// 初始化
        /// </summary>
        void Init();
    }

    /// <summary>
    /// 初始化器
    /// </summary>
    //[Contract]
    public abstract class Initializer : IInitializable
    {
        /// <summary>
        /// 用来标识一个方法的执行的顺序
        /// </summary>
        [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
        protected class OrderAttribute : Attribute
        {
            /// <summary>
            /// 序号
            /// </summary>
            public readonly uint Order;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="order">方法执行的顺序</param>
            public OrderAttribute(uint order)
            {
                Order = order;
            }
        }

        static Func[] Actions;

        /// <summary>
        /// 构造初始化器
        /// </summary>
        protected Initializer()
        {
            if (Actions == null)
                Actions = (from m in GetType().GetMethods(MemberFlags.InstanceFlags)
                           let attr = m.GetAttribute<OrderAttribute>(true)
                           let length = m.GetParameters().Length
                           where attr != null && length == 0
                           orderby attr.Order
                           select m.GetFunc()).ToArray();
        }

        /// <summary>
        /// 在初始化前触发
        /// </summary>
        protected virtual void OnInitializing() { }
        /// <summary>
        /// 在初始化后触发
        /// </summary>
        protected virtual void OnInitialized() { }
        /// <summary>
        /// 异常发生时触发
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnExceptionFired(Exception e) { throw e.Handle(); }

        void IInitializable.Init()
        {
            try
            {
                OnInitializing();
                foreach (var action in Actions)
                    action(this, null);
                OnInitialized();
            }
            catch (Exception e)
            {
                OnExceptionFired(e);
            }
        }
        
    }

    /// <summary>
    /// 抽象初始化器
    /// </summary>
    [Component]
    public abstract class AbstractInitializer : Initializer { }
}
