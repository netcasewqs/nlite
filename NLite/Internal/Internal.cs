using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLite.Interceptor.Fluent;
using System.Diagnostics;
using NLite.Interceptor.Matcher;
using NLite.Interceptor.Metadata;
using NLite.Reflection;

namespace NLite.Interceptor.Internal
{
    class NamespaceTargetTypeInfo : INamespaceTargetTypeInfo
    {
        private HashSet<Type> excludes = new HashSet<Type>();

        public string Namespace { get; set; }
        public IEnumerable<Type> Excludes
        {
            get
            {
                return excludes;
            }
        }

        internal void AddExclude(Type type)
        {
            excludes.Add(type);
        }
    }

    class SignleTargetTypeInfo : ISingleTargetTypeInfo
    {
        public Type SingleType { get; set; }
    }

    class AspectInfo : IAspectInfo
    {
        private List<ICutPointInfo> pointCuts = new List<ICutPointInfo>();

        internal void AddPointCut(ICutPointInfo pointCut)
        {
            pointCuts.Add(pointCut);
        }

        public ITargetTypeInfo TargetType { get; set; }
        public IEnumerable<ICutPointInfo> PointCuts { get { return pointCuts; } }
    }

    class PointCutInfo : ICutPointInfo
    {
        public IEnumerable<Type> Advices { get; set; }
        public IMethodSignature Signature { get; set; }
    }

    class MethodSignature : IMethodSignature
    {
        private HashSet<string> arguments;

        public MethodSignature()
        {
            ReturnType = "*";
            Method = "*";
            Flags = CutPointFlags.All;
            Access = AccessFlags.All;
            Deep = 0;

            arguments = new HashSet<string>();
        }


        public int Deep { get; set; }
        public CutPointFlags Flags { get; set; }
        public AccessFlags Access { get; set; }
        public string ReturnType { get; set; }
        public string Method { get; set; }
        public IEnumerable<string> Arguments { get; set; }

        internal void AddArgument(string arg)
        {
            arguments.Add(arg);
        }


    }

    abstract class TargetTypeExpression : IAspectExpression
    {
        private AspectInfo info;

        public TargetTypeExpression()
        {
            info = new AspectInfo
            {
                TargetType = ToTargetType()
            };
        }

        public IPointCutExpression PointCut(CutPointFlags flags)
        {
            var pointCut = new PointCutExpression(flags);
            info.AddPointCut(pointCut);
            return pointCut;
        }

        public IPointCutExpression PointCut()
        {
            return PointCut(CutPointFlags.All);
        }

        public IAspectInfo ToAspect()
        {
            return info;
        }

        protected abstract ITargetTypeInfo ToTargetType();
    }

    class SingleTypeExpression : TargetTypeExpression
    {
        SignleTargetTypeInfo info;

        public SingleTypeExpression(Type singleType)
        {
            info.SingleType = singleType;
        }

        protected override ITargetTypeInfo ToTargetType()
        {
            info = new SignleTargetTypeInfo();
            return info;
        }

    }

    class NamespaceExpression : TargetTypeExpression, INamespaceExpression
    {
       
        NamespaceTargetTypeInfo info;

        public NamespaceExpression(string @namespace)
        {
            Trace.Assert(!string.IsNullOrEmpty(@namespace), "namespace == null");
            info.Namespace = @namespace;
        }



        public INamespaceExpression Exclude<T>()
        {
            info.AddExclude(typeof(T));
            return this;
        }



        protected override ITargetTypeInfo ToTargetType()
        {
            info = new NamespaceTargetTypeInfo();
            return info;
        }
    }



    class PointCutExpression : IPointCutExpression
        , IMethodSignatureExpression
        , IAdviceExpression
        , ICutPointInfo
    {
        
        private HashSet<Type> advices;
        MethodSignature signature;

        public PointCutExpression(CutPointFlags flags)
        {
            signature = new MethodSignature { Flags = flags };
        }

        #region IMethodForPointCutExpression Members

        public IMethodSignatureExpression Deep(int deep)
        {
            this.signature.Deep = deep;
            return this;
        }

        public IMethodSignatureExpression Method(string methodName)
        {
            this.signature.Method = methodName;
            return this;
        }

        public IMethodSignatureExpression Method()
        {
            return Method("*");
        }

        public IMethodSignatureExpression Access(AccessFlags access)
        {
            this.signature.Access = access;
            return this;
        }

        public IMethodSignatureExpression ReturnType(string methodType)
        {
            this.signature.ReturnType = methodType;
            return this;
        }

        public IMethodSignatureExpression Argument(string argument)
        {
            signature.AddArgument(argument);
            return this;
        }

        public IAdviceExpression Advice<T>()
        {
            if (advices == null)
                advices = new HashSet<Type>();
            advices.Add(typeof(T));
            return this;
        }



        #endregion

       

        #region IPointCutInfo Members

        IMethodSignature ICutPointInfo.Signature
        {
            get { return signature; }
        }

        IEnumerable<Type> ICutPointInfo.Advices
        {
            get { return advices; }
        }

        #endregion
    }


    class SingleTypeMatcher : IClassMatcher
    {
        public bool Match(Type targetType, IAspectInfo aspect)
        {
            Type type = GetTypeToCompare(aspect);
            return Match(targetType, type);
        }

        private static bool Match(Type targetType, Type type)
        {
            if (type.IsInterface && targetType.IsClass)
            {
                foreach (var item in targetType.GetInterfaces())
                    return Match(item, type);
            }

            if (type.IsGenericType)
            {
                if (type.IsCloseGenericType())
                {
                    if (targetType.IsOpenGenericType())
                        return type.GetGenericTypeDefinition().IsAssignableFrom(targetType);
                }

                if (targetType.IsCloseGenericType())
                    return type.IsAssignableFrom(targetType.GetGenericTypeDefinition());

            }
            return type.IsAssignableFrom(targetType);
        }

        protected virtual Type GetTypeToCompare(IAspectInfo aspect)
        {
            return (aspect.TargetType as ISingleTargetTypeInfo).SingleType;
        }
    }

    class NamespaceMatcher : IClassMatcher
    {
        public bool Match(Type targetType, IAspectInfo aspect)
        {
            var targetInfo = aspect.TargetType as INamespaceTargetTypeInfo;

            if (targetInfo.Namespace != targetType.Namespace)
                return targetType.Namespace.Contains(targetInfo.Namespace);

            if (targetInfo.Excludes.Contains(targetType))
                return false;

            return true;
        }

    }
}
