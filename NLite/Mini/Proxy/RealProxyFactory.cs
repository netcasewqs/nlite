#if !SILVERLIGHT
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Remoting.Proxies;
using System.Runtime.Remoting;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using NLite.Interceptor;
using System.Reflection.Emit;
using System.Threading;
using NLite.Mini.Proxy.Internal;
using NLite.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Remoting.Activation;
using System.Security.Permissions;
using NLite.Mini.Context;

namespace NLite.Mini.Proxy
{
    /// <summary>
    /// 
    /// </summary>
    public static class ProxyFactory
    {
       /// <summary>
       /// 
       /// </summary>
        public static readonly IProxyFactory Default = new EmitProxyFactory();
    }

    namespace Internal
    {
        //[Obsolete]
        //class RealProxyFactory : IProxyFactory
        //{
        //    public object Create(Context.IComponentContext ctx, object instance, params object[] constructorArguments)
        //    {
        //        if (instance != null)
        //        {
        //            var interfaces = ctx.Component.Contracts.Where(i => i.IsInterface).ToArray();
        //            var marshallableObj = instance as MarshalByRefObject;

        //            if (interfaces.Length == 0 && marshallableObj == null)
        //                return instance;

        //            var methods = (ctx.Component.ExtendedProperties["methods"] as MethodInfo[]).Select(p=>p.Name).ToArray();

        //            InvocationDelegate onInvocation = (target, method, parameters) =>
        //                {
        //                    var m = method as MethodInfo;
        //                    if (methods.FirstOrDefault(p => p == m.Name) == null)
        //                        return m.Invoke(target, parameters);
        //                    var invocation = new InvocationInfo(target, method as MethodInfo, parameters);
                           
        //                    var wrapper = new DefaultInvocationHandler(target, methods);

        //                    return wrapper.Invoke(invocation);
        //                };
        //            if (marshallableObj != null)
        //                return new MarshallableDynamicProxyImpl(marshallableObj, onInvocation)
        //                    .GetTransparentProxy();

        //            return new InterfaceDynamicProxyImpl(instance, onInvocation, true, interfaces)
        //                .GetTransparentProxy();
        //        }
        //        throw new NotImplementedException();
        //    }

        //    public bool RequiresTargetInstance(IKernel kernel, IComponentInfo model)
        //    {
        //        return true;
        //    }

        //    delegate object InvocationDelegate(object target, MethodBase method, object[] parameters);
        //    interface IDynamicProxy
        //    {
        //    }
        //    class InterfaceDynamicProxyImpl : RealProxy, IRemotingTypeInfo, IDynamicProxy
        //    {
        //        private object proxyTarget;
        //        private bool strict;
        //        private Type[] supportedTypes;
        //        private InvocationDelegate invocationHandler;

        //        protected internal InterfaceDynamicProxyImpl(object proxyTarget, InvocationDelegate invocationHandler, bool strict, Type[] supportedTypes)
        //            : base(typeof(IDynamicProxy))
        //        {
        //            this.proxyTarget = proxyTarget;
        //            this.invocationHandler = invocationHandler;
        //            this.strict = strict;
        //            this.supportedTypes = supportedTypes;
        //        }

        //        public override ObjRef CreateObjRef(System.Type type)
        //        {
        //            throw new NotSupportedException("ObjRef for DynamicProxy isn't supported");
        //        }

        //        public bool CanCastTo(System.Type toType, object obj)
        //        {
        //            // Assume we can (which is the default unless strict is true)
        //            bool canCast = true;

        //            if (strict)
        //            {
        //                // First check if the proxyTarget supports the cast
        //                if (toType.IsAssignableFrom(proxyTarget.GetType()))
        //                {
        //                    canCast = true;
        //                }
        //                else if (supportedTypes != null)
        //                {
        //                    canCast = false;
        //                    // Check if the list of supported interfaces supports the cast
        //                    foreach (Type type in supportedTypes)
        //                    {
        //                        if (toType == type)
        //                        {
        //                            canCast = true;
        //                            break;
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    canCast = false;
        //                }
        //            }

        //            return canCast;
        //        }

        //        public override IMessage Invoke(IMessage message)
        //        {
        //            var methodMessage = new MethodCallMessageWrapper((IMethodCallMessage)message);
        //            var method = methodMessage.MethodBase;

        //            object returnValue = null;
        //            if (method.DeclaringType == typeof(IDynamicProxy))
        //                returnValue = method.Invoke(this, methodMessage.Args);
        //            else
        //            {
        //                returnValue = invocationHandler(proxyTarget, method, methodMessage.Args);
        //            }

        //            return new ReturnMessage(returnValue, methodMessage.Args, methodMessage.ArgCount, methodMessage.LogicalCallContext, methodMessage);
        //        }

        //        string IRemotingTypeInfo.TypeName
        //        {
        //            get
        //            {
        //                throw new NotImplementedException();
        //            }
        //            set
        //            {
        //                throw new NotImplementedException();
        //            }
        //        }
        //    }
        //    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        //    class MarshallableDynamicProxyImpl : RealProxy, IDynamicProxy
        //    {
        //        public string URI { get; protected set; }
        //        public MarshalByRefObject ProxyTargetTyped { get; protected set; }
        //        public object ProxyTarget { get { return ProxyTargetTyped; } }
        //        public InvocationDelegate InvocationHandler { get; set; }

        //        public MarshallableDynamicProxyImpl(MarshalByRefObject targetObject, InvocationDelegate invoker)
        //            : this(targetObject.GetType(), targetObject, invoker)
        //        {
        //        }


        //        public MarshallableDynamicProxyImpl(Type type1, MarshalByRefObject targetObject, InvocationDelegate invoker)
        //            : base(type1)
        //        {
        //            ProxyTargetTyped = targetObject;
        //            InvocationHandler = invoker;

        //            ObjRef myObjRef = RemotingServices.Marshal(ProxyTargetTyped);
        //            URI = myObjRef.URI;
        //        }

        //        public override IMessage Invoke(IMessage message)
        //        {
        //            if (message is IConstructionCallMessage)
        //            {
        //                IConstructionReturnMessage myIConstructionReturnMessage =
        //                   this.InitializeServerObject((IConstructionCallMessage)message);
        //                ConstructionResponse constructionResponse = new
        //                   ConstructionResponse(null, (IMethodCallMessage)message);
        //                return constructionResponse;
        //            }
        //            IMethodCallMessage methodMessage = new MethodCallMessageWrapper((IMethodCallMessage)message);

        //            MethodBase method = methodMessage.MethodBase;

        //            object returnValue = null;
        //            if (method.DeclaringType == typeof(IDynamicProxy))
        //                returnValue = method.Invoke(this, methodMessage.Args);
        //            else
        //                returnValue = InvocationHandler(ProxyTargetTyped, method, methodMessage.Args);

        //            ReturnMessage returnMessage = new ReturnMessage(returnValue, methodMessage.Args, methodMessage.ArgCount, methodMessage.LogicalCallContext, methodMessage);
        //            return returnMessage;
        //        }
        //        public override ObjRef CreateObjRef(Type ServerType)
        //        {
        //            CustomObjRef myObjRef = new CustomObjRef(ProxyTargetTyped, ServerType);
        //            myObjRef.URI = URI;
        //            return myObjRef;
        //        }
        //        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        //        {
        //            base.GetObjectData(info, context);
        //        }


        //        public class CustomObjRef : ObjRef
        //        {
        //            public MarshalByRefObject RealObject { get; set; }

        //            public CustomObjRef(MarshalByRefObject obj, Type type)
        //                : base(obj, type)
        //            {
        //                RealObject = obj;
        //            }

        //            public override void GetObjectData(SerializationInfo info,
        //                                               StreamingContext context)
        //            {
        //                base.GetObjectData(info, context);
        //            }

        //            public override object GetRealObject(StreamingContext context)
        //            {
        //                return this.RealObject;
        //            }
        //        }


        //    }
        //}

    }
}
#endif