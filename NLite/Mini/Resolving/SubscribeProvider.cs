using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using NLite.Messaging;
using NLite.Reflection;

namespace NLite.Mini.Resolving
{
    class SubscribeProvider
    {
        public Func<object, ISubscribeInfo> Creator;
    }

    class ActionSubscribeProvider : SubscribeProvider
    {
        static readonly Type ActionType = typeof(Action);
        public ActionSubscribeProvider(MethodInfo method,string topic, SubscribeMode mode)
        {
            Creator = instance =>
            {
                var handler = Delegate.CreateDelegate(ActionType, instance, method);
                var messageHandler = ObserverHandler.Create((Action)handler);
                return new SubscribeInfo(null, messageHandler) { Topic = topic, Mode = mode };
            };
        }
    }

    class Action1SubscribeProvider : SubscribeProvider
    {
        static readonly Type Action1Type = typeof(Action<>);
        static readonly MethodInfo Create1HandlerMethod = typeof(ObserverHandler).GetMethod("Create1", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

        public Action1SubscribeProvider(MethodInfo method, ParameterInfo[] ps, string topic, SubscribeMode mode)
        {
            var msgType = ps[0].ParameterType;
            var handlerType = Action1Type.MakeGenericType(msgType);
            Creator = instance =>
            {
                var handler = Delegate.CreateDelegate(handlerType, instance, method);
                var messageHandler = (IObserverHandler<IMessage>)Create1HandlerMethod.MakeGenericMethod(msgType).FastFuncInvoke(null, handler);
                return new SubscribeInfo(msgType, messageHandler) { Topic = topic, Mode = mode };
            };
       }
    }

    class Action2SubscribeProvider : SubscribeProvider
    {
        static readonly Type Action2Type = typeof(Action<,>);
        static readonly MethodInfo Create2HandlerMethod = typeof(ObserverHandler).GetMethod("Create2", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

        public Action2SubscribeProvider(MethodInfo method, ParameterInfo[] ps, string topic, SubscribeMode mode)
        {
            var msgType = ps[1].ParameterType;
            var handlerType = Action2Type.MakeGenericType(Types.Object, msgType);
            
            Creator = instance =>
                {
                    var handler = Delegate.CreateDelegate(handlerType, instance, method);
                    var messageHandler =(IObserverHandler<IMessage>)Create2HandlerMethod.MakeGenericMethod(msgType).FastFuncInvoke(null, handler);
                    return new SubscribeInfo(msgType, messageHandler) { Topic = topic, Mode = mode };
                };
        }
    }

    class FuncSubscribeProvider : SubscribeProvider
    {
        static readonly Type FuncAType = typeof(Func<>);
        static readonly MethodInfo CreateAHandlerMethod = typeof(ObserverHandler).GetMethod("CreateA", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

        public FuncSubscribeProvider(MethodInfo method,string topic, SubscribeMode mode)
        {
            var handlerType = FuncAType.MakeGenericType(method.ReturnType);

           Creator = instance =>
           {
               var handler = Delegate.CreateDelegate(handlerType, instance, method);
               var messageHandler = (IObserverHandler<IMessage>)CreateAHandlerMethod.MakeGenericMethod(method.ReturnType).FastFuncInvoke(null, handler);
               return new SubscribeInfo(null, messageHandler) { Topic = topic, Mode = mode };
           };
        }
    }

    class Func1SubscribeProvider : SubscribeProvider
    {
        static readonly Type FuncBType = typeof(Func<,>);
        static readonly MethodInfo CreateBHandlerMethod = typeof(ObserverHandler).GetMethod("CreateB", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

        public Func1SubscribeProvider(MethodInfo method, ParameterInfo[] ps, string topic, SubscribeMode mode)
        {
            var msgType = ps[0].ParameterType;
            var handlerType = FuncBType.MakeGenericType(msgType, method.ReturnType);
            Creator = instance =>
            {
                var handler = Delegate.CreateDelegate(handlerType, instance, method);
                var messageHandler = (IObserverHandler<IMessage>)CreateBHandlerMethod.MakeGenericMethod(msgType, method.ReturnType).FastFuncInvoke(null, handler);
                return new SubscribeInfo(msgType, messageHandler) { Topic = topic, Mode = mode };
            };
        }
    }

    class Func2SubscribeProvider : SubscribeProvider
    {
        static readonly Type FuncCType = typeof(Func<,,,>);
        static readonly MethodInfo CreateCHandlerMethod = typeof(ObserverHandler).GetMethod("CreateC", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

        public Func2SubscribeProvider(MethodInfo method, ParameterInfo[] ps, string topic, SubscribeMode mode)
        {
            var msgType = ps[1].ParameterType;
            var handlerType = FuncCType.MakeGenericType(Types.Object, msgType, method.ReturnType);

            Creator = instance =>
                {
                    var handler = Delegate.CreateDelegate(handlerType, instance, method);
                    var messageHandler = (IObserverHandler<IMessage>)CreateCHandlerMethod.MakeGenericMethod(msgType, method.ReturnType).FastFuncInvoke(null, handler);
                    return new SubscribeInfo(msgType, messageHandler) { Topic = topic, Mode = mode };
                };
        }
    }
}
