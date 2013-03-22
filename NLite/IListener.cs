
using System;
using System.Diagnostics;
namespace NLite
{
    /// <summary>
    /// 监听器接口
    /// </summary>
    /// <remarks>充当观察者模式中的抽象观察者角色</remarks>
    public interface IListener
    {

    }

    ///// <summary>
    ///// 
    ///// </summary>
    ///// <typeparam name="TEnum"></typeparam>
    //public interface IListener<TEnum>
    //{
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    TEnum Type { get; }
    //}

    /// <summary>
    /// 监听器
    /// </summary>
    public class Listener : IListener
    {
    }

    ///// <summary>
    ///// 
    ///// </summary>
    ///// <typeparam name="TEnum"></typeparam>
    //public class Listener<TEnum> : Listener
    //{
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    public TEnum Type { get; private set; }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="binderType"></param>
    //    protected Listener(TEnum binderType)
    //    {
    //        Trace.Assert(binderType != null, "binderType == null");
    //        //Trace.Assert(Enum.IsDefined(binderType.GetType(), binderType), "binderType does not Enum binderType");
    //        Type = binderType;
    //    }
    //}
}
