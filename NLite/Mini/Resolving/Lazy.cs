using System;
using NLite.Reflection;


namespace NLite.Mini.Resolving
{
   
   
    static class Lazy
    {
        class LazyWrapper<T> : Lazy<T>,ILazy
        {
            public IKernel Kernel { get; private set; }
            public IComponentInfo Component { get; private set; }
            public LazyWrapper(Func<object> func, IKernel kernal, IComponentInfo component)
                : base(ConvertStrongTypeFunc(func))
            {
                Kernel = kernal;
                Component = component;
            }

            private static Func<T> ConvertStrongTypeFunc(Func<object> func)
            {
                Func<T> result = null;
                result = () => (T)func();
                return result;
            }
        }

        class LazyWrapper<T, TMetadata> : Lazy<T, TMetadata>, ILazy
        {
            public IKernel Kernel { get; private set; }
            public IComponentInfo Component { get; private set; }
            public LazyWrapper(Func<object> valueFactory, TMetadata metadata, IKernel kernal, IComponentInfo component) :
                base(ConvertStrongTypeFunc(valueFactory),metadata)
            {
                Kernel = kernal;
                Component = component;
            }

            private static Func<T> ConvertStrongTypeFunc(Func<object> func)
            {
                Func<T> result = null;
                result = () => (T)func();
                return result;
            }
        }

        static Type KernelType = typeof(IKernel);
        static Type ComponentType = typeof(IComponentInfo);
        static Type[] constructorArgsTypeArray = new Type[] { typeof(Func<object>), KernelType ,ComponentType};

        public static Func<object> GetLazyValueProvider(Type lazyElementType, Func<object> func,IKernel kernel,IComponentInfo component)
        {
            var lazyType = typeof(LazyWrapper<>).MakeGenericType(lazyElementType);
            var creator = lazyType.GetConstructor(constructorArgsTypeArray).GetCreator();
            return () => creator.Invoke(func,kernel,component);
        }

        public static Func<object> GetLazyCreator(Type lazyElementType, Type metadataType, Func<object> func, object metadata, IKernel kernel, IComponentInfo component)
        {
            var lazyType = typeof(LazyWrapper<,>).MakeGenericType(lazyElementType, metadataType);
            var creator = lazyType.GetConstructor(new Type[] { typeof(Func<object>), metadataType,KernelType,ComponentType }).GetCreator();

            return () => creator.Invoke(func, metadata, kernel, component);
        }
    }

    
    static class CollectionLazy
    {
        class LazyWrapper<T> : Lazy<T>, ICollectonLazy
        {
            public IKernel[] Kernels { get; private set; }
            public IComponentInfo[] Components { get; private set; }

            public LazyWrapper(Func<object> func, IKernel[] kernels, IComponentInfo[] components)
                : base(ConvertStrongTypeFunc(func))
            {
                Kernels = kernels;
                Components = components;
            }

            private static Func<T> ConvertStrongTypeFunc(Func<object> func)
            {
                Func<T> result = null;
                result = () => (T)func();
                return result;
            }
        }

        class LazyWrapper<T, TMetadata> : Lazy<T, TMetadata>, ICollectonLazy
        {
            public IKernel[] Kernels { get; private set; }
            public IComponentInfo[] Components { get; private set; }

            public LazyWrapper(Func<object> valueFactory, TMetadata metadata, IKernel[] kernels, IComponentInfo[] components) :
                base(ConvertStrongTypeFunc(valueFactory), metadata)
            {
                Kernels = kernels;
                Components = components;
            }

            private static Func<T> ConvertStrongTypeFunc(Func<object> func)
            {
                Func<T> result = null;
                result = () => (T)func();
                return result;
            }
        }

        public static Func<object> GetLazyValueProvider(Type lazyElementType, Func<object> func, IKernel[] kernels, IComponentInfo[] components)
        {
            var lazyType = typeof(LazyWrapper<>).MakeGenericType(lazyElementType);
            var creator = lazyType.GetConstructor(new Type[] { typeof(Func<object>) ,typeof(IKernel[]),typeof(IComponentInfo[])}).GetCreator();
            return () => creator.Invoke(func,kernels,components);
        }

        public static Func<object> GetLazyCreator(Type lazyElementType, Type metadataType, Func<object> func, object metadata, IKernel[] kernels, IComponentInfo[] components)
        {
            var lazyType = typeof(LazyWrapper<,>).MakeGenericType(lazyElementType, metadataType);
            var creator = lazyType.GetConstructor(new Type[] { typeof(Func<object>), metadataType, typeof(IKernel[]), typeof(IComponentInfo[]) }).GetCreator();

            return () => creator.Invoke(func, metadata, kernels, components);
        }
    }
}
