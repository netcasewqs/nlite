using System;
using System.Reflection;
using System.Linq;
using NLite.Mini.Context;
using NLite.Reflection;
using NLite.Reflection.Internal;
using System.Collections.Generic;

namespace NLite.Mini.Resolving
{

    interface IExportInfo
    {
        string Id { get; }
        void Execute(IComponentContext ctx);
    }

    abstract class AbstractExportInfo:IExportInfo
    {
        public string Id { get; set; }
        public abstract void Execute(IComponentContext ctx);
    }
   

    class FieldExportInfo : AbstractExportInfo
    {
        public FieldInfo Field;
        public Getter Getter;
        public override void Execute(IComponentContext ctx)
        {
            ctx.Kernel.RegisterInstance(Id, Field.FieldType, Getter(ctx.Instance));
        }
    }

    class PropertyExportInfo : AbstractExportInfo
    {
        public PropertyInfo Property;
        public Getter Getter;
        public override void Execute(IComponentContext ctx)
        {
            ctx.Kernel.RegisterInstance(Id, Property.PropertyType, Getter(ctx.Instance));
        }
    }

    class MethodExportInfo : AbstractExportInfo
    {
        public MethodInfo Method;
        public Type Contract;
        public override void Execute(IComponentContext ctx)
        {
            ctx.Kernel.RegisterInstance(Id, Contract, Delegate.CreateDelegate(Contract, ctx.Instance, Method));
        }
    }

    class ExportInfo
    {
        public string Id;
        public Type MemberType;
        public MemberInfo Member;
        public Getter Getter;
        //public Type Contract;
    }
   
}
