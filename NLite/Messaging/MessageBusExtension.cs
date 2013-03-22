using System;
using System.Linq.Expressions;
using System.Reflection;
using NLite.Internal;
using NLite.Messaging.Internal;
using NLite.Reflection;

namespace NLite.Messaging
{
    /// <summary>
    /// 
    /// </summary>
    public static class MessageBusExtensions
    {
        #region Func<object,TData,TResult>
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="bus"></param>
        /// <param name="topic">消息主题</param>
        /// <param name="handler">处理器</param>
        /// <returns></returns>
        public static IDisposable Subscribe<TData, TResult>(this IMessageBus bus, string topic, Func<object, TData, TResult> handler)
        {
            Guard.NotNull(bus, "bus");
            Guard.NotNull(handler, "handler");
            //return bus.Subscribe(topic, typeof(TData), SubscribeMode.Sync, ObserverHandler.CreateC<TData,TResult>(handler));
            return bus.Subscribe(new SubscribeInfo<TData, TResult>(handler) { Topic = topic, Mode = SubscribeMode.Sync });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="bus"></param>
        /// <param name="topic">消息主题</param>
        /// <param name="mode"></param>
        /// <param name="handler">处理器</param>
        /// <returns></returns>
        public static IDisposable Subscribe<TData, TResult>(this IMessageBus bus, string topic, SubscribeMode mode, Func<object, TData, TResult> handler)
        {
            Guard.NotNull(bus, "bus");
            Guard.NotNull(handler, "handler");
            //return bus.Subscribe(topic, typeof(TData), mode, ObserverHandler.CreateC<TData, TResult>(handler));
            return bus.Subscribe(new SubscribeInfo<TData, TResult>(handler) { Topic = topic, Mode = mode });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="bus"></param>
        /// <param name="topic">消息主题</param>
        /// <param name="handler">处理器</param>
        public static void Unsubscribe<TData, TResult>(this IMessageBus bus, string topic, Func<object, TData, TResult> handler)
        {
            Guard.NotNull(bus, "bus");
            Guard.NotNull(handler, "handler");
            bus.Unsubscribe(topic, typeof(TData), ObserverHandler.CreateC<TData, TResult>(handler));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="bus"></param>
        /// <param name="handler">处理器</param>
        /// <returns></returns>
        public static IDisposable Subscribe<TData, TResult>(this IMessageBus bus, Func<object, TData, TResult> handler)
        {
            Guard.NotNull(bus, "bus");
            Guard.NotNull(handler, "handler");
            return Subscribe<TData, TResult>(bus, SubscribeMode.Sync, handler);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="bus"></param>
        /// <param name="mode"></param>
        /// <param name="handler">处理器</param>
        /// <returns></returns>
        public static IDisposable Subscribe<TData, TResult>(this IMessageBus bus, SubscribeMode mode, Func<object, TData, TResult> handler)
        {
            Guard.NotNull(bus, "bus");
            Guard.NotNull(handler, "handler");
            //return bus.Subscribe(null, typeof(TData), mode, ObserverHandler.CreateC<TData, TResult>(handler));
            return bus.Subscribe(new SubscribeInfo<TData, TResult>(handler) { Mode = mode });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="bus"></param>
        /// <param name="handler">处理器</param>
        public static void Unsubscribe<TData, TResult>(this IMessageBus bus, Func<object, TData, TResult> handler)
        {
            Guard.NotNull(bus, "bus");
            Guard.NotNull(handler, "handler");
            bus.Unsubscribe(null, typeof(TData), ObserverHandler.CreateC<TData, TResult>(handler));
        }
        #endregion

        #region Func<TData,TResult>
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="bus"></param>
        /// <param name="topic">消息主题</param>
        /// <param name="handler">处理器</param>
        /// <returns></returns>
        public static IDisposable Subscribe<TData, TResult>(this IMessageBus bus, string topic, Func<TData, TResult> handler)
        {
            Guard.NotNull(bus, "bus");
            Guard.NotNull(handler, "handler");
            //return bus.Subscribe(topic, typeof(TData), SubscribeMode.Sync, ObserverHandler.CreateB<TData, TResult>(handler));
            return bus.Subscribe(new SubscribeInfo<TData, TResult>(handler) { Topic = topic });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="bus"></param>
        /// <param name="topic">消息主题</param>
        /// <param name="mode"></param>
        /// <param name="handler">处理器</param>
        /// <returns></returns>
        public static IDisposable Subscribe<TData, TResult>(this IMessageBus bus, string topic, SubscribeMode mode, Func<TData, TResult> handler)
        {
            Guard.NotNull(bus, "bus");
            Guard.NotNull(handler, "handler");
            //return bus.Subscribe(topic, typeof(TData), mode, ObserverHandler.CreateB<TData, TResult>(handler));
            return bus.Subscribe(new SubscribeInfo<TData, TResult>(handler) { Topic = topic, Mode = mode });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="bus"></param>
        /// <param name="topic">消息主题</param>
        /// <param name="handler">处理器</param>
        public static void Unsubscribe<TData, TResult>(this IMessageBus bus, string topic, Func<TData, TResult> handler)
        {
            Guard.NotNull(bus, "bus");
            Guard.NotNull(handler, "handler");
            bus.Unsubscribe(topic, typeof(TData), ObserverHandler.CreateB<TData, TResult>(handler));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="bus"></param>
        /// <param name="handler">处理器</param>
        /// <returns></returns>
        public static IDisposable Subscribe<TData, TResult>(this IMessageBus bus, Func<TData, TResult> handler)
        {
            Guard.NotNull(bus, "bus");
            Guard.NotNull(handler, "handler");
            //return bus.Subscribe(null, typeof(TData), SubscribeMode.Sync, ObserverHandler.CreateB<TData, TResult>(handler));
            return bus.Subscribe(new SubscribeInfo<TData, TResult>(handler));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="bus"></param>
        /// <param name="mode"></param>
        /// <param name="handler">处理器</param>
        /// <returns></returns>
        public static IDisposable Subscribe<TData, TResult>(this IMessageBus bus, SubscribeMode mode, Func<TData, TResult> handler)
        {
            Guard.NotNull(bus, "bus");
            Guard.NotNull(handler, "handler");
            //return bus.Subscribe(null, typeof(TData), mode, ObserverHandler.CreateB<TData, TResult>(handler));
            return bus.Subscribe(new SubscribeInfo<TData, TResult>(handler) { Mode = mode });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="bus"></param>
        /// <param name="handler">处理器</param>
        public static void Unsubscribe<TData, TResult>(this IMessageBus bus, Func<TData, TResult> handler)
        {
            Guard.NotNull(bus, "bus");
            Guard.NotNull(handler, "handler");
            bus.Unsubscribe(null, typeof(TData), ObserverHandler.CreateB<TData, TResult>(handler));
        }
        #endregion

        #region Func<TResult>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bus"></param>
        /// <param name="topic">消息主题</param>
        /// <param name="handler">处理器</param>
        /// <returns></returns>
        public static IDisposable Subscribe<TResult>(this IMessageBus bus, string topic, Func<TResult> handler)
        {
            Guard.NotNull(bus, "bus");
            Guard.NotNull(handler, "handler");
            //return bus.Subscribe(topic, null, SubscribeMode.Sync, ObserverHandler.CreateA(handler));
            return bus.Subscribe(new SubscribeInfo<TResult>(handler) { Topic = topic });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bus"></param>
        /// <param name="topic">消息主题</param>
        /// <param name="mode"></param>
        /// <param name="handler">处理器</param>
        /// <returns></returns>
        public static IDisposable Subscribe<TResult>(this IMessageBus bus, string topic, SubscribeMode mode, Func<TResult> handler)
        {
            Guard.NotNull(bus, "bus");
            Guard.NotNull(handler, "handler");
            //return bus.Subscribe(topic, null, mode, ObserverHandler.CreateA(handler));
            return bus.Subscribe(new SubscribeInfo<TResult>(handler) { Topic = topic, Mode = mode });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bus"></param>
        /// <param name="topic">消息主题</param>
        /// <param name="handler">处理器</param>
        public static void Unsubscribe<TResult>(this IMessageBus bus, string topic, Func<TResult> handler)
        {
            Guard.NotNull(bus, "bus");
            Guard.NotNull(handler, "handler");
            bus.Unsubscribe(topic, null, ObserverHandler.CreateA(handler));
        }
        #endregion


        #region Func<object,TData,object>

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="bus"></param>
        /// <param name="topic">消息主题</param>
        /// <param name="handler">处理器</param>
        /// <returns></returns>
        public static IDisposable Subscribe<TData>(this IMessageBus bus, string topic, Func<object, TData, object> handler)
        {
            Guard.NotNull(bus, "bus");
            Guard.NotNull(handler, "handler");
            //return bus.Subscribe(topic, typeof(TData), SubscribeMode.Sync, ObserverHandler.CreateC<TData, object>(handler));
            return bus.Subscribe(new SubscribeInfo<TData>(handler) { Topic = topic });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="bus"></param>
        /// <param name="topic">消息主题</param>
        /// <param name="mode"></param>
        /// <param name="handler">处理器</param>
        /// <returns></returns>
        public static IDisposable Subscribe<TData>(this IMessageBus bus, string topic, SubscribeMode mode, Func<object, TData, object> handler)
        {
            Guard.NotNull(bus, "bus");
            Guard.NotNull(handler, "handler");
            //return bus.Subscribe(topic, typeof(TData), mode, ObserverHandler.CreateC<TData, object>(handler));
            return bus.Subscribe(new SubscribeInfo<TData>(handler) { Topic = topic, Mode = mode });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="bus"></param>
        /// <param name="topic">消息主题</param>
        /// <param name="handler">处理器</param>
        public static void Unsubscribe<TData>(this IMessageBus bus, string topic, Func<object, TData, object> handler)
        {
            Guard.NotNull(bus, "bus");
            Guard.NotNull(handler, "handler");
            bus.Unsubscribe(topic, typeof(TData), ObserverHandler.CreateC<TData, object>(handler));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="bus"></param>
        /// <param name="handler">处理器</param>
        /// <returns></returns>
        public static IDisposable Subscribe<TData>(this IMessageBus bus, Func<object, TData, object> handler)
        {
            Guard.NotNull(bus, "bus");
            Guard.NotNull(handler, "handler");
            //return Subscribe<TData>(bus, SubscribeMode.Sync, handler);
            return bus.Subscribe(new SubscribeInfo<TData>(handler));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="bus"></param>
        /// <param name="mode"></param>
        /// <param name="handler">处理器</param>
        /// <returns></returns>
        public static IDisposable Subscribe<TData>(this IMessageBus bus, SubscribeMode mode, Func<object, TData, object> handler)
        {
            Guard.NotNull(bus, "bus");
            Guard.NotNull(handler, "handler");
            //return bus.Subscribe(null, typeof(TData), mode, ObserverHandler.CreateC<TData, object>(handler));
            return bus.Subscribe(new SubscribeInfo<TData>(handler) { Mode = mode });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="bus"></param>
        /// <param name="handler">处理器</param>
        public static void Unsubscribe<TData>(this IMessageBus bus, Func<object, TData, object> handler)
        {
            Guard.NotNull(bus, "bus");
            Guard.NotNull(handler, "handler");
            bus.Unsubscribe(null, typeof(TData), ObserverHandler.CreateC<TData, object>(handler));
        }
        #endregion

        #region Func<TData,object>
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="bus"></param>
        /// <param name="topic">消息主题</param>
        /// <param name="handler">处理器</param>
        /// <returns></returns>
        public static IDisposable Subscribe<TData>(this IMessageBus bus, string topic, Func<TData, object> handler)
        {
            Guard.NotNull(bus, "bus");
            Guard.NotNull(handler, "handler");
            //return bus.Subscribe(topic, typeof(TData), SubscribeMode.Sync, ObserverHandler.CreateB<TData, object>(handler));
            return bus.Subscribe(new SubscribeInfo<TData>(handler) { Topic = topic });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="bus"></param>
        /// <param name="topic">消息主题</param>
        /// <param name="mode"></param>
        /// <param name="handler">处理器</param>
        /// <returns></returns>
        public static IDisposable Subscribe<TData>(this IMessageBus bus, string topic, SubscribeMode mode, Func<TData, object> handler)
        {
            Guard.NotNull(bus, "bus");
            Guard.NotNull(handler, "handler");
            //return bus.Subscribe(topic, typeof(TData), mode, ObserverHandler.CreateB<TData, object>(handler));
            return bus.Subscribe(new SubscribeInfo<TData>(handler) { Topic = topic, Mode = mode });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="bus"></param>
        /// <param name="topic">消息主题</param>
        /// <param name="handler">处理器</param>
        public static void Unsubscribe<TData>(this IMessageBus bus, string topic, Func<TData, object> handler)
        {
            Guard.NotNull(bus, "bus");
            Guard.NotNull(handler, "handler");
            bus.Unsubscribe(topic, typeof(TData), ObserverHandler.CreateB<TData, object>(handler));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="bus"></param>
        /// <param name="handler">处理器</param>
        /// <returns></returns>
        public static IDisposable Subscribe<TData>(this IMessageBus bus, Func<TData, object> handler)
        {
            Guard.NotNull(bus, "bus");
            Guard.NotNull(handler, "handler");
            //return bus.Subscribe(null, typeof(TData), SubscribeMode.Sync, ObserverHandler.CreateB<TData, object>(handler));
            return bus.Subscribe(new SubscribeInfo<TData>(handler));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="bus"></param>
        /// <param name="mode"></param>
        /// <param name="handler">处理器</param>
        /// <returns></returns>
        public static IDisposable Subscribe<TData>(this IMessageBus bus, SubscribeMode mode, Func<TData, object> handler)
        {
            Guard.NotNull(bus, "bus");
            Guard.NotNull(handler, "handler");
            //return bus.Subscribe(null, typeof(TData), mode, ObserverHandler.CreateB<TData, object>(handler));
            return bus.Subscribe(new SubscribeInfo<TData>(handler) { Mode = mode });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="bus"></param>
        /// <param name="handler">处理器</param>
        public static void Unsubscribe<TData>(this IMessageBus bus, Func<TData, object> handler)
        {
            Guard.NotNull(bus, "bus");
            Guard.NotNull(handler, "handler");
            bus.Unsubscribe(null, typeof(TData), ObserverHandler.CreateB<TData, object>(handler));
        }
        #endregion

        #region Func<object>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bus"></param>
        /// <param name="topic">消息主题</param>
        /// <param name="handler">处理器</param>
        /// <returns></returns>
        public static IDisposable Subscribe(this IMessageBus bus, string topic, Func<object> handler)
        {
            Guard.NotNull(bus, "bus");
            Guard.NotNull(handler, "handler");
            //return bus.Subscribe(topic, null, SubscribeMode.Sync, ObserverHandler.CreateA(handler));
            return bus.Subscribe(new SubscribeInfo(handler) { Topic = topic });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bus"></param>
        /// <param name="topic">消息主题</param>
        /// <param name="mode"></param>
        /// <param name="handler">处理器</param>
        /// <returns></returns>
        public static IDisposable Subscribe(this IMessageBus bus, string topic, SubscribeMode mode, Func<object> handler)
        {
            Guard.NotNull(bus, "bus");
            Guard.NotNull(handler, "handler");
            //return bus.Subscribe(topic, null, mode, ObserverHandler.CreateA(handler));
            return bus.Subscribe(new SubscribeInfo(handler) { Topic = topic, Mode = mode });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bus"></param>
        /// <param name="topic">消息主题</param>
        /// <param name="handler">处理器</param>
        public static void Unsubscribe(this IMessageBus bus, string topic, Func<object> handler)
        {
            Guard.NotNull(bus, "bus");
            Guard.NotNull(handler, "handler");
            bus.Unsubscribe(topic, null, ObserverHandler.CreateA(handler));
        }
        #endregion


        #region Action<object,TData>

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="bus"></param>
        /// <param name="topic">消息主题</param>
        /// <param name="handler">处理器</param>
        /// <returns></returns>
        public static IDisposable Subscribe<TData>(this IMessageBus bus, string topic, Action<object, TData> handler)
        {
            Guard.NotNull(bus, "bus");
            Guard.NotNull(handler, "handler");
            //return bus.Subscribe(topic, typeof(TData), SubscribeMode.Sync, ObserverHandler.Create2<TData>(handler));
            return bus.Subscribe(new SubscribeInfo<TData>(handler) { Topic = topic });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="bus"></param>
        /// <param name="topic">消息主题</param>
        /// <param name="mode"></param>
        /// <param name="handler">处理器</param>
        /// <returns></returns>
        public static IDisposable Subscribe<TData>(this IMessageBus bus, string topic, SubscribeMode mode, Action<object, TData> handler)
        {
            Guard.NotNull(bus, "bus");
            Guard.NotNull(handler, "handler");
            //return bus.Subscribe(topic, typeof(TData), mode, ObserverHandler.Create2<TData>(handler));
            return bus.Subscribe(new SubscribeInfo<TData>(handler) { Topic = topic, Mode = mode });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="bus"></param>
        /// <param name="topic">消息主题</param>
        /// <param name="handler">处理器</param>
        public static void Unsubscribe<TData>(this IMessageBus bus, string topic, Action<object, TData> handler)
        {
            Guard.NotNull(bus, "bus");
            Guard.NotNull(handler, "handler");
            bus.Unsubscribe(topic, typeof(TData), ObserverHandler.Create2<TData>(handler));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="bus"></param>
        /// <param name="handler">处理器</param>
        /// <returns></returns>
        public static IDisposable Subscribe<TData>(this IMessageBus bus, Action<object, TData> handler)
        {
            Guard.NotNull(bus, "bus");
            Guard.NotNull(handler, "handler");
            //return Subscribe<TData>(bus, SubscribeMode.Sync, handler);
            return bus.Subscribe(new SubscribeInfo<TData>(handler));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="bus"></param>
        /// <param name="mode"></param>
        /// <param name="handler">处理器</param>
        /// <returns></returns>
        public static IDisposable Subscribe<TData>(this IMessageBus bus, SubscribeMode mode, Action<object, TData> handler)
        {
            Guard.NotNull(bus, "bus");
            Guard.NotNull(handler, "handler");
            //return bus.Subscribe(null, typeof(TData), mode, ObserverHandler.Create2<TData>(handler));
            return bus.Subscribe(new SubscribeInfo<TData>(handler) { Mode = mode });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="bus"></param>
        /// <param name="handler">处理器</param>
        public static void Unsubscribe<TData>(this IMessageBus bus, Action<object, TData> handler)
        {
            Guard.NotNull(bus, "bus");
            Guard.NotNull(handler, "handler");
            bus.Unsubscribe(null, typeof(TData), ObserverHandler.Create2<TData>(handler));
        }
        #endregion

        #region Action<TData>
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="bus"></param>
        /// <param name="topic">消息主题</param>
        /// <param name="handler">处理器</param>
        /// <returns></returns>
        public static IDisposable Subscribe<TData>(this IMessageBus bus, string topic, Action<TData> handler)
        {
            Guard.NotNull(bus, "bus");
            Guard.NotNull(handler, "handler");
            //return bus.Subscribe(topic, typeof(TData), SubscribeMode.Sync, ObserverHandler.Create1<TData>(handler));
            return bus.Subscribe(new SubscribeInfo<TData>(handler) { Topic = topic });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="bus"></param>
        /// <param name="topic">消息主题</param>
        /// <param name="mode"></param>
        /// <param name="handler">处理器</param>
        /// <returns></returns>
        public static IDisposable Subscribe<TData>(this IMessageBus bus, string topic, SubscribeMode mode, Action<TData> handler)
        {
            Guard.NotNull(bus, "bus");
            Guard.NotNull(handler, "handler");
            //return bus.Subscribe(topic, typeof(TData), mode, ObserverHandler.Create1<TData>(handler));
            return bus.Subscribe(new SubscribeInfo<TData>(handler) { Topic = topic, Mode = mode });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="bus"></param>
        /// <param name="topic">消息主题</param>
        /// <param name="handler">处理器</param>
        public static void Unsubscribe<TData>(this IMessageBus bus, string topic, Action<TData> handler)
        {
            Guard.NotNull(bus, "bus");
            Guard.NotNull(handler, "handler");
            bus.Unsubscribe(topic, typeof(TData), ObserverHandler.Create1<TData>(handler));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="bus"></param>
        /// <param name="handler">处理器</param>
        /// <returns></returns>
        public static IDisposable Subscribe<TData>(this IMessageBus bus, Action<TData> handler)
        {
            Guard.NotNull(bus, "bus");
            Guard.NotNull(handler, "handler");
            //return bus.Subscribe(null, typeof(TData), SubscribeMode.Sync, ObserverHandler.Create1<TData>(handler));
            return bus.Subscribe(new SubscribeInfo<TData>(handler));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="bus"></param>
        /// <param name="mode"></param>
        /// <param name="handler">处理器</param>
        /// <returns></returns>
        public static IDisposable Subscribe<TData>(this IMessageBus bus, SubscribeMode mode, Action<TData> handler)
        {
            Guard.NotNull(bus, "bus");
            Guard.NotNull(handler, "handler");
            //return bus.Subscribe(null, typeof(TData), mode, ObserverHandler.Create1<TData>(handler));
            return bus.Subscribe(new SubscribeInfo<TData>(handler) { Mode = mode });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="bus"></param>
        /// <param name="handler">处理器</param>
        public static void Unsubscribe<TData>(this IMessageBus bus, Action<TData> handler)
        {
            Guard.NotNull(bus, "bus");
            Guard.NotNull(handler, "handler");
            bus.Unsubscribe(null, typeof(TData), ObserverHandler.Create1<TData>(handler));
        }
        #endregion

        #region Action
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bus"></param>
        /// <param name="topic">消息主题</param>
        /// <param name="handler">处理器</param>
        /// <returns></returns>
        public static IDisposable Subscribe(this IMessageBus bus, string topic, Action handler)
        {
            Guard.NotNull(bus, "bus");
            Guard.NotNull(handler, "handler");
            //return bus.Subscribe(topic, null, SubscribeMode.Sync, ObserverHandler.Create(handler));
            return bus.Subscribe(new SubscribeInfo(handler) { Topic = topic });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bus"></param>
        /// <param name="topic">消息主题</param>
        /// <param name="mode"></param>
        /// <param name="handler">处理器</param>
        /// <returns></returns>
        public static IDisposable Subscribe(this IMessageBus bus, string topic, SubscribeMode mode, Action handler)
        {
            Guard.NotNull(bus, "bus");
            Guard.NotNull(handler, "handler");
            //return bus.Subscribe(topic, null, mode, ObserverHandler.Create(handler));
            return bus.Subscribe(new SubscribeInfo(handler) { Topic = topic, Mode = mode });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bus"></param>
        /// <param name="topic">消息主题</param>
        /// <param name="handler">处理器</param>
        public static void Unsubscribe(this IMessageBus bus, string topic, Action handler)
        {
            Guard.NotNull(bus, "bus");
            Guard.NotNull(handler, "handler");
            bus.Unsubscribe(topic, null, ObserverHandler.Create(handler));
        }
        #endregion

        #region Remove
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="bus"></param>
        /// <param name="topic">消息主题</param>
        public static void Remove<TData>(this IMessageBus bus, string topic)
        {
            Guard.NotNull(bus,"bus");
            bus.Remove(topic, typeof(TData));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="bus"></param>
        public static void Remove<TData>(this IMessageBus bus)
        {
            Guard.NotNull(bus, "bus");
            bus.Remove(null, typeof(TData));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bus"></param>
        /// <param name="topic">消息主题</param>
        public static void Remove(this IMessageBus bus, string topic)
        {
            Guard.NotNullOrEmpty(topic,"topic");
            bus.Remove(topic, null);
        }
        #endregion

        #region Publish
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="bus"></param>
        /// <param name="topic">消息主题</param>
        /// <param name="sender">sender</param>
        /// <param name="msg">消息</param>
        public static IMessageResponse Publish<TData>(this IMessageBus bus, string topic,object sender, TData msg)
        {
            Guard.NotNull(bus, "bus");
            Guard.NotNull(msg, "msg");

            return bus.Publish(new MessageRequest<TData> { Topic = topic, Data = msg, Sender = sender });
        }

        

       /// <summary>
       /// 
       /// </summary>
       /// <typeparam name="TData"></typeparam>
       /// <param name="bus"></param>
       /// <param name="topic">消息主题</param>
       /// <param name="msg">消息</param>
        public static IMessageResponse Publish<TData>(this IMessageBus bus, string topic, TData msg)
        {
            Guard.NotNull(bus, "bus");
            Guard.NotNull(msg, "msg");
            return bus.Publish(new MessageRequest<TData> { Topic = topic, Data = msg });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="bus"></param>
        /// <param name="sender">sender</param>
        /// <param name="msg">消息</param>
        public static IMessageResponse Publish<TData>(this IMessageBus bus, object sender, TData msg)
        {
            Guard.NotNull(bus, "bus");
            Guard.NotNull(msg, "msg");
            return bus.Publish(new MessageRequest<TData> { Sender = sender, Data = msg });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="bus"></param>
        /// <param name="msg">消息</param>
        public static IMessageResponse Publish<TData>(this IMessageBus bus, TData msg)
        {
            Guard.NotNull(bus, "bus");
            Guard.NotNull(msg, "msg");

            return bus.Publish(new MessageRequest<TData> { Data = msg });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bus"></param>
        /// <param name="topic">消息主题</param>
        public static IMessageResponse Publish(this IMessageBus bus, string topic)
        {
            Guard.NotNull(bus, "bus");
            Guard.NotNullOrEmpty(topic,"topic");

            return bus.Publish(new MessageRequest { Topic = topic });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bus"></param>
        /// <param name="sender">sender</param>
        /// <param name="topic">消息主题</param>
        public static IMessageResponse Publish(this IMessageBus bus, object sender, string topic)
        {
            Guard.NotNull(bus, "bus");
            Guard.NotNullOrEmpty(topic, "topic");

            return bus.Publish(new MessageRequest { Sender = sender, Topic = topic });
        }
        #endregion

        #region Event Attach
        /// <summary>
        /// 
        /// </summary>
        /// <param name="router"></param>
        /// <param name="cfg"></param>
        /// <param name="event"></param>
        /// <returns></returns>
        public static IDisposable Attach(this IMessageBus router,object owner, string @event)
        {
            return Attach(router, null, owner, @event);
        }

       /// <summary>
       /// 
       /// </summary>
       /// <typeparam name="TOwner"></typeparam>
       /// <param name="router"></param>
       /// <param name="cfg"></param>
       /// <param name="evt"></param>
       /// <returns></returns>
        public static IDisposable Attach<TOwner>(this IMessageBus router, TOwner owner,Expression<Func<TOwner, object>> evt)
        {
            var member = evt.FindMember();
            System.Diagnostics.Trace.Assert(member != null);
            return Attach(router,null, owner, member.Name);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TOwner"></typeparam>
        /// <param name="router"></param>
        /// <param name="topic"></param>
        /// <param name="cfg"></param>
        /// <param name="evt"></param>
        /// <returns></returns>
        public static IDisposable Attach<TOwner>(this IMessageBus router, string topic, TOwner owner, Expression<Func<TOwner, object>> evt)
        {
            var member = evt.FindMember();
            System.Diagnostics.Trace.Assert(member != null);
            return Attach(router, topic, owner, member.Name);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="router"></param>
        /// <param name="topic">消息主题</param>
        /// <param name="cfg"></param>
        /// <param name="event"></param>
        /// <returns></returns>
        public static IDisposable Attach(this IMessageBus router, string topic, object owner, string @event)
        {
            if (owner == null)
                throw new ArgumentNullException("owner");

            if (string.IsNullOrEmpty(@event))
                throw new ArgumentNullException("event");

            var ei = owner.GetType().GetEvent(@event, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (ei == null)
                throw new Exception("invalid event:" + @event + " for " + owner.GetType().Name);


            Type eventArgsType = null;
            var ps = ei.EventHandlerType.GetMethod("Invoke").GetParameters();
            if (ps == null || ps.Length == 0)
                throw new NotSupportedException(@event);

            MethodInfo closureInvoke = null;
            if (ps.Length == 1)
            {
                eventArgsType = ps[0].ParameterType;
                closureInvoke = EventRaiser.Invoke1Method.MakeGenericMethod(eventArgsType);
            }
            else if (ps.Length == 2)
            {
                eventArgsType = ps[1].ParameterType;
                closureInvoke = EventRaiser.Invoke2Method.MakeGenericMethod(eventArgsType);
            }

            

            var handler = Delegate.CreateDelegate(ei.EventHandlerType, new EventRaiser(router, topic), closureInvoke);

            var addHandler = ei.GetAddMethod();
            var removeHandler = ei.GetRemoveMethod();

            if (addHandler == null)
                addHandler = ei.GetAddMethod(true);
            if (removeHandler == null)
                removeHandler = ei.GetRemoveMethod(true);

            addHandler.Invoke(owner, new object[] {  handler });

            return Disposable.Create(() => removeHandler.Invoke(owner, new object[] { handler }));
        }
        #endregion
    }
}
