using System;
using NLite.Messaging.Internal;
using System.Reflection;
using NLite.Internal;

namespace NLite.Messaging
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    public interface IObserverHandler<TMessage>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="message"></param>
        object Invoke(object sender, TMessage message);

        /// <summary>
        /// 
        /// </summary>
        object Target { get; }

        /// <summary>
        /// 
        /// </summary>
        MethodInfo Method { get; }
    }


    /// <summary>
    /// 
    /// </summary>
    public static class ObserverHandler
    {
        static Func<object, TData, object> Convert<TData, TResult>(this Func<object, TData, TResult> source)
        {
            return (s, args) => (object)source(s, args);
        }
        static Func<TData, object> Convert<TData, TResult>(this Func<TData, TResult> source)
        {
            return s => (object)source(s);
        }
        static Func<object> Convert<TResult>(this Func<TResult> source)
        {
            return () => (object)source();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="handler"></param>
        /// <returns></returns>
        public static IObserverHandler<IMessage> Create2<TData>(Action<object,TData> handler)
        {
            
            
            return new DefaultObserverHandler2<TData>
            {
                InnerHandler = (s, args) =>
                    {
                        handler(s, args);
                        return Null.Value;
                    }
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="handler"></param>
        /// <returns></returns>
        public static IObserverHandler<IMessage> CreateC<TData, TResult>(Func<object, TData, TResult> handler)
        {
            return new DefaultObserverHandler2<TData>
            {
                InnerHandler = (target, args) => handler(target, args)
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="handler"></param>
        /// <returns></returns>
        public static IObserverHandler<IMessage> Create1<TData>(Action<TData> handler)
        {
            return new DefaultObserverHandler1<TData>
            {
                InnerHandler = args =>
                    {
                        handler(args);
                        return Null.Value;
                    }
            };
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <typeparam name="TData"></typeparam>
        ///// <param name="handler"></param>
        ///// <returns></returns>
        //public static IObserverHandler<IMessage> CreateB<TData>(Func<TData,object> handler)
        //{
        //    return new DefaultObserverHandler1<TData>
        //    {
        //        InnerHandler = handler
        //    };
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="handler"></param>
        /// <returns></returns>
        public static IObserverHandler<IMessage> CreateB<TData, TResult>(Func<TData, TResult> handler)
        {
            return new DefaultObserverHandler1<TData>
            {
                InnerHandler = handler.Convert<TData,TResult>()
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="handler"></param>
        /// <returns></returns>
        public static IObserverHandler<IMessage> Create(Action handler)
        {
            return new DefaultObserverHandler
            {
                InnerHandler = () =>
                    {

                        handler();
                        return Null.Value;
                    }
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="handler"></param>
        /// <returns></returns>
        public static IObserverHandler<IMessage> CreateA<TResult>(Func<TResult> handler)
        {
            return new DefaultObserverHandler
            {
                InnerHandler = ()=>handler(),
            };
        }
    }
}
