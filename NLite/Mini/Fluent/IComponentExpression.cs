
using System;
namespace NLite.Mini.Fluent
{
    /// <summary>
    /// 组件表达是接口
    /// </summary>
    public partial interface IComponentExpression : IRegistryExpression
    {
        /// <summary>
        /// 设置Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IComponentExpression Named(string id);
        /// <summary>
        /// 设置组件生命周期类型
        /// </summary>
        /// <param  name="lifestyle"></param>
        /// <returns></returns>
        IComponentExpression Lifestyle(LifestyleFlags lifestyle);

        /// <summary>
        /// 设置组件工厂类型
        /// </summary>
        /// <param name="activator"></param>
        /// <returns></returns>
        IComponentExpression Activator(string activator);

        /// <summary>
        /// 添加组件的一个元数据项
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        IComponentExpression Add(string key, object value);

        /// <summary>
        /// 设置契约类型
        /// </summary>
        /// <param name="contract"></param>
        /// <returns></returns>
        IComponentExpression Bind(Type contract);

        /// <summary>
        /// 设置契约类型
        /// </summary>
        /// <typeparam name="TContract"></typeparam>
        /// <returns></returns>
        IComponentExpression Bind<TContract>();

        /// <summary>
        /// 设置契约类型和Id
        /// </summary>
        /// <param name="contract"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        IComponentExpression Bind(Type contract, string id);

        /// <summary>
        /// 设置契约类型和Id
        /// </summary>
        /// <typeparam name="TContract"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        IComponentExpression Bind<TContract>(string id);

        /// <summary>
        /// 设置组件类型
        /// </summary>
        /// <param name="componentType"></param>
        /// <returns></returns>
        IComponentExpression To(Type componentType);

        /// <summary>
        /// 设置组件类型
        /// </summary>
        /// <typeparam name="TComponent"></typeparam>
        /// <returns></returns>
        IComponentExpression To<TComponent>();

        /// <summary>
        /// 设置组件的工厂方法
        /// </summary>
        /// <param name="factory"></param>
        /// <returns></returns>
        IComponentExpression Factory(Func<object> factory);


    }
}
