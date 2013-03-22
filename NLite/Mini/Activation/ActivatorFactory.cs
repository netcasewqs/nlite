using System;
using System.Collections.Generic;
using NLite.Collections;

namespace NLite.Mini.Activation
{
    /// <summary>
    /// 组件工厂的工厂
    /// </summary>
    public class ActivatorFactory : IActivatorFactory
    {

        private Dictionary<string, Func<IActivator>> Map = new Dictionary<string, Func<IActivator>>();

        /// <summary>
        /// 
        /// </summary>
        public ActivatorFactory()
        {
            Map[ActivatorType.Default] = () => new DefaultActivator();
            Map[ActivatorType.Factory] = () => new DelegateActivator();
            Map[ActivatorType.Instance] = () => new InstanceActivator();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="binderType"></param>
        /// <param name="creator"></param>
        public void Register(string type, Func<IActivator> creator)
        {
            if (string.IsNullOrEmpty(type))
                throw new ArgumentNullException("type");
            if (creator == null)
                throw new ArgumentNullException("creator");

            lock(Map)
                Map.Add(type, creator);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="binderType"></param>
        public void Unregister(string type)
        {
            if (string.IsNullOrEmpty(type))
                throw new ArgumentNullException("type");
            lock (Map)
            {
                if (Map.ContainsKey(type))
                    Map.Remove(type);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="binderType"></param>
        /// <returns></returns>
        public IActivator Create(string type)
        {
            if (string.IsNullOrEmpty(type))
                throw new ArgumentNullException("type");

            Func<IActivator> item;
            if (!Map.TryGetValue(type, out item))
                throw new ArgumentException("invalid activator type:" + type);

            return item();
        }
       
    }

   
}
